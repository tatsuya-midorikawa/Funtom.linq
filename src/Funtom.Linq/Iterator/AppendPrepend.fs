namespace Funtom.Linq.Iterator

open System
open System.Collections
open System.Collections.Generic
open Basis

module AppendPrepend =
  // src: https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/AppendPrepend.cs#L39
  // src: https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/AppendPrepend.SpeedOpt.cs#L11
  [<AbstractClass>]
  type AppendPrependIteratpr<'T> (source: seq<'T>) =
    inherit Iterator<'T> ()
    member val internal source = source
    member val internal enumerator = Unchecked.defaultof<IEnumerator<'T>> with get, set

    abstract member Append : 'T -> AppendPrependIteratpr<'T>
    abstract member Prepend : 'T -> AppendPrependIteratpr<'T>
    abstract member ToArray : unit -> array<'T>
    abstract member ToList : unit -> ResizeArray<'T>
    abstract member GetCount : bool -> int

    member internal __.GetSourceEnumerator() = __.enumerator <- __.source.GetEnumerator()
    member internal __.LoadFromEnumerator() =
      if __.enumerator.MoveNext() then
        __.current <- __.enumerator.Current
        true
      else
        __.Dispose()
        false
    override __.Dispose() = 
      if __.enumerator <> Unchecked.defaultof<IEnumerator<'T>> then
        __.enumerator.Dispose()
        __.enumerator <- Unchecked.defaultof<IEnumerator<'T>>
      base.Dispose()

