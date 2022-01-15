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

    abstract member Dispose : unit -> unit
    default __.Dispose () = 
      __.current <- Unchecked.defaultof<'T>
      __.state <- 0
    
    abstract member MoveNext : unit -> bool
    abstract member Current : 'T with get
    default __.Current with get() = __.current
    abstract member Reset : unit -> unit
    default __.Reset () = NotSupportedException() |> raise

    abstract member GetEnumerator : unit -> IEnumerator<'T>
    default __.GetEnumerator () =
      let mutable enumerator =
        if __.state = 0 && threadId = Environment.CurrentManagedThreadId then __
        else __.Clone()
      enumerator.state <- 1
      enumerator

    abstract member Clone : unit -> Iterator<'T>

    interface IDisposable with
      member __.Dispose () = __.Dispose()
    interface IEnumerator with
      member __.MoveNext() = __.MoveNext()
      member __.Current with get() = __.Current
      member __.Reset () = __.Reset ()
    interface IEnumerator<'T> with
      member __.Current with get() = __.Current
    interface IEnumerable with
      member __.GetEnumerator () = __.GetEnumerator ()
    interface IEnumerable<'T> with
      member __.GetEnumerator () = __.GetEnumerator ()