namespace Funtom.linq.core

open System
open System.Collections
open System.Collections.Generic
open Funtom.linq

module rec Select =
  let inline combine_predicates ([<InlineIfLambda>] p1: 'source -> bool) ([<InlineIfLambda>] p2: 'source -> bool) (x: 'source) = p1 x && p2 x
  let inline combine_selectors ([<InlineIfLambda>] lhs: 'T -> 'U) ([<InlineIfLambda>] rhs: 'U -> 'V) = lhs >> rhs
  let inline get_tid () = Environment.CurrentManagedThreadId

  /// <summary>
  /// 
  /// </summary>
  [<NoComparison;NoEquality;>]
  type SelectEnumerator<'T, 'U> (iter: IEnumerator<'T>, [<InlineIfLambda>] selector: 'T -> 'U) =
    let mutable current : 'U = defaultof<'U>
    let dispose () = ()
    let rec move_next () =
      if iter.MoveNext()
        then current <- selector iter.Current; true
        else false
    let current (): 'U = current
    let reset () = ()

    member __.Dispose() = dispose ()
    member __.MoveNext() = move_next ()
    member __.Current with get(): 'U = current ()
    member __.Reset() = reset ()
    
    interface IDisposable with member __.Dispose () = dispose ()
    interface IEnumerator with
      member __.MoveNext () = move_next ()
      member __.Current with get() = current ()
      member __.Reset () = reset ()
    interface IEnumerator<'U> with member __.Current with get() = current ()

  /// <summary>
  /// 
  /// </summary>
  [<NoComparison;NoEquality;>]
  type SelectIterator<'T, 'U> (src: seq<'T>, [<InlineIfLambda>] selector: 'T -> 'U) =
    let get_enumerator () = new SelectEnumerator<'T, 'U> (src.GetEnumerator(), selector)
    interface IEnumerable with member __.GetEnumerator () = get_enumerator ()
    interface IEnumerable<'U> with member __.GetEnumerator () = get_enumerator ()
    member __.select<'V> (selector': 'U -> 'V) = SelectIterator<'T, 'V>(src, (combine_selectors selector selector'))

  /// <summary>
  /// 
  /// </summary>
  [<NoComparison;NoEquality;>]
  type SelectArrayEnumerator<'T, 'U> (src: array<'T>, [<InlineIfLambda>] selector: 'T -> 'U) =
    let mutable current : 'U = defaultof<'U>
    let mutable i : int = 0
    let dispose () = ()
    let move_next () =
      if i < src.Length
        then current <- selector src[i]; i <- i + 1; true
        else dispose (); false
    let current (): 'U = current
    let reset () = ()

    member __.Dispose() = dispose ()
    member __.MoveNext() = move_next ()
    member __.Current with get() : 'U = current ()
    member __.Reset() = reset ()
    
    interface IDisposable with member __.Dispose () = dispose ()
    interface IEnumerator with
      member __.MoveNext () = move_next ()
      member __.Current with get() = current ()
      member __.Reset () = reset ()
    interface IEnumerator<'U> with member __.Current with get() = current ()

  /// <summary>
  /// 
  /// </summary>
  [<NoComparison;NoEquality;>]
  type SelectArrayIterator<'T, 'U> (src: array<'T>, [<InlineIfLambda>] selector: 'T -> 'U) =
    let get_enumerator () = new SelectArrayEnumerator<'T, 'U> (src, selector)
    interface IEnumerable with member __.GetEnumerator () = get_enumerator ()
    interface IEnumerable<'U> with member __.GetEnumerator () = get_enumerator ()
    member __.select<'V> (selector': 'U -> 'V) = SelectArrayIterator<'T, 'V>(src, (combine_selectors selector selector'))

  /// <summary>
  /// 
  /// </summary>
  module SelectListIterator =
    let inline create ([<InlineIfLambda>]selector: 'T -> 'R) (src: ResizeArray<'T>) = 
      { 
        SelectListIterator.src = src
        selector = selector
        tid = get_tid()
        current = defaultof<'R>
        idx = 0
      }
    let inline get_enumerator (iter: SelectListIterator<'source, 'result>) = 
      if iter.tid = get_tid()
        then iter
        else { iter with tid = get_tid(); current = defaultof<'result> }
    let inline move_next (iter: SelectListIterator<'source, 'result>) =
      if uint iter.idx < uint iter.src.Count
        then
          iter.current <- iter.selector iter.src[iter.idx]
          iter.idx <- iter.idx + 1
          true
        else
          false

  /// <summary>
  /// 
  /// </summary>
  [<NoComparison;NoEquality>]
  type SelectListIterator<'src, 'res> =
    {
      src: ResizeArray<'src>
      selector: 'src -> 'res
      tid : int
      mutable current : 'res
      mutable idx : int
    }
    interface IDisposable with member __.Dispose () = ()
    interface IEnumerator with
      member __.MoveNext () : bool = SelectListIterator.move_next __
      member __.Current with get() = __.current :> obj
      member __.Reset () = raise(NotSupportedException "not supported")
    interface IEnumerator<'res> with member __.Current with get() = __.current 
    interface IEnumerable with member __.GetEnumerator () = SelectListIterator.get_enumerator __
    interface IEnumerable<'res> with member __.GetEnumerator () = SelectListIterator.get_enumerator __

  /// <summary>
  /// 
  /// </summary>
  module SelectFsListIterator =
    let inline create ([<InlineIfLambda>]selector: 'T -> 'R) (source: list<'T>) = 
      { 
        SelectFsListIterator.selector = selector
        thread_id = Environment.CurrentManagedThreadId
        current = Unchecked.defaultof<'R>
        cache = source
      }
    let inline get_enumerator (iter: SelectFsListIterator<'source, 'result>) = 
      if iter.thread_id = Environment.CurrentManagedThreadId then iter
      else { iter with thread_id = Environment.CurrentManagedThreadId; current = Unchecked.defaultof<'result> }
    let inline move_next (iter: SelectFsListIterator<'source, 'result>) =
      match iter.cache with
      | h::tail ->
        iter.current <- iter.selector h
        iter.cache <- tail
        true
      | _ ->
        false

  /// <summary>
  /// 
  /// </summary>
  [<NoComparison;NoEquality>]
  type SelectFsListIterator<'T, 'R> =
    {
      selector: 'T -> 'R
      thread_id : int
      mutable current : 'R
      mutable cache : list<'T>
    }
    interface IDisposable with member __.Dispose () = ()
    interface IEnumerator with
      member __.MoveNext () : bool = SelectFsListIterator.move_next __
      member __.Current with get() = __.current :> obj
      member __.Reset () = raise(NotSupportedException "not supported")
    interface IEnumerator<'R> with member __.Current with get() = __.current 
    interface IEnumerable with member __.GetEnumerator () = SelectFsListIterator.get_enumerator __
    interface IEnumerable<'R> with member __.GetEnumerator () = SelectFsListIterator.get_enumerator __