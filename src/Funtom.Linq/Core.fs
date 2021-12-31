namespace Funtom.Linq

open System.Collections
open System.Collections.Generic
open System
open System.Linq
open System.Diagnostics
open System.Runtime.CompilerServices
open System.Runtime.InteropServices


module rec Core =
  let inline combine_predicates ([<InlineIfLambda>] p1: 'source -> bool) ([<InlineIfLambda>] p2: 'source -> bool) (x: 'source) = p1 x && p2 x
  
  /// <summary>
  /// 
  /// </summary>
  [<NoComparison;NoEquality;>]
  type WhereEnumerator<'T>(enumerator: IEnumerator<'T>, [<InlineIfLambda>] predicate: 'T -> bool)=
    let mutable current : 'T = Unchecked.defaultof<'T>
    let dispose () = enumerator.Dispose()
    let rec move_next () = 
      if enumerator.MoveNext() then
        let item = enumerator.Current
        if predicate item then
          current <- item
          true
        else
          move_next ()
      else
        dispose ()
        false
    let current (): 'T = current
    let reset () = enumerator.Reset()

    member __.Dispose() = dispose ()
    member __.MoveNext() = move_next ()
    member __.Current with get() : 'T = current ()
    member __.Reset() = reset ()
    
    interface IDisposable with member __.Dispose () = dispose ()
    interface IEnumerator with
         member __.MoveNext () = move_next ()
         member __.Current with get() = current ()
         member __.Reset () = reset ()
    interface IEnumerator<'T> with member __.Current with get() = current ()
  
  /// <summary>
  /// 
  /// </summary>
  [<NoComparison;NoEquality;>]
  type WhereIterator<'T> (source: seq<'T>, [<InlineIfLambda>] predicate: 'T -> bool) =
    let get_enumerator () = new WhereEnumerator<'T> (source.GetEnumerator(), predicate)
    interface IEnumerable with member __.GetEnumerator () = get_enumerator ()
    interface IEnumerable<'T> with member __.GetEnumerator () = get_enumerator ()
    member __.where (predicate': 'T -> bool) = WhereIterator<'T>(source, (combine_predicates predicate predicate'))
  
  /// <summary>
  /// 
  /// </summary>
  [<NoComparison;NoEquality;>]
  type WhereArrayEnumerator<'T> (source: array<'T>, [<InlineIfLambda>] predicate: 'T -> bool) =
    let mutable current : 'T = Unchecked.defaultof<'T>
    let mutable index : int = 0
    let dispose () = ()
    let rec move_next () =
      if index < source.Length then
        let item = source[index]
        index <- index + 1
        if predicate item then current <- item; true
        else move_next ()
      else
        dispose ()
        false
    let current (): 'T = current
    let reset () = ()

    member __.Dispose() = dispose ()
    member __.MoveNext() = move_next ()
    member __.Current with get() : 'T = current ()
    member __.Reset() = reset ()
    
    interface IDisposable with member __.Dispose () = dispose ()
    interface IEnumerator with
         member __.MoveNext () = move_next ()
         member __.Current with get() = current ()
         member __.Reset () = reset ()
    interface IEnumerator<'T> with member __.Current with get() = current ()

  /// <summary>
  /// 
  /// </summary>
  [<NoComparison;NoEquality;>]
  type WhereArrayIterator<'T> (source: array<'T>, [<InlineIfLambda>] predicate: 'T -> bool) =
    let get_enumerator () = new WhereArrayEnumerator<'T> (source, predicate)
    interface IEnumerable with member __.GetEnumerator () = get_enumerator ()
    interface IEnumerable<'T> with member __.GetEnumerator () = get_enumerator ()
    member __.where (predicate': 'T -> bool) = WhereArrayIterator<'T>(source, (combine_predicates predicate predicate'))

  /// <summary>
  /// 
  /// </summary>
  [<NoComparison;NoEquality;>]
  type WhereResizeArrayEnumerator<'T> (source: ResizeArray<'T>, [<InlineIfLambda>] predicate: 'T -> bool) =
    let mutable current : 'T = Unchecked.defaultof<'T>
    let mutable index : int = 0
    let dispose () = ()
    let rec move_next () =
      if index < source.Count then
        let item = source[index]
        index <- index + 1
        if predicate item then current <- item; true
        else move_next ()
      else
        dispose ()
        false
    let current (): 'T = current
    let reset () = ()

    member __.Dispose() = dispose ()
    member __.MoveNext() = move_next ()
    member __.Current with get() : 'T = current ()
    member __.Reset() = reset ()
    
    interface IDisposable with member __.Dispose () = dispose ()
    interface IEnumerator with
         member __.MoveNext () = move_next ()
         member __.Current with get() = current ()
         member __.Reset () = reset ()
    interface IEnumerator<'T> with member __.Current with get() = current ()

  /// <summary>
  /// 
  /// </summary>
  [<NoComparison;NoEquality;>]
  type WhereResizeArrayIterator<'T> (source: ResizeArray<'T>, [<InlineIfLambda>] predicate: 'T -> bool) =
    let get_enumerator () = new WhereResizeArrayEnumerator<'T> (source, predicate)
    interface IEnumerable with member __.GetEnumerator () = get_enumerator ()
    interface IEnumerable<'T> with member __.GetEnumerator () = get_enumerator ()
    member __.where (predicate': 'T -> bool) = WhereResizeArrayIterator<'T>(source, (combine_predicates predicate predicate'))
  
  /// <summary>
  /// 
  /// </summary>
  [<NoComparison;NoEquality;>]
  type WhereListEnumerator<'T> (source: list<'T>, [<InlineIfLambda>] predicate: 'T -> bool) =
    let mutable current : 'T = Unchecked.defaultof<'T>
    let mutable cache : list<'T> = source
    let dispose () = ()
    let rec move_next (src: list<'T>) =
      match src with
      | h::tail ->
        if predicate h then
          current <- h
          cache <- tail
          true
        else
          move_next(tail)
      | _ ->
        dispose()
        false
    let current (): 'T = current
    let reset () = ()

    member __.Dispose() = dispose ()
    member __.MoveNext() = move_next cache
    member __.Current with get() : 'T = current ()
    member __.Reset() = reset ()
    
    interface IDisposable with member __.Dispose () = dispose ()
    interface IEnumerator with
         member __.MoveNext () = move_next cache
         member __.Current with get() = current ()
         member __.Reset () = reset ()
    interface IEnumerator<'T> with member __.Current with get() = current ()

  /// <summary>
  /// 
  /// </summary>
  [<NoComparison;NoEquality;>]
  type WhereListIterator<'T> (source: list<'T>, [<InlineIfLambda>] predicate: 'T -> bool) =
    let get_enumerator () = new WhereListEnumerator<'T> (source, predicate)
    interface IEnumerable with member __.GetEnumerator () = get_enumerator ()
    interface IEnumerable<'T> with member __.GetEnumerator () = get_enumerator ()
    member __.where (predicate': 'T -> bool) = WhereListIterator<'T>(source, (combine_predicates predicate predicate'))

  /// <summary>
  /// 
  /// </summary>
  module SelectEnumerableIterator =
    let inline create ([<InlineIfLambda>]selector: 'T -> 'R) (source: seq<'T>) = 
      { 
        SelectEnumerableIterator.source = source
        selector = selector
        thread_id = Environment.CurrentManagedThreadId
        enumerator = Unchecked.defaultof<IEnumerator<'T>>
        current = Unchecked.defaultof<'R>
        state = -1
      }
    let inline dispose (iter: SelectEnumerableIterator<'T, 'R>) = 
      if iter.enumerator <> Unchecked.defaultof<IEnumerator<'T>> then
        iter.enumerator.Dispose()
        iter.enumerator <- Unchecked.defaultof<IEnumerator<'T>>
        iter.current <- Unchecked.defaultof<'R>
        iter.state <- -1

    let inline get_enumerator (iter: SelectEnumerableIterator<'T, 'R>) =
      if iter.state = -1 && iter.thread_id = Environment.CurrentManagedThreadId then
        iter.enumerator <- iter.source.GetEnumerator()
        iter.state <- 0
        iter
      else
        { iter with thread_id = Environment.CurrentManagedThreadId; state = -1; current = Unchecked.defaultof<'R> }

    let inline move_next (iter: SelectEnumerableIterator<'T, 'R>) =
      if iter.enumerator.MoveNext() then
        iter.current <- iter.selector iter.enumerator.Current
        true
      else
        dispose iter
        false
  
  /// <summary>
  /// 
  /// </summary>
  [<NoComparison;NoEquality>]
  type SelectEnumerableIterator<'T, 'R> =
    {
      source: seq<'T>
      selector: 'T -> 'R
      thread_id : int 
      mutable enumerator : IEnumerator<'T>
      mutable current : 'R
      mutable state : int
    }
    interface IDisposable with member __.Dispose () = SelectEnumerableIterator.dispose __
    interface IEnumerator with 
      member __.MoveNext () : bool = SelectEnumerableIterator.move_next __
      member __.Current with get() = __.current :> obj
      member __.Reset () = raise(NotSupportedException "not supported")
    interface IEnumerator<'R> with member __.Current with get() = __.current
    interface IEnumerable with member __.GetEnumerator () = SelectEnumerableIterator.get_enumerator __
    interface IEnumerable<'R> with member __.GetEnumerator () = SelectEnumerableIterator.get_enumerator __

  /// <summary>
  /// 
  /// </summary>
  module SelectArrayIterator =
    let inline create ([<InlineIfLambda>]selector: 'T -> 'R) (source: array<'T>) = 
      { 
        SelectArrayIterator.source = source
        selector = selector
        thread_id = Environment.CurrentManagedThreadId
        state = -1
      }
    let inline dispose (iter: SelectArrayIterator<'source, 'result>) = iter.state <- -2
    let inline get_enumerator (iter: SelectArrayIterator<'source, 'result>) = 
      if iter.state = -2 && iter.thread_id = Environment.CurrentManagedThreadId then
        iter.state <- -1
        iter
      else
        { iter with state = -1; thread_id = Environment.CurrentManagedThreadId }
    let inline move_next (iter: SelectArrayIterator<'source, 'result>) =
      if iter.state < -1 || iter.state = iter.source.Length - 1 then
        dispose iter
        false
      else
        iter.state <- iter.state + 1
        true
        
  /// <summary>
  /// 
  /// </summary>
  [<NoComparison;NoEquality>]
  type SelectArrayIterator<'source, 'result> =
    {
      source: array<'source>
      selector: 'source -> 'result
      thread_id : int
      mutable state : int
    }
    interface IDisposable with member __.Dispose () = SelectArrayIterator.dispose __
    interface IEnumerator with
      member __.MoveNext () : bool = SelectArrayIterator.move_next __
      member __.Current with get() = (__.selector __.source[__.state]) :> obj
      member __.Reset () = raise(NotSupportedException "not supported")
    interface IEnumerator<'result> with member __.Current with get() = __.selector __.source[__.state]
    interface IEnumerable with member __.GetEnumerator () = SelectArrayIterator.get_enumerator __
    interface IEnumerable<'result> with member __.GetEnumerator () = SelectArrayIterator.get_enumerator __
  
  /// <summary>
  /// 
  /// </summary>
  module SelectListIterator =
    let inline create ([<InlineIfLambda>]selector: 'T -> 'R) (source: ResizeArray<'T>) = 
      { 
        SelectListIterator.source = source
        selector = selector
        thread_id = Environment.CurrentManagedThreadId
        current = Unchecked.defaultof<'R>
        index = 0
      }
    let inline get_enumerator (iter: SelectListIterator<'source, 'result>) = 
      if iter.thread_id = Environment.CurrentManagedThreadId then iter
      else { iter with thread_id = Environment.CurrentManagedThreadId; current = Unchecked.defaultof<'result> }
    let inline move_next (iter: SelectListIterator<'source, 'result>) =
      if uint iter.index < uint iter.source.Count then
        iter.current <- iter.selector iter.source[iter.index]
        iter.index <- iter.index + 1
        true
      else
        false

  /// <summary>
  /// 
  /// </summary>
  [<NoComparison;NoEquality>]
  type SelectListIterator<'source, 'result> =
    {
      source: ResizeArray<'source>
      selector: 'source -> 'result
      thread_id : int
      mutable current : 'result
      mutable index : int
    }
    interface IDisposable with member __.Dispose () = ()
    interface IEnumerator with
      member __.MoveNext () : bool = SelectListIterator.move_next __
      member __.Current with get() = __.current :> obj
      member __.Reset () = raise(NotSupportedException "not supported")
    interface IEnumerator<'result> with member __.Current with get() = __.current 
    interface IEnumerable with member __.GetEnumerator () = SelectListIterator.get_enumerator __
    interface IEnumerable<'result> with member __.GetEnumerator () = SelectListIterator.get_enumerator __

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
