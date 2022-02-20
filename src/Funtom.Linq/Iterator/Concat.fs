namespace Funtom.Linq.Iterator

open Funtom.Linq
open System.Collections
open System.Collections.Generic

module Concat =
  open Basis
  [<AbstractClass>]
  type ConcatIterator<'T> () =
    inherit Iterator<'T>()
    let mutable enumerator = defaultof<IEnumerator<'T>>
    
    abstract member GetEnumerable : int -> seq<'T>
    abstract member Concat : seq<'T> -> ConcatIterator<'T>
    abstract member GetCount : bool -> int
    abstract member ToArray : unit -> 'T[]

    override __.Dispose() =
      if enumerator |> isNotDefault then
        enumerator.Dispose()
        enumerator <- defaultof<IEnumerator<'T>>
    
    override __.MoveNext() : bool =
      if __.state = 1 then
        enumerator <- __.GetEnumerable(0).GetEnumerator()
        __.state <- 2

      if 1 < __.state then
        let mutable break' = false
        let mutable result = false
        while not break' do
          if enumerator.MoveNext() then
            __.current <- enumerator.Current
            result <- true
            break' <- true

          let next = __.GetEnumerable(__.state - 1)
          __.state <- __.state + 1
          if next |> isNotDefault then
            enumerator.Dispose()
            enumerator <- next.GetEnumerator()
          else
            __.Dispose()
            result <- false
            break' <- true

        result
      else
        false

    member __.ToList() =
      let count = __.GetCount(true)
      let list = if count <> -1 then ResizeArray<'T>(count) else ResizeArray()
      let rec fn (i: int) =
        match __.GetEnumerable(i) with
        | null -> ()
        | xs -> list.AddRange(xs); fn(i + 1)
      fn(0)
      list