namespace Funtom.Linq.Iterator

open System
open System.Collections
open System.Collections.Generic

module Basis =
  [<AbstractClass>]
  type Iterator<'T> () =
    let threadId = Environment.CurrentManagedThreadId
    member val internal state = 0 with get, set
    member val internal current = Unchecked.defaultof<'T> with get, set

    abstract member Clone : unit -> Iterator<'T>

    interface IDisposable with 
      member __.Dispose () =
        __.current <- Unchecked.defaultof<'T>
        __.state <- 0
    interface IEnumerator with
      member __.MoveNext() = NotImplementedException() |> raise
      member __.Current with get() = __.current
      member __.Reset () = NotSupportedException() |> raise
    interface IEnumerator<'T> with
      member __.Current with get() = __.current
    interface IEnumerable with
      member __.GetEnumerator () = (__ :> IEnumerable<'T>).GetEnumerator()
    interface IEnumerable<'T> with
      member __.GetEnumerator () =
        let mutable enumerator =
          if __.state = 0 && threadId = Environment.CurrentManagedThreadId then __
          else __.Clone()
        enumerator.state <- 1
        enumerator