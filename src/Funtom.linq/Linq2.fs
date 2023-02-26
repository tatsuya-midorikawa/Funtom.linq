namespace Funtom.linq

open System
open System.Collections.Generic
// open Funtom.linq.iterator
open Funtom.linq.core
open Funtom.linq.iterator.Empty
open Funtom.linq.Interfaces

module Linq2 =
  // src: https://github.com/dotnet/runtime/blob/release/6.0/src/libraries/System.Linq/src/System/Linq/Select.cs#L13
  let inline select<'T, 'U> ([<InlineIfLambda>] selector: 'T -> 'U) (source: seq< 'T>) : seq< 'U> =
    match source with
    | :? IReadOnlyList<'T> as list ->
      match list with
      | :? array<'T> as ary -> if ary.Length = 0 then Array.Empty<'U>() else new SelectArrayIterator<'T, 'U>(ary, selector)
      | :? list<'T> as list -> new SelectFsharpListIterator<'T, 'U>(list, selector)
      | :? ResizeArray<'T> as list -> new SelectResizeArrayIterator<'T, 'U>(list, selector)
      | _ -> new SelectIReadOnlyListIterator<'T, 'U>(list, selector)
    | :? IIterator<'T> as iter -> iter.Select selector
    | :? IPartition<'T> as partition ->
      match partition with
      | :? EmptyPartition<'T> as empty -> EmptyPartition<'U>.Instance
      | _ -> new SelectIPartitionIterator<'T, 'U>(partition, selector)
    | :? IList<'T> as ilist -> new SelectIListIterator<'T, 'U>(ilist, selector)
    | _ -> new SelectEnumerableIterator<'T, 'U>(source, selector)

  // TODO
  //let inline select<'T, 'U> ([<InlineIfLambda>]selector: 'T -> 'U) (src: seq<'T>): seq<'U> = src.Select selector
  let inline selecti ([<InlineIfLambda>]selector: ^T -> int -> ^Result) (src: seq< ^T>): seq< ^Result> =
    let mutable i = -1
    seq {
      for v in src do
        i <- i + 1
        yield (selector v i)
    }