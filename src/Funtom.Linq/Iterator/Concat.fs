namespace Funtom.linq.Iterator

open Funtom.linq
open System.Collections
open System.Collections.Generic

module rec Concat =
  open Basis
  open Interfaces
  open Enumerable

  // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/Concat.cs#L190
  // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/Concat.SpeedOpt.cs#L195
  [<AbstractClass>]
  type ConcatIterator<'T> () =
    inherit Iterator<'T>()
    let mutable enumerator = defaultof<IEnumerator<'T>>
    
    abstract member GetEnumerable : int -> seq<'T>
    abstract member Concat : seq<'T> -> ConcatIterator<'T>
    abstract member GetCount : bool -> int
    abstract member ToArray : unit -> 'T[]

    override __.Dispose() =
      if enumerator |> isNotDefault then
        enumerator.Dispose()
        enumerator <- defaultof<IEnumerator<'T>>
    
    override __.MoveNext() : bool =
      if __.state = 1 then
        enumerator <- __.GetEnumerable(0).GetEnumerator()
        __.state <- 2

      if 1 < __.state then
        let mutable break' = false
        let mutable result = false
        while not break' do
          if enumerator.MoveNext() then
            __.current <- enumerator.Current
            result <- true
            break' <- true

          let next = __.GetEnumerable(__.state - 1)
          __.state <- __.state + 1
          if next |> isNotDefault then
            enumerator.Dispose()
            enumerator <- next.GetEnumerator()
          else
            __.Dispose()
            result <- false
            break' <- true

        result
      else
        false

    member __.ToList() =
      let count = __.GetCount(true)
      let list = if count <> -1 then ResizeArray<'T>(count) else ResizeArray()
      let rec fn (i: int) =
        match __.GetEnumerable(i) with
        | null -> ()
        | xs -> list.AddRange(xs); fn(i + 1)
      fn(0)
      list

    interface IListProvider<'T> with
      member __.ToArray() = __.ToArray()
      member __.ToList() = __.ToList()
      member __.GetCount(onlyIfCheap: bool) = __.GetCount(onlyIfCheap)
  
  // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/Concat.cs#L32
  // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/Concat.SpeedOpt.cs#L11
  [<Sealed>]
  type Concat2Iterator<'T> (first: seq<'T>, second: seq<'T>) =
    inherit ConcatIterator<'T> ()
    member __.first = first
    member __.second = second
    override __.Clone() = new Concat2Iterator<'T>(first, second)
    override __.Concat(next: seq<'T>) =
      let hasOnlyCollections = 
        match (next, first, second) with
        | (:? ICollection<'T>), (:? ICollection<'T>), (:? ICollection<'T>) -> true
        | _ -> false
      new ConcatNIterator<'T>(__, next, 2, hasOnlyCollections) 

    override __.GetEnumerable (index: int) =
      match index with
      | 0 -> first
      | 1 -> second
      | _ -> defaultof<seq<'T>>

    override __.GetCount(onlyIfCheap: bool) =
      let firstCount = 
        match tryGetNonEnumeratedCount first with
        | (true, v) -> v
        | (false, _) -> if onlyIfCheap then -1 else first |> count

      let secondCount = 
        match tryGetNonEnumeratedCount second with
        | (true, v) -> v
        | (false, _) -> if onlyIfCheap then -1 else second |> count

      match (firstCount, secondCount) with
      | (-1, _) | (_, -1) -> -1
      | _ -> Checked.(+) firstCount secondCount
      
    override __.ToArray() =
      let mutable builder = SparseArrayBuilder.Create()
      let reversedFirst = builder.ReserveOrAdd first
      let reversedSecond = builder.ReserveOrAdd second
      let array = builder.ToArray()
      
      if reversedFirst then
        let marker = builder.markers.First()
        Enumerable.copy(first, array, 0, marker.count)

      if reversedSecond then
        let marker = builder.markers.Last()
        Enumerable.copy(second, array, marker.index, marker.count)

      array

  // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/Concat.cs#L93
  // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/Concat.SpeedOpt.cs#L65
  [<Sealed>]
  type ConcatNIterator<'T> (tail: ConcatIterator<'T>, head: seq<'T>, headIndex: int, hasOnlyCollections: bool) =
    inherit ConcatIterator<'T> ()
    member private __.headIndex = headIndex
    member private __.tail = tail
    member private __.head = head
    member private __.previousN with get() = match tail with :? ConcatNIterator<'T> as n -> n | _ -> defaultof<ConcatNIterator<'T>>
    override __.Clone() = new ConcatNIterator<'T> (tail, head, headIndex, hasOnlyCollections)

    override __.Concat(next: seq<'T>) =
      if headIndex = System.Int32.MaxValue - 2
      then
        new Concat2Iterator<'T>(__, next)
      else
        let hasOnlyCollections = hasOnlyCollections && (match next with :? ICollection<'T> -> true | _ -> false)
        new ConcatNIterator<'T>(__, next, headIndex + 1, hasOnlyCollections)

    override __.GetEnumerable (index: int) =
      if headIndex < index
      then 
        defaultof<seq<'T>>
      else
        let mutable node = defaultof<ConcatNIterator<'T>>
        let mutable previousN = __
        let rec search() =
          node <- previousN
          if index = node.headIndex 
          then
            node.head
          else
            previousN <- node.previousN
            if previousN <> defaultof<ConcatNIterator<'T>>
            then search()
            else defaultof<seq<'T>>
        let result = search()
        if result <> defaultof<seq<'T>>
        then result
        else node.tail.GetEnumerable(index)

    // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/Concat.SpeedOpt.cs#L67
    override __.GetCount (onlyIfCheap: bool) =
      if onlyIfCheap && not hasOnlyCollections 
      then -1
      else
        let mutable count = 0        
        let mutable node = defaultof<ConcatNIterator<'T>>
        let mutable previousN = __
        let rec loop() =
          node <- previousN
          let src = node.head
          let srcCount = Enumerable.count src
          count <- Checked.(+) count srcCount

          previousN <- node.previousN
          if previousN <> defaultof<ConcatNIterator<'T>>
          then loop()
          else ()
        loop()
        Checked.(+) count (node.tail.GetCount(onlyIfCheap))

    // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/Concat.SpeedOpt.cs#L99
    override __.ToArray() = if hasOnlyCollections then __.preallocatingToArray() else __.lazyToArray()

    // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/Concat.SpeedOpt.cs#L101
    member private __.lazyToArray() =
      let mutable builder = SparseArrayBuilder.Create()
      let mutable deferredCopies = ArrayBuilder<int>(4)
      let rec loop (i: int) =
        let source = __.GetEnumerable(i)
        if source = defaultof<seq<'T>>
        then ()
        else
          if builder.ReserveOrAdd source then deferredCopies.Add(i)
          loop(i + 1)

      loop 0
      let array = builder.ToArray()
      let mutable markers = builder.markers
      for i = 0 to (markers.Count - 1) do
        let marker = markers[i]
        let source = __.GetEnumerable(deferredCopies[i])
        Enumerable.copy(source, array, marker.index, marker.count)

      array

    // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/Concat.SpeedOpt.cs#L140
    member private __.preallocatingToArray() =
      let count = __.GetCount(true)
      if count = 0
      then
        Array.empty<'T>
      else
        let array = Array.zeroCreate<'T> count
        let mutable index = array.Length

        let mutable node = defaultof<ConcatNIterator<'T>>
        let mutable previousN = __
        let rec loop() =
          node <- previousN
          let src = node.head :?> ICollection<'T>
          let srcCount = src.Count
          if 0 < srcCount then 
            index <- Checked.(-) index srcCount
            src.CopyTo(array, index)
          if previousN <> defaultof<ConcatNIterator<'T>>
          then loop()
          else ()
        loop()

        let mutable previous2 = node.tail :?> Concat2Iterator<'T>
        let mutable second = previous2.second :?> ICollection<'T>
        let secondCount = second.Count
        if 0 < secondCount then second.CopyTo(array, Checked.(-) index secondCount)
        if secondCount < index then (previous2.first :?> ICollection<'T>).CopyTo(array, 0)
        array
        