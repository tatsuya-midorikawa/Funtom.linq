namespace Funtom.Linq.Iterator

open System
open System.Collections
open System.Collections.Generic
open Funtom.Linq

module Reverse =
  open Basis
  open Interfaces
  open Enumerable

  // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/Concat.cs#L190
  // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/Concat.SpeedOpt.cs#L195
  type ReverseIterator<'T> (src: seq<'T>) =
    inherit Iterator<'T>()
    let mutable buffer = defaultof<array<'T>>
    let mutable index = -2

    override __.Clone() = new ReverseIterator<'T> (src)

    override __.Dispose() =
      buffer <- defaultof<array<'T>>
      base.Dispose()
    
    override __.MoveNext() : bool =
      if index = -2 then
        buffer <- Enumerable.toArray src
        index <- buffer.Length - 1

      if 0 <= index then
        __.current <- buffer[index]
        index <- index - 1
        true
      else
        __.Dispose()
        false

    member __.ToArray() = 
      let xs = Enumerable.toArray src
      Array.Reverse xs
      xs
    member __.ToList() = 
      let xs = Enumerable.toList src
      xs.Reverse()
      xs
    member __.GetCount (onlyIfCheap: bool) =
      if onlyIfCheap
      then
        match src with
        | :? IListProvider<'T> as xs -> xs.GetCount(true)
        | :? IReadOnlyCollection<'T> as xs -> xs.Count
        | :? ICollection<'T> as xs -> xs.Count
        | :? ICollection as xs -> xs.Count
        | _ -> -1
      else
        Enumerable.count src
    interface IListProvider<'T> with
      member __.ToArray() = __.ToArray()
      member __.ToList() = __.ToList()
      member __.GetCount(onlyIfCheap: bool) = __.GetCount(onlyIfCheap)
  