namespace Funtom.linq

open System.Collections
open System.Collections.Generic
open System
open System.Diagnostics
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open Funtom.linq.Iterator
open Basis

module rec Core =
  let inline combine_predicates ([<InlineIfLambda>] p1: 'source -> bool) ([<InlineIfLambda>] p2: 'source -> bool) (x: 'source) = p1 x && p2 x
  let inline combine_selectors ([<InlineIfLambda>] lhs: 'T -> 'U) ([<InlineIfLambda>] rhs: 'U -> 'V) = lhs >> rhs
  
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
  [<NoComparison;NoEquality;>]
  type SelectEnumerator<'T, 'U> (iterator: IEnumerator<'T>, [<InlineIfLambda>] selector: 'T -> 'U) =
    let mutable current : 'U = Unchecked.defaultof<'U>
    let dispose () = ()
    let rec move_next () =
      if iterator.MoveNext() then
        current <- selector iterator.Current
        true
      else
        false
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
  type SelectIterator<'T, 'U> (source: seq<'T>, [<InlineIfLambda>] selector: 'T -> 'U) =
    let get_enumerator () = new SelectEnumerator<'T, 'U> (source.GetEnumerator(), selector)
    interface IEnumerable with member __.GetEnumerator () = get_enumerator ()
    interface IEnumerable<'U> with member __.GetEnumerator () = get_enumerator ()
    member __.select<'V> (selector': 'U -> 'V) = SelectIterator<'T, 'V>(source, (combine_selectors selector selector'))

  /// <summary>
  /// 
  /// </summary>
  [<NoComparison;NoEquality;>]
  type SelectArrayEnumerator<'T, 'U> (source: array<'T>, [<InlineIfLambda>] selector: 'T -> 'U) =
    let mutable current : 'U = Unchecked.defaultof<'U>
    let mutable index : int = 0
    let dispose () = ()
    let move_next () =
      if index < source.Length then
        current <- selector source[index]
        index <- index + 1
        true
      else
        dispose ()
        false
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
  type SelectArrayIterator<'T, 'U> (source: array<'T>, [<InlineIfLambda>] selector: 'T -> 'U) =
    let get_enumerator () = new SelectArrayEnumerator<'T, 'U> (source, selector)
    interface IEnumerable with member __.GetEnumerator () = get_enumerator ()
    interface IEnumerable<'U> with member __.GetEnumerator () = get_enumerator ()
    member __.select<'V> (selector': 'U -> 'V) = SelectArrayIterator<'T, 'V>(source, (combine_selectors selector selector'))

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
    
  /// <summary>
  /// 
  /// </summary>
  [<NoComparison;NoEquality>]
  type OfTypeEnumerator<'T> (iter: IEnumerator) =
    let mutable current: 'T = Unchecked.defaultof<'T>
    let dispose () = ()
    let rec move_next () =
      if iter.MoveNext() then
        let mutable c = iter.Current
        match c with
        | :? 'T -> current <- Unsafe.As<obj, 'T>(&c); true
        | _ -> move_next () 
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
  [<NoComparison;NoEquality>]
  type OfTypeIterator<'T> (source: IEnumerable) =
    let get_enumerator () = new OfTypeEnumerator<'T> (source.GetEnumerator())
    interface IEnumerable with member __.GetEnumerator () = get_enumerator ()
    interface IEnumerable<'T> with member __.GetEnumerator () = get_enumerator ()
    
  /// <summary>
  /// 
  /// </summary>
  [<NoComparison;NoEquality>]
  type CastEnumerator<'T> (iter: IEnumerator) =
    let mutable current: 'T = Unchecked.defaultof<'T>
    let dispose () = ()
    let move_next () =
      if iter.MoveNext() then
        let mutable c = iter.Current
        match c with
        | :? 'T -> current <- Unsafe.As<obj, 'T>(&c); true
        | _ -> raise (InvalidCastException $"Unable to cast object of type '%s{c.GetType().FullName}' to type '%s{typeof<'T>.FullName}'.")
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
  [<NoComparison;NoEquality>]
  type CastIterator<'T> (source: IEnumerable) =
    let get_enumerator () = new CastEnumerator<'T> (source.GetEnumerator())
    interface IEnumerable with member __.GetEnumerator () = get_enumerator ()
    interface IEnumerable<'T> with member __.GetEnumerator () = get_enumerator ()