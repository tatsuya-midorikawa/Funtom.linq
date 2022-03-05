namespace Funtom.Linq.Iterator

open Funtom.Linq
open System.Collections
open System.Collections.Generic

module DefaultIfEmpty =
  open Basis
  open Interfaces
  open Enumerable

  // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/DefaultIfEmpty.cs#L24
  // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/DefaultIfEmpty.SpeedOpt.cs#L11
  [<Sealed>]
  type DefaultIfEmptyIterator<'T> (source: seq<'T>, defaultValue: 'T) =
    inherit Iterator<'T>()
    let mutable enumerator = defaultof<IEnumerator<'T>>

    override __.Clone() = new DefaultIfEmptyIterator<'T> (source, defaultValue)

    override __.Dispose() =
      if enumerator |> isNotDefault then
        enumerator.Dispose()
        enumerator <- defaultof<IEnumerator<'T>>
      base.Dispose()

    override __.MoveNext() : bool =
      if __.state = 1 then
        enumerator <- source.GetEnumerator()
        if enumerator.MoveNext()
        then
          __.current <- enumerator.Current
          __.state <- 2
        else
          __.current <- defaultValue
          __.state <- -1
        true
      else if __.state = 2 then
        if enumerator.MoveNext()
        then
          __.current <- defaultValue
          true
        else
          __.Dispose()
          false
      else
        __.Dispose()
        false

    member __.ToArray() =
      let array = source |> Enumerable.toArray
      if array.Length = 0 then [| defaultValue |] else array

    member __.ToList() =
      let list = source |> Enumerable.toList
      if list.Count = 0 then list.Add defaultValue
      list

    member __.GetCount(onlyIfCheap: bool) =
      let count =
        if not onlyIfCheap
        then source |> Enumerable.count
        else
          match source with
          | :? IReadOnlyCollection<'T> as collection -> collection.Count
          | :? ICollection<'T> as collection -> collection.Count
          | :? ICollection as collection -> collection.Count
          | :? IListProvider<'T> as provider -> provider.GetCount true
          | _ -> -1
      if count = 0 then 1 else count

    interface IListProvider<'T> with
      member __.ToArray() = __.ToArray()
      member __.ToList() = __.ToList()
      member __.GetCount(onlyIfCheap: bool) = __.GetCount(onlyIfCheap)