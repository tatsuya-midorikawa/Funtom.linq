namespace Funtom.linq.core

open System
open System.Collections
open System.Collections.Generic

type IIterator<'T> =
  inherit IEnumerable
  inherit IEnumerable<'T>
  abstract member Select<'U> : ('T -> 'U) -> seq<'U>
  //// TODO: implement default
  //abstract member Where: ('T -> bool) -> seq<'T>

[<AbstractClass>]
type Iterator<'T> () =
  abstract member GetEnumerator : unit -> IEnumerator<'T>
  abstract member Select<'U> : ('T -> 'U) -> seq<'U>
  interface IIterator<'T> with
    member __.Select<'U> (selector: 'T -> 'U) = __.Select(selector)
  interface IEnumerable with
    member __.GetEnumerator () = __.GetEnumerator ()
  interface IEnumerable<'T> with
    member __.GetEnumerator () = __.GetEnumerator ()