namespace Funtom.Linq.Iterator

open Funtom.Linq
open System.Collections
open System.Collections.Generic

module Distinct =
  open Basis
  open Interfaces
  open Enumerable

  let private DefaultInternalSetCapacity = 7

  // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/Distinct.cs#L62
  // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/Distinct.SpeedOpt.cs#L10
  [<Sealed>]
  type DistinctIterator<'T> (source: seq<'T>, comparer: IEqualityComparer<'T>) =
    inherit Iterator<'T>()
    let mutable enumerator = defaultof<IEnumerator<'T>>
    let mutable set = defaultof<HashSet<'T>>

    override __.Clone() = new DistinctIterator<'T> (source, comparer)

    override __.Dispose() =
      if enumerator |> isNotDefault then
        enumerator.Dispose()
        enumerator <- defaultof<IEnumerator<'T>>
        set <- defaultof<HashSet<'T>>
      base.Dispose()

    override __.MoveNext() : bool =
      if __.state = 1 then
        enumerator <- source.GetEnumerator()
        if enumerator.MoveNext()
        then
          let element = enumerator.Current
          set <- HashSet<'T>(DefaultInternalSetCapacity, comparer)
          set.Add(element) |> ignore
          __.current <- element
          __.state <- 2
          true
        else
          __.Dispose()
          false
      else if __.state = 2 then
        let rec loop() =
          if enumerator.MoveNext()
          then
            let element = enumerator.Current
            if set.Add(element)
            then __.current <- element; true
            else loop()
          else
            __.Dispose()
            false
        loop ()
      else
        __.Dispose()
        false

    member __.ToArray() = Enumerable.hashseToArray(HashSet<'T>(source, comparer))
    member __.ToList() = Enumerable.hashseToList(HashSet<'T>(source, comparer))
    member __.GetCount(onlyIfCheap: bool) = if onlyIfCheap then -1 else HashSet<'T>(source, comparer).Count

    interface IListProvider<'T> with
      member __.ToArray() = __.ToArray()
      member __.ToList() = __.ToList()
      member __.GetCount(onlyIfCheap: bool) = __.GetCount(onlyIfCheap)

  [<Sealed>]
  type DistinctListIterator<'T> (source: list<'T>, comparer: IEqualityComparer<'T>) =
    inherit Iterator<'T>()
    let mutable source' = source
    let mutable set = defaultof<HashSet<'T>>

    override __.Clone() = new DistinctIterator<'T> (source, comparer)

    override __.Dispose() =
      set <- defaultof<HashSet<'T>>
      base.Dispose()

    override __.MoveNext() : bool =
      if __.state = 1 then
        match source' with
        | h::tail ->
          set <- HashSet<'T>(DefaultInternalSetCapacity, comparer)
          set.Add(h) |> ignore
          __.current <- h
          source' <- tail
          __.state <- 2
          true
        | _ -> __.Dispose(); false
      else if __.state = 2 then
        let rec loop(xs: list<'T>) =
          match xs with
          | h::tail ->
            if set.Add h
            then 
              __.current <- h
              source' <- tail
              true
            else
              loop(tail)
          | _ ->
            __.Dispose()
            false
        loop (source')
      else
        __.Dispose()
        false

    member __.ToArray() = Enumerable.hashseToArray(HashSet<'T>(source, comparer))
    member __.ToList() = Enumerable.hashseToList(HashSet<'T>(source, comparer))
    member __.GetCount(onlyIfCheap: bool) = if onlyIfCheap then -1 else HashSet<'T>(source, comparer).Count

    interface IListProvider<'T> with
      member __.ToArray() = __.ToArray()
      member __.ToList() = __.ToList()
      member __.GetCount(onlyIfCheap: bool) = __.GetCount(onlyIfCheap)

  [<Sealed>]
  type DistinctArrayIterator<'T> (source: array<'T>, comparer: IEqualityComparer<'T>) =
    inherit Iterator<'T>()
    let mutable set = defaultof<HashSet<'T>>
    let mutable index = 0

    override __.Clone() = new DistinctIterator<'T> (source, comparer)

    override __.Dispose() =
      set <- defaultof<HashSet<'T>>
      base.Dispose()

    override __.MoveNext() : bool =
      if __.state = 1 then
        if source.Length = 0
        then __.Dispose(); false
        else
          let element = source[index]
          set <- HashSet<'T>(DefaultInternalSetCapacity, comparer)
          set.Add(element) |> ignore
          __.current <- element
          __.state <- 2
          index <- index + 1
          true
      else if __.state = 2 then
        let rec loop() =
          if index < source.Length
          then
            let element = source[index]
            index <- index + 1;
            if set.Add(element)
            then __.current <- element; true
            else loop()
          else 
            __.Dispose()
            false
        loop ()
      else
        __.Dispose()
        false

  [<Sealed>]
  type DistinctResizeArrayIterator<'T> (source: ResizeArray<'T>, comparer: IEqualityComparer<'T>) =
    inherit Iterator<'T>()
    let mutable set = defaultof<HashSet<'T>>
    let mutable index = 0

    override __.Clone() = new DistinctIterator<'T> (source, comparer)

    override __.Dispose() =
      set <- defaultof<HashSet<'T>>
      base.Dispose()

    override __.MoveNext() : bool =
      if __.state = 1 then
        if source.Count = 0
        then __.Dispose(); false
        else
          let element = source[index]
          set <- HashSet<'T>(DefaultInternalSetCapacity, comparer)
          set.Add(element) |> ignore
          __.current <- element
          __.state <- 2
          index <- index + 1
          true
      else if __.state = 2 then
        let rec loop() =
          if index < source.Count
          then
            let element = source[index]
            index <- index + 1;
            if set.Add(element)
            then __.current <- element; true
            else loop()
          else 
            __.Dispose()
            false
        loop ()
      else
        __.Dispose()
        false

  [<Sealed>]
  type DistinctByIterator<'T, 'U> (source: seq<'T>, [<InlineIfLambda>]selector: 'T -> 'U, comparer: IEqualityComparer<'U>) =
    inherit Iterator<'T>()
    let mutable enumerator = defaultof<IEnumerator<'T>>
    let mutable set = defaultof<HashSet<'U>>

    override __.Clone() = new DistinctByIterator<'T, 'U> (source, selector, comparer)

    override __.Dispose() =
      if enumerator |> isNotDefault then
        enumerator.Dispose()
        enumerator <- defaultof<IEnumerator<'T>>
        set <- defaultof<HashSet<'U>>
      base.Dispose()

    override __.MoveNext() : bool =
      if __.state = 1 then
        enumerator <- source.GetEnumerator()
        if enumerator.MoveNext()
        then
          let element = enumerator.Current
          set <- HashSet<'U>(DefaultInternalSetCapacity, comparer)
          set.Add(selector element) |> ignore
          __.current <- element
          __.state <- 2
          true
        else
          __.Dispose()
          false
      else if __.state = 2 then
        let rec loop() =
          if enumerator.MoveNext()
          then
            let element = enumerator.Current
            if set.Add(selector element)
            then __.current <- element; true
            else loop()
          else
            __.Dispose()
            false
        loop ()
      else
        __.Dispose()
        false

    member __.ToArray() =
      let set = HashSet<'U>(DefaultInternalSetCapacity, comparer)
      let xs = ResizeArray<'T>(DefaultInternalSetCapacity)
      for v in source do 
        if set.Add(selector v) then xs.Add v
      xs.ToArray()
    member __.ToList() =
      let set = HashSet<'U>(DefaultInternalSetCapacity, comparer)
      let xs = ResizeArray<'T>(DefaultInternalSetCapacity)
      for v in source do 
        if set.Add(selector v) then xs.Add v
      xs
    member __.GetCount(onlyIfCheap: bool) = 
      let xs = ResizeArray<'U>(DefaultInternalSetCapacity)
      for v in source do xs.Add (selector v)
      if onlyIfCheap then -1 else HashSet<'U>(xs, comparer).Count

    interface IListProvider<'T> with
      member __.ToArray() = __.ToArray()
      member __.ToList() = __.ToList()
      member __.GetCount(onlyIfCheap: bool) = __.GetCount(onlyIfCheap)

  [<Sealed>]
  type DistinctByListIterator<'T, 'U> (source: list<'T>, [<InlineIfLambda>]selector: 'T -> 'U, comparer: IEqualityComparer<'U>) =
    inherit Iterator<'T>()
    let mutable source' = source
    let mutable set = defaultof<HashSet<'U>>

    override __.Clone() = new DistinctByIterator<'T, 'U> (source, selector, comparer)

    override __.Dispose() =
      set <- defaultof<HashSet<'U>>
      base.Dispose()

    override __.MoveNext() : bool =
      if __.state = 1 then
        match source' with
        | h::tail ->
          set <- HashSet<'U>(DefaultInternalSetCapacity, comparer)
          set.Add(selector h) |> ignore
          __.current <- h
          source' <- tail
          __.state <- 2
          true
        | _ -> __.Dispose(); false
      else if __.state = 2 then
        let rec loop(xs: list<'T>) =
          match xs with
          | h::tail ->
            if set.Add (selector h)
            then 
              __.current <- h
              source' <- tail
              true
            else
              loop(tail)
          | _ ->
            __.Dispose()
            false
        loop (source')
      else
        __.Dispose()
        false

    member __.ToArray() =
      let set = HashSet<'U>(DefaultInternalSetCapacity, comparer)
      let xs = ResizeArray<'T>(DefaultInternalSetCapacity)
      for v in source do 
        if set.Add(selector v) then xs.Add v
      xs.ToArray()
    member __.ToList() =
      let set = HashSet<'U>(DefaultInternalSetCapacity, comparer)
      let xs = ResizeArray<'T>(DefaultInternalSetCapacity)
      for v in source do 
        if set.Add(selector v) then xs.Add v
      xs
    member __.GetCount(onlyIfCheap: bool) = 
      let xs = ResizeArray<'U>(DefaultInternalSetCapacity)
      for v in source do xs.Add (selector v)
      if onlyIfCheap then -1 else HashSet<'U>(xs, comparer).Count

    interface IListProvider<'T> with
      member __.ToArray() = __.ToArray()
      member __.ToList() = __.ToList()
      member __.GetCount(onlyIfCheap: bool) = __.GetCount(onlyIfCheap)

  [<Sealed>]
  type DistinctByArrayIterator<'T, 'U> (source: array<'T>, [<InlineIfLambda>]selector: 'T -> 'U, comparer: IEqualityComparer<'U>) =
    inherit Iterator<'T>()
    let mutable set = defaultof<HashSet<'U>>
    let mutable index = 0

    override __.Clone() = new DistinctByIterator<'T, 'U> (source, selector, comparer)

    override __.Dispose() =
      set <- defaultof<HashSet<'U>>
      base.Dispose()

    override __.MoveNext() : bool =
      if __.state = 1 then
        if source.Length = 0
        then __.Dispose(); false
        else
          let element = source[index]
          set <- HashSet<'U>(DefaultInternalSetCapacity, comparer)
          set.Add(selector element) |> ignore
          __.current <- element
          __.state <- 2
          index <- index + 1
          true
      else if __.state = 2 then
        let rec loop() =
          if index < source.Length
          then
            let element = source[index]
            index <- index + 1;
            if set.Add(selector element)
            then __.current <- element; true
            else loop()
          else 
            __.Dispose()
            false
        loop ()
      else
        __.Dispose()
        false

    member __.ToArray() =
      let set = HashSet<'U>(DefaultInternalSetCapacity, comparer)
      let xs = ResizeArray<'T>(DefaultInternalSetCapacity)
      for v in source do 
        if set.Add(selector v) then xs.Add v
      xs.ToArray()
    member __.ToList() =
      let set = HashSet<'U>(DefaultInternalSetCapacity, comparer)
      let xs = ResizeArray<'T>(DefaultInternalSetCapacity)
      for v in source do 
        if set.Add(selector v) then xs.Add v
      xs
    member __.GetCount(onlyIfCheap: bool) = 
      let xs = ResizeArray<'U>(DefaultInternalSetCapacity)
      for v in source do xs.Add (selector v)
      if onlyIfCheap then -1 else HashSet<'U>(xs, comparer).Count

    interface IListProvider<'T> with
      member __.ToArray() = __.ToArray()
      member __.ToList() = __.ToList()
      member __.GetCount(onlyIfCheap: bool) = __.GetCount(onlyIfCheap)

  [<Sealed>]
  type DistinctByResizeArrayIterator<'T, 'U> (source: ResizeArray<'T>, [<InlineIfLambda>]selector: 'T -> 'U, comparer: IEqualityComparer<'U>) =
    inherit Iterator<'T>()
    let mutable set = defaultof<HashSet<'U>>
    let mutable index = 0

    override __.Clone() = new DistinctByIterator<'T, 'U> (source, selector, comparer)

    override __.Dispose() =
      set <- defaultof<HashSet<'U>>
      base.Dispose()

    override __.MoveNext() : bool =
      if __.state = 1 then
        if source.Count = 0
        then __.Dispose(); false
        else
          let element = source[index]
          set <- HashSet<'U>(DefaultInternalSetCapacity, comparer)
          set.Add(selector element) |> ignore
          __.current <- element
          __.state <- 2
          index <- index + 1
          true
      else if __.state = 2 then
        let rec loop() =
          if index < source.Count
          then
            let element = source[index]
            index <- index + 1;
            if set.Add(selector element)
            then __.current <- element; true
            else loop()
          else 
            __.Dispose()
            false
        loop ()
      else
        __.Dispose()
        false

    member __.ToArray() =
      let set = HashSet<'U>(DefaultInternalSetCapacity, comparer)
      let xs = ResizeArray<'T>(DefaultInternalSetCapacity)
      for v in source do 
        if set.Add(selector v) then xs.Add v
      xs.ToArray()
    member __.ToList() =
      let set = HashSet<'U>(DefaultInternalSetCapacity, comparer)
      let xs = ResizeArray<'T>(DefaultInternalSetCapacity)
      for v in source do 
        if set.Add(selector v) then xs.Add v
      xs
    member __.GetCount(onlyIfCheap: bool) = 
      let xs = ResizeArray<'U>(DefaultInternalSetCapacity)
      for v in source do xs.Add (selector v)
      if onlyIfCheap then -1 else HashSet<'U>(xs, comparer).Count

    interface IListProvider<'T> with
      member __.ToArray() = __.ToArray()
      member __.ToList() = __.ToList()
      member __.GetCount(onlyIfCheap: bool) = __.GetCount(onlyIfCheap)