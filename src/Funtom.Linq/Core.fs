namespace Funtom.Linq

open System.Collections
open System.Collections.Generic
open System
open System.Diagnostics

module Core =
  let inline combine_predicates ([<InlineIfLambda>] p1: 'source -> bool) ([<InlineIfLambda>] p2: 'source -> bool) (x: 'source) = p1 x && p2 x

  type WhereEnumerableIterator<'source> (source: seq<'source>, [<InlineIfLambda>] predicate: 'source -> bool) as self =
    let mutable state : int = 0
    let mutable enumerator : option<IEnumerator<'source>> = None
    let mutable current : 'source = Unchecked.defaultof<'source>
    let thread_id : int = Environment.CurrentManagedThreadId
    
    let clone () = new WhereEnumerableIterator<'source>(source, predicate)

    let dispose () = 
      enumerator |> Option.iter (fun e -> 
        e.Dispose()
        current <- Unchecked.defaultof<'source>
        state <- -1)
      enumerator <- None

    let get_enumerator () =
      if state = 0 && thread_id = Environment.CurrentManagedThreadId then
        state <- 1
        self
      else
        let enumerator = clone()
        enumerator.State <- 1
        enumerator
      
    let rec move_next (e: IEnumerator<'source>) =
      Debug.WriteLine($"# frame: %d{StackTrace().FrameCount}")
      if e.MoveNext() then
        let v = e.Current
        if predicate v then
          current <- v
          true
        else
          move_next(e)
      else
        false

    interface IDisposable with
      member __.Dispose () = dispose ()

    interface IEnumerator with
      member __.MoveNext () : bool = 
        let inline inner_move_next () = 
          let result = enumerator |> Option.map move_next |> Option.defaultValue false
          if not result then
            dispose()
          result

        match state with
        | 1 -> 
          enumerator <- source.GetEnumerator() |> Some
          state <- 2
          inner_move_next ()
        | 2 ->
          inner_move_next ()
        | _ -> 
          false

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

