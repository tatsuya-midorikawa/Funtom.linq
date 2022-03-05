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