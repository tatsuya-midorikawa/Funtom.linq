namespace Funtom.Linq.Iterator

open Funtom.Linq
open System.Collections
open System.Collections.Generic

module rec Concat =
  open Basis
  open Interfaces
  open Count

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
      let mutable secoundCount = 0
      match first.tryGetNonEnumerateCount() with
      | (true, v) -> v
      | (false, _) -> if onlyIfCheap then -1 else first.Count()
      let (b, firstCount) = first.tryGetNonEnumerateCount()

      0
      

  // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/Concat.cs#L93
  [<Sealed>]
  type ConcatNIterator<'T> (tail: ConcatIterator<'T>, head: seq<'T>, headIndex: int, hasOnlyCollections: bool) =
    inherit ConcatIterator<'T> ()