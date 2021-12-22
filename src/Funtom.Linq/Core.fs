namespace Funtom.Linq

open System.Collections
open System.Collections.Generic
open System

module Core =
  let inline combine_predicates ([<InlineIfLambda>] p1: 'source -> bool) ([<InlineIfLambda>] p2: 'source -> bool) (x: 'source) = p1 x && p2 x

  type WhereEnumerableIterator<'source> (source: seq<'source>, [<InlineIfLambda>] predicate: 'source -> bool) as self =
    let mutable state : int = 0
    let mutable enumerator = lazy(source.GetEnumerator())
    let mutable current : 'source = Unchecked.defaultof<'source>
    let thread_id : int = Environment.CurrentManagedThreadId
    
    let clone () = new WhereEnumerableIterator<'source>(source, predicate)

    let dispose () = 
      if enumerator <> null then
        enumerator.Value.Dispose()
        enumerator <- null
        current <- Unchecked.defaultof<'source>
        state <- -1

    let get_enumerator () =
      if state = 0 && thread_id = Environment.CurrentManagedThreadId then
        state <- 1
        self
      else
        let enumerator = clone()
        enumerator.State <- 1
        enumerator

    interface IDisposable with
      member __.Dispose () = dispose ()

    interface IEnumerator with
      member __.MoveNext () : bool = 
        state <- 2
        let mutable result = false
        while enumerator.Value.MoveNext() && not result do
          let item = enumerator.Value.Current
          current <- item
          result <- predicate item
        if not result then
          dispose ()
        result
      member __.Current with get() = current
      member __.Reset () = raise(NotSupportedException "not supported")

    interface IEnumerator<'source> with
      member __.Current with get() = current

    interface IEnumerable with
      member __.GetEnumerator () = get_enumerator ()

    interface IEnumerable<'source> with
      member __.GetEnumerator () = get_enumerator ()

    member __.where (predicate': 'source -> bool) =
      let p = combine_predicates predicate predicate'
      new WhereEnumerableIterator<'source>(source, p)

    member private __.State with get() = state and set v = state <- v


  let inline where ([<InlineIfLambda>] predicate: 'source -> bool) (source: seq<'source>) =
    match source with
    | :? WhereEnumerableIterator<'source> as iterator -> iterator.where predicate
    | _ -> new WhereEnumerableIterator<'source> (source, predicate)