namespace Funtom.linq

open System
open System.Collections
open System.Collections.Generic
// open System.Runtime.CompilerServices
// open System.Diagnostics
open Funtom.linq.Core
open Funtom.linq.Interfaces
open Funtom.linq.iterator
open Empty
open Basis
open Select
open AppendPrepend
open Chunk
open DefaultIfEmpty
open Distinct
open ElementAt

module Linq =
  // TODO: seq<'T> に対する ToArray() が非常に遅いので、要高速化
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.toarray?view=net-6.0
  let inline toArray (src: seq<'T>) = Enumerable.toArray src

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.todictionary?view=net-6.0
  let inline toDictionary ([<InlineIfLambda>]selector: ^T -> ^Key) (src: seq< ^T>) = 
    Enumerable.toDictionary (src, selector)
  let inline toDictionary'  ([<InlineIfLambda>]selector: ^T -> ^Key) (comparer: IEqualityComparer< ^Key>) (src: seq< ^T>) = 
    Enumerable.toDictionary' (src, selector, comparer)
  let inline toDictionary2 ([<InlineIfLambda>]keySelector: ^T -> ^Key) ([<InlineIfLambda>]elementSelector: ^T -> ^Element) (src: seq< ^T>) = 
    Enumerable.toDictionary2 (src, keySelector, elementSelector)
  let inline toDictionary2' ([<InlineIfLambda>]elementSelector: ^T -> ^Element) ([<InlineIfLambda>]keySelector: ^T -> ^Key) (comparer: IEqualityComparer< ^Key>) (src: seq< ^T>) =
    Enumerable.toDictionary2' (src, keySelector, elementSelector, comparer)
    
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.tohashset?view=net-6.0
  let inline toHashSet (src: seq< ^T>) = HashSet< ^T>(src, null)
  let inline toHashSet' (comparer: IEqualityComparer< ^T>) (src: seq< ^T>) = HashSet< ^T>(src, comparer)
  
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.tolist?view=net-6.0
  let inline toList (src: seq< ^T>) = Enumerable.toList src

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.aggregate?view=net-6.0#System_Linq_Enumerable_Aggregate__2_System_Collections_Generic_IEnumerable___0____1_System_Func___1___0___1__
  let inline aggregate (seed: ^Accumulate) ([<InlineIfLambda>]fx: ^Accumulate -> ^T -> ^Accumulate) (src: seq< ^T>) =
    match src with
    | :? list< ^T> as ls ->
      let rec fn(xs: list< ^T>) (seed': ^Accumulate) =
        match xs with
        | h::tail ->
          fn tail (fx seed' h)
        | _ -> seed'
      fn ls seed
    | :? array< ^T> as ary ->
      let rec fn(i: int) (seed': ^Accumulate)=
        if i < ary.Length then
          fn (i + 1) (fx seed' ary[i])
        else seed'
      fn 0 seed
    | :? ResizeArray< ^T> as ary ->
      let rec fn(i: int) (seed': ^Accumulate)=
        if i < ary.Count then
          fn (i + 1) (fx seed' ary[i])
        else seed'
      fn 0 seed
    | _ ->
      use iter = src.GetEnumerator()
      let rec fn (seed': ^Accumulate) =
        if iter.MoveNext() then fn (fx seed' iter.Current)
        else seed'
      fn seed
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.aggregate?view=net-6.0#System_Linq_Enumerable_Aggregate__3_System_Collections_Generic_IEnumerable___0____1_System_Func___1___0___1__System_Func___1___2__
  let inline aggregate' (seed: ^Accumulate) ([<InlineIfLambda>]fx: ^Accumulate -> ^T -> ^Accumulate) ([<InlineIfLambda>]resultSelector: ^Accumulate -> ^Result) (src: seq< ^T>) =
    src
    |> aggregate seed fx 
    |> resultSelector
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.aggregate?view=net-6.0#System_Linq_Enumerable_Aggregate__1_System_Collections_Generic_IEnumerable___0__System_Func___0___0___0__
  let inline aggregate'' ([<InlineIfLambda>]fx: ^T -> ^T -> ^T) (src: seq< ^T>) =
    use iter = src.GetEnumerator()
    if iter.MoveNext() then
      let rec fn (seed': ^T) =
        if iter.MoveNext() then fn (fx seed' iter.Current)
        else seed'
      fn iter.Current
    else
      raise (invalidOp "Sequence contains no elements.")

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.all?view=net-6.0
  // https://github.com/dotnet/corefx/blob/master/src/System.Linq/src/System/Linq/AnyAll.cs
  let inline all ([<InlineIfLambda>]predicate: ^T -> bool) (src: seq< ^T>) =    
    match src with
    | :? list< ^T> as ls ->
      let rec fn(xs: list< ^T>) =
        match xs with
        | h::tail -> if predicate h then fn tail else false
        | _ -> true
      fn ls
    | :? array< ^T> as ary ->
      let rec fn(i: int) =
        if i < ary.Length 
        then if predicate ary[i] then fn (i + 1) else false
        else true
      fn 0
    | :? ResizeArray< ^T> as ary ->
      let rec fn(i: int) =
        if i < ary.Count 
        then if predicate ary[i] then fn (i + 1) else false
        else true
      fn 0
    | _ ->
      use iter = src.GetEnumerator()
      let rec fn() =
        if iter.MoveNext()
        then if predicate iter.Current then fn() else false
        else true
      fn()
  
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.any?view=net-6.0#System_Linq_Enumerable_Any__1_System_Collections_Generic_IEnumerable___0__
  // https://github.com/dotnet/corefx/blob/master/src/System.Linq/src/System/Linq/AnyAll.cs
  let inline any (src: seq< ^T>) =
    match src with
    | :? ICollection< ^T> as xs -> xs.Count <> 0
    | :? IReadOnlyCollection< ^T> as xs -> xs.Count <> 0
    | _ -> 
      use iter = src.GetEnumerator()
      iter.MoveNext()
  
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.any?view=net-6.0#System_Linq_Enumerable_Any__1_System_Collections_Generic_IEnumerable___0__System_Func___0_System_Boolean__
  // https://github.com/dotnet/corefx/blob/master/src/System.Linq/src/System/Linq/AnyAll.cs
  let inline any' ([<InlineIfLambda>]predicate: ^T -> bool) (src: seq< ^T>) = 
    match src with
    | :? list< ^T> as ls ->
      let rec fn(xs: list< ^T>) =
        match xs with
        | h::tail -> if predicate h then true else fn tail
        | _ -> false
      fn ls
    | :? array< ^T> as ary ->
      let rec fn(i: int) =
        if i < ary.Length
        then if predicate ary[i] then true else fn (i + 1)
        else false
      fn 0
    | :? ResizeArray< ^T> as ary ->
      let rec fn(i: int) =
        if i < ary.Count
        then if predicate ary[i] then true else fn (i + 1)
        else false
      fn 0
    | _ ->
      use iter = src.GetEnumerator()
      let rec fn() =
        if iter.MoveNext()
        then if predicate iter.Current then true else fn()
        else false
      fn()

  // TODO: It is a very slow function compared to System.Linq,
  //       so performance tuning should be done.
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.append?view=net-6.0
  // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/AppendPrepend.cs#L11
  let inline append (element: ^T) (src: seq< ^T>) = 
    match src with
    | :? AppendPrependIterator< ^T> as appendable -> appendable.Append(element)
    | _ -> new AppendPrepend1Iterator< ^T>(src, element, true)

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.asenumerable?view=net-6.0
  let inline asEnumerable (src: seq< ^T>) = src
    
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.cast?view=net-6.0
  let inline cast (source: IEnumerable) : IEnumerable< ^T> = 
    match source with
    | :? IEnumerable< ^T> as src -> src
    | _ -> CastIterator< ^T> source

  // TODO: Performance tuning
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.chunk?view=net-6.0
  let inline chunk (size: int) (src: seq< ^T>) = chunkIterator(src, size)

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.concat?view=net-6.0
  let inline concat (snd: seq< ^T>) (fst: seq< ^T>) = 
    match fst with
    | :? Concat.ConcatIterator< ^T> as fst -> fst.Concat(snd)
    | _ -> new Concat.Concat2Iterator< ^T>(fst, snd)

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.contains?view=net-6.0
  let inline contains (target: ^T) (src: seq< ^T>) = src |> Enumerable.contains target
  let inline contains' (target: ^T, comparer: IEqualityComparer<'T>) (src: seq< ^T>) = src |> Enumerable.contains' (target, comparer)
  
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.count?view=net-6.0
  let inline count (src: seq< ^T>) = src |> Enumerable.count
  let inline count'< ^T> ([<InlineIfLambda>]predicate: ^T -> bool) (src: seq< ^T>) = Enumerable.count' predicate src
  
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.defaultifempty?view=net-6.0
  let inline defaultIfEmpty' (defaultValue: ^T) (src: seq< ^T>) = new DefaultIfEmptyIterator<'T> (src, defaultValue)
  let inline defaultIfEmpty (src: seq< ^T>) = src |> defaultIfEmpty' defaultof<'T>

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.distinct?view=net-6.0
  let inline distinct' (comparer: IEqualityComparer< ^T>) (src: seq< ^T>) : seq< ^T> = 
    match src with
    | :? list< ^T> as xs -> new DistinctListIterator< ^T>(xs, comparer)
    | :? array< ^T> as xs -> new DistinctArrayIterator< ^T>(xs, comparer)
    | :? ResizeArray< ^T> as xs -> new DistinctResizeArrayIterator< ^T>(xs, comparer)
    | _ -> new DistinctIterator< ^T>(src, comparer)
  let inline distinct (src: seq< ^T>) = src |> distinct' null

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.distinctby?view=net-6.0
  let inline distinctBy' ([<InlineIfLambda>]selector: ^T -> ^U) (comparer: IEqualityComparer< ^U>) (src: seq< ^T>) : seq< ^T> =
    match src with
    | :? list< ^T> as xs -> new DistinctByListIterator< ^T, ^U>(xs, selector, comparer)
    | :? array< ^T> as xs -> new DistinctByArrayIterator< ^T, ^U>(xs, selector, comparer)
    | :? ResizeArray< ^T> as xs -> new DistinctByResizeArrayIterator< ^T, ^U>(xs, selector, comparer)
    | _ -> new DistinctByIterator< ^T, ^U>(src, selector, comparer)
  let inline distinctBy ([<InlineIfLambda>]selector: ^T -> ^U) (src: seq< ^T>) = src |> distinctBy' selector null
  
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.elementat?view=net-6.0
  let inline elementAt (index: int) (src: seq< ^T>) =
    match src with
    | :? list< ^T> as xs -> xs[index]
    | :? array< ^T> as xs -> xs[index]
    | :? ResizeArray< ^T> as xs -> xs[index]
    | :? IList< ^T> as xs -> xs[index]
    | :? IReadOnlyList< ^T> as xs -> xs[index]
    | _ -> 
      match (src, index) |> tryGetElement with
      | (true, element) -> element
      | _ -> raise(IndexOutOfRangeException "args: index")
  
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.elementatordefault?view=net-6.0
  let inline elementAtOrDefault (index: int) (src: seq< ^T>) =
    match src with
    | :? list< ^T> as xs -> (xs, index) |> tryGetElement |> snd
    | :? array< ^T> as xs -> if index < 0 || xs.Length <= index then defaultof< ^T> else xs[index]
    | :? ResizeArray< ^T> as xs -> if index < 0 || xs.Count <= index then defaultof< ^T> else xs[index]
    | :? IList< ^T> as xs -> if index < 0 || xs.Count <= index then defaultof< ^T> else xs[index]
    | :? IReadOnlyList< ^T> as xs -> if index < 0 || xs.Count <= index then defaultof< ^T> else xs[index]
    | _ -> (src, index) |> tryGetElement |> snd

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.empty?view=net-6.0
  let inline empty () : seq<'T> = Array.empty<'T>

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.except?view=net-6.0fst.Except snd
  let inline except' (snd: seq< ^T>, comparer: IEqualityComparer< ^T>) (fst: seq< ^T>) =
    let set = HashSet< ^T>(snd, comparer)
    seq { for element in fst do if set.Add element then yield element }
  let inline except (snd: seq< ^T>) (fst: seq< ^T>) = except' (snd, null) fst

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.except?view=net-6.0
  let inline exceptBy' (snd: seq< ^U>, [<InlineIfLambda>]selector: ^T -> ^U, comparer: IEqualityComparer< ^U>) (fst: seq< ^T>) =
    let set = HashSet< ^U>(snd, comparer)
    seq { for element in fst do if set.Add (selector element) then yield element }
  let inline exceptBy (snd: seq< ^U>, [<InlineIfLambda>]selector: ^T -> ^U) (fst: seq< ^T>) = fst |> exceptBy' (snd, selector, null)
  
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.first?view=net-6.0
  let inline first (src: seq< ^T>) =
    match src with
    | :? IList< ^T> as xs -> xs[0]
    | :? IReadOnlyList< ^T> as xs -> xs[0]
    | _ ->  match src |> Enumerable.tryGetFirst with (v, true) -> v | _ -> raise (invalidOp "")
  let inline first'< ^T> (predicate: ^T -> bool) (src: seq< ^T>) = 
    match (src, predicate) |> Enumerable.tryGetFirst' with (v, true) -> v | _ -> raise (invalidOp "")
  
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.firstordefault?view=net-6.0
  // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/First.cs#L33
  let inline firstOrDefault (src: seq< ^T>) = src |> Enumerable.tryGetFirst |> fst
  let inline firstOrDefault' ([<InlineIfLambda>]predicate: ^T -> bool) (src: seq< ^T>) = (src, predicate) |> Enumerable.tryGetFirst' |> fst
  let inline firstOrDefaultWith (defaultValue: ^T) (src: seq< ^T>) =
    let (element, found) = src |> Enumerable.tryGetFirst
    if found then element else defaultValue
  let inline firstOrDefaultWith' (defaultValue: ^T) ([<InlineIfLambda>]predicate: ^T -> bool) (src: seq< ^T>) =
    let (element, found) = (src, predicate) |> Enumerable.tryGetFirst'
    if found then element else defaultValue

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.groupby?view=net-6.0
  // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/Grouping.cs#L12
  let inline groupBy ([<InlineIfLambda>]selector: 'src -> 'key) (source: seq<'src>) =
    GroupedEnumerable<'src, 'key>(source, selector, EqualityComparer<'key>.Default)
  // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/Grouping.cs#L15
  let inline groupBy' ([<InlineIfLambda>]selector: 'src -> 'key, comparer: IEqualityComparer<'key>) (source: seq<'src>) =
    GroupedEnumerable<'src, 'key>(source, selector, comparer)
  // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/Grouping.cs#L18
  let inline groupBy2 ([<InlineIfLambda>]keyselector: 'src -> 'key, [<InlineIfLambda>]elemselector: 'src -> 'elem) (source: seq<'src>) =
    GroupedEnumerable<'src, 'key, 'elem>(source, keyselector, elemselector, EqualityComparer<'key>.Default)
  // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/Grouping.cs#L21
  let inline groupBy2' ([<InlineIfLambda>]keyselector: 'src -> 'key, [<InlineIfLambda>]elemselector: 'src -> 'elem, comparer: IEqualityComparer<'key>) (source: seq<'src>) =
    GroupedEnumerable<'src, 'key, 'elem>(source, keyselector, elemselector, comparer)
  // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/Grouping.cs#L24
  let inline groupByR ([<InlineIfLambda>]keyselector: 'src -> 'key, resultselector: 'key -> seq<'src> -> 'result) (source: seq<'src>) =
    GroupedResultEnumerable<'src, 'key, 'result>(source, keyselector, resultselector, EqualityComparer<'key>.Default)
  // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/Grouping.cs#L30
  let inline groupByR' ([<InlineIfLambda>]keyselector: 'src -> 'key, [<InlineIfLambda>]resultselector: 'key -> seq<'src> -> 'result, comparer: IEqualityComparer<'key>) (source: seq<'src>) =
    GroupedResultEnumerable<'src, 'key, 'result>(source, keyselector, resultselector, comparer)
  // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/Grouping.cs#L27
  let inline groupByR2 ([<InlineIfLambda>]keyselector: 'src -> 'key, [<InlineIfLambda>]elemselector: 'src -> 'elem, [<InlineIfLambda>]resultselector: 'key -> seq<'elem> -> 'result) (source: seq<'src>) =
    GroupedResultEnumerable<'src, 'key, 'elem, 'result>(source, keyselector, elemselector, resultselector, EqualityComparer<'key>.Default)
  // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/Grouping.cs#L30
  let inline groupByR2' ([<InlineIfLambda>]keyselector: 'src -> 'key, [<InlineIfLambda>]elemselector: 'src -> 'elem, [<InlineIfLambda>]resultselector: 'key -> seq<'elem> -> 'result, comparer: IEqualityComparer<'key>) (source: seq<'src>) =
    GroupedResultEnumerable<'src, 'key, 'elem, 'result>(source, keyselector, elemselector, resultselector, comparer)
    
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.groupjoin?view=net-6.0
  // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/GroupJoin.cs#L10
  let inline groupJoin (inner: seq<'inner>, [<InlineIfLambda>]outerkeyselector: 'outer -> 'key, [<InlineIfLambda>]innerkeyselector: 'inner -> 'key, [<InlineIfLambda>]resultselector: 'outer -> seq<'inner> -> 'result) (outer: seq<'outer>) =
    groupJoinIterator(outer, inner, outerkeyselector, innerkeyselector, resultselector, EqualityComparer<'key>.Default)
    // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/GroupJoin.cs#L10
  let inline groupJoin' (inner: seq<'inner>, [<InlineIfLambda>]outerkeyselector: 'outer -> 'key, [<InlineIfLambda>]innerkeyselector: 'inner -> 'key, [<InlineIfLambda>]resultselector: 'outer -> seq<'inner> -> 'result, comparer: IEqualityComparer<'key>) (outer: seq<'outer>) =
    groupJoinIterator(outer, inner, outerkeyselector, innerkeyselector, resultselector, comparer)

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.intersect?view=net-6.0
  let inline intersect' (snd: seq< ^T>, comparer: IEqualityComparer< ^T>) (fst: seq< ^T>) = 
    let set = HashSet< ^T>(snd, comparer)
    seq { for element in fst do if set.Remove element then yield element }
  let inline intersect (second: seq< ^Source>) (first: seq< ^Source>)  = first |> intersect' (second, null)

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.intersectby?view=net-6.0
  let inline intersectBy' (snd: seq< ^U>, [<InlineIfLambda>]selector: ^T -> ^U, comparer: IEqualityComparer< ^U>) (fst: seq< ^T>) =
    let set = HashSet< ^U>(snd, comparer)
    seq { for element in fst do if set.Remove (selector element) then yield element }
  let inline intersectBy (snd: seq< ^U>, [<InlineIfLambda>]selector: ^T -> ^U) (fst: seq< ^T>) = fst |> intersectBy' (snd, selector, null)

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.join?view=net-6.0
  // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/Join.cs
  let inline join (inner: seq< ^inner>, [<InlineIfLambda>]outerkeyselector: ^outer -> ^key, [<InlineIfLambda>]innerkeyselector: ^inner -> ^key, [<InlineIfLambda>]resultselector: ^outer -> ^inner -> ^result) (outer: seq< ^outer>) =
    joinIterator(outer, inner, outerkeyselector, innerkeyselector, resultselector, EqualityComparer< ^key>.Default)
  let inline join' (inner: seq< ^inner>, [<InlineIfLambda>]outerkeyselector: ^outer -> ^key, [<InlineIfLambda>]innerkeyselector: ^inner -> ^key, [<InlineIfLambda>]resultselector: ^outer -> ^inner -> ^result, comparer: IEqualityComparer< ^key>) (outer: seq< ^outer>) =
    joinIterator(outer, inner, outerkeyselector, innerkeyselector, resultselector, comparer)

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.last?view=net-6.0
  let inline last (src: seq< ^T>) = src |> Enumerable.tryGetLast
  let inline last'< ^T> ([<InlineIfLambda>]predicate: ^T -> bool) (src: seq< ^T>) = Enumerable.tryGetLast' (src, predicate)
  
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.lastordefault?view=net-6.0
  let inline lastOrDefault (src: seq< ^T>) = src |> Enumerable.tryGetLast |> fst
  let inline lastOrDefault' ([<InlineIfLambda>]predicate: ^T -> bool) (src: seq< ^T>) = (src, predicate) |> Enumerable.tryGetLast' |> fst
  let inline lastOrDefaultWith (defaultValue: ^T) (src: seq< ^T>) = 
    let (element, found) = src |> Enumerable.tryGetLast
    if found then element else defaultValue
  let inline lastOrDefaultWith' ([<InlineIfLambda>]predicate: ^T -> bool, defaultValue: ^T) (src: seq< ^T>) =
    let (element, found) = (src, predicate) |> Enumerable.tryGetLast'
    if found then element else defaultValue

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.longcount?view=net-6.0
  let inline longCount (src: seq< ^T>) : int64 = src |> Enumerable.longCount
  let inline longCount' ([<InlineIfLambda>]predicate: ^T -> bool) (src: seq< ^T>) = src |> Enumerable.longCount' predicate
  
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.max?view=net-6.0
  let inline max (src: seq< ^T>) =
    match src with
    | :? list< ^T> as ls ->
      let rec max' (ls: list< ^T>, v: ^T) =
        match ls with
        | h::tail -> max' (tail, if h < v then v else h)
        | _ -> v
      match ls with
      | h::tail -> max' (tail, h)
      | _ -> defaultof< ^T>
    | :? array< ^T> as ary -> 
      if 0 < ary.Length then
        let mutable v = ary[0]
        for i = 1 to ary.Length - 1 do
          let current = ary[i]
          if v < current then v <- current
        v
      else
        defaultof< ^T>
    | :? ResizeArray< ^T> as ls -> 
      if 0 < ls.Count then
        let mutable v = ls[0]
        for i = 1 to ls.Count - 1 do
          let current = ls[i]
          if v < current then v <- current
        v
      else
        defaultof< ^T>
    | _ ->
      use iter = src.GetEnumerator()
      if iter.MoveNext() then
        let mutable v = iter.Current
        while iter.MoveNext() do
          let c = iter.Current
          if v < c then v <- c
        v
      else
        defaultof< ^T>

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.maxby?view=net-6.0
  // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/Max.cs#L516
  let inline maxBy' ([<InlineIfLambda>]selector: ^T -> ^U, comparer: IComparer< ^U>) (src: seq< ^T>) = 
    use e = src.GetEnumerator()
    if e.MoveNext() |> not
    then 
      if defaultof< ^T> = null then defaultof< ^T> else raise (invalidOp "no elements")
    else
      let mutable value = e.Current
      let mutable key = selector value
      while e.MoveNext() do
        let next_value = e.Current
        let next_key = selector next_value
        if 0 < comparer.Compare(next_key, key) then
          key <- next_key
          value <- next_value
      value

  let inline maxBy ([<InlineIfLambda>]selector: ^T -> ^U) (src: seq< ^T>) = src |> maxBy' (selector, Comparer< ^U>.Default)
  
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.min?view=net-6.0
  let inline min (src: seq< ^T>) =
    match src with
    | :? list< ^T> as ls ->
      let rec min' (ls: list< ^T>, v: ^T) =
        match ls with
        | h::tail -> min' (tail, if h < v then h else v)
        | _ -> v
      match ls with
      | h::tail -> min' (tail, h)
      | _ -> Unchecked.defaultof< ^T>
    | :? array< ^T> as ary -> 
      if 0 < ary.Length then
        let mutable v = ary[0]
        for i = 1 to ary.Length - 1 do
          let current = ary[i]
          if current < v then v <- current
        v
      else
        Unchecked.defaultof< ^T>
    | :? ResizeArray< ^T> as ls -> 
      if 0 < ls.Count then
        let mutable v = ls[0]
        for i = 1 to ls.Count - 1 do
          let current = ls[i]
          if current < v then v <- current
        v
      else
        Unchecked.defaultof< ^T>
    | _ ->
      use iter = src.GetEnumerator()
      if iter.MoveNext() then
        let mutable v = iter.Current
        while iter.MoveNext() do
          let c = iter.Current
          if c < v then v <- c
        v
      else
        Unchecked.defaultof< ^T>

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.minby?view=net-6.0
  // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/Min.cs#L474
  let inline minBy' ([<InlineIfLambda>]selector: ^T -> ^U, comparer: IComparer< ^U>) (src: seq< ^T>) =
    use e = src.GetEnumerator()
    if e.MoveNext() |> not
    then 
      if defaultof< ^T> = null then defaultof< ^T> else raise (invalidOp "no elements")
    else
      let mutable value = e.Current
      let mutable key = selector value
      while e.MoveNext() do
        let next_value = e.Current
        let next_key = selector next_value
        if comparer.Compare(next_key, key) < 0 then
          key <- next_key
          value <- next_value
      value
  let inline minBy ([<InlineIfLambda>]selector: ^T -> ^U) (src: seq< ^T>) = src |> minBy'(selector, Comparer< ^U>.Default)

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.oftype?view=net-6.0
  let inline ofType< ^T> (src: IEnumerable) = OfTypeIterator< ^T> src
  
  // // [NOT IMPLEMENTED] / WIP
  // // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.orderby?view=net-6.0
  // // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/OrderBy.cs
  // let inline orderBy ([<InlineIfLambda>]selector: ^T -> ^Key) (src: seq< ^T>) = src.OrderBy(selector)
  // let inline orderBy' ([<InlineIfLambda>]selector: ^T -> ^Key) (comparer: IComparer< ^Key>) (src: seq< ^T>) = src.OrderBy(selector, comparer)

  // // [NOT IMPLEMENTED]
  // // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.orderbydescending?view=net-6.0
  // let inline orderByDescending ([<InlineIfLambda>]selector: 'source -> 'key) (src: seq<'source>) = src.OrderByDescending(selector)
  // let inline orderByDescending' ([<InlineIfLambda>]selector: 'source -> 'key) (comparer: IComparer<'key>) (src: seq<'source>) = src.OrderByDescending(selector, comparer)

  // TODO: It is a very slow function compared to System.Linq,
  //       so performance tuning should be done.
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.prepend?view=net-6.0
  // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/AppendPrepend.cs#L23
  let inline prepend (element: ^T) (src: seq< ^T>) =
    match src with
    | :? AppendPrependIterator< ^T> as appendable -> appendable.Prepend(element)
    | _ -> new AppendPrepend1Iterator< ^T>(src, element, false)

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.reverse?view=net-6.0
  let inline reverse (src: seq< ^T>) = new Reverse.ReverseIterator< ^T>(src)
  
  // src: https://github.com/dotnet/runtime/blob/release/6.0/src/libraries/System.Linq/src/System/Linq/Select.cs#L13
  let inline select<'T, 'U> ([<InlineIfLambda>] selector: 'T -> 'U) (source: seq< 'T>) : seq< 'U> =
    match source with
    | :? Iterator<'T> as iter -> iter.Select selector
    | :? IList<'T> as ilist ->
      match ilist with
      | :? array<'T> as ary -> if ary.Length = 0 then Array.Empty<'U>() else new SelectArrayIterator<'T, 'U>(ary, selector)
      | :? ResizeArray<'T> as list -> new SelectListIterator<'T, 'U>(list, selector)
      | _ -> new SelectIListIterator<'T, 'U>(ilist, selector)
    | :? Funtom.linq.Interfaces.IPartition<'T> as partition ->
      match partition with
      | :? EmptyPartition<'T> as empty -> EmptyPartition<'U>.Instance
      | _ -> new SelectIPartitionIterator<'T, 'U>(partition, selector)
    | _ -> new SelectEnumerableIterator<'T, 'U>(source, selector)

  // TODO
  //let inline select<'T, 'U> ([<InlineIfLambda>]selector: 'T -> 'U) (src: seq<'T>): seq<'U> = src.Select selector
  let inline select' ([<InlineIfLambda>]selector: ^T -> int -> ^Result) (src: seq< ^T>): seq< ^Result> =
    let mutable i = -1
    seq {
      for v in src do
        i <- i + 1
        yield (selector v i)
    }

  // // [NOT IMPLEMENTED]
  // // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.selectmany?view=net-6.0
  // let inline selectMany ([<InlineIfLambda>]selector: ^T -> seq< ^Result>) (src: seq< ^T>) = src.SelectMany(selector)
  // let inline selectMany' ([<InlineIfLambda>]selector: ^T -> int -> seq< ^Result>) (src: seq< ^T>) = src.SelectMany(selector)
  // let inline selectMany2 ([<InlineIfLambda>]resultSelector: ^T -> ^Collection -> ^Result) ([<InlineIfLambda>]collectionSelector: ^T -> seq< ^Collection>) (src: seq< ^T>) = src.SelectMany(collectionSelector, resultSelector)
  // let inline selectMany2' ([<InlineIfLambda>]resultSelector: ^T -> ^Collection -> ^Result) ([<InlineIfLambda>]collectionSelector: ^T -> int -> seq< ^Collection>) (src: seq< ^T>) = src.SelectMany(collectionSelector, resultSelector)

  // // [NOT IMPLEMENTED]
  // // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.sequenceequal?view=net-6.0
  // let inline sequenceEqual (snd: seq< ^T>) (fst: seq< ^T>) = fst.SequenceEqual(snd)
  // let inline sequenceEqual' (comparer: IEqualityComparer< ^T>) (snd: seq< ^T>) (fst: seq< ^T>) = fst.SequenceEqual(snd, comparer)

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.single?view=net-6.0
  let inline single (src: seq< ^T>) =
    let (found, value) = src |> Enumerable.tryGetSingle
    if found then value else raise (invalidOp "no elements")
  let inline single' ([<InlineIfLambda>]predicate: ^T -> bool) (src: seq< ^T>) =
    let (found, value) = (src, predicate) |> Enumerable.tryGetSingle'
    if found then value else raise (invalidOp "no elements")

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.singleordefault?view=net-6.0
  let inline singleOrDefault (src: seq< ^T>) = src |> Enumerable.tryGetSingle |> snd
  let inline singleOrDefault' ([<InlineIfLambda>]predicate: ^T -> bool) (src: seq< ^T>) = (src, predicate) |> Enumerable.tryGetSingle' |> snd
  let inline singleOrDefaultWith (defaultValue: ^T) (src: seq< ^T>) =
    let (found, value) = src |> Enumerable.tryGetSingle
    if found then value else defaultValue
  let inline singleOrDefaultWith' ([<InlineIfLambda>]predicate: ^T -> bool, defaultValue: ^T) (src: seq< ^T>) =
    let (found, value) = (src, predicate) |> Enumerable.tryGetSingle'
    if found then value else defaultValue

  // // [NOT IMPLEMENTED]
  // // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.skip?view=net-6.0
  // let inline skip (count: int) (src: seq< ^T>) = src.Skip(count)

  // // [NOT IMPLEMENTED]
  // // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.skiplast?view=net-6.0
  // let inline skipLast (count: int) (src: seq< ^T>) = src.SkipLast(count)

  // // [NOT IMPLEMENTED]
  // // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.skipwhile?view=net-6.0
  // let inline skipWhile ([<InlineIfLambda>]predicate: ^T -> bool) (src: seq< ^T>) = src.SkipWhile(predicate)
  // let inline skipWhile' ([<InlineIfLambda>]predicate: ^T -> int -> bool) (src: seq< ^T>) = src.SkipWhile(predicate)

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.sum?view=net-6.0
  let inline sum (src: seq< ^T>) =
    match src with
    | :? array< ^T> as ary ->
      if 0 < ary.Length then
        let mutable acc = ary[0]
        for i = 1 to ary.Length - 1 do
          acc <- acc + ary[i]
        acc
      else Unchecked.defaultof< ^T>
    | :? ResizeArray< ^T> as ary ->
      if 0 < ary.Count then
        let mutable acc = ary[0]
        for i = 1 to ary.Count - 1 do
          acc <- acc + ary[i]
        acc
      else Unchecked.defaultof< ^T>
    | :? list< ^T> as ls ->
      let rec f xs acc =
        match xs with
        | h::tail -> f tail (acc + h)
        | _ -> acc
      match ls with
      | h::tail -> f tail h
      | _ -> Unchecked.defaultof< ^T>
    | _ ->
      use iter = src.GetEnumerator()
      if iter.MoveNext() then
        let mutable acc = iter.Current
        while iter.MoveNext() do
          acc <- acc + iter.Current
        acc
      else
        Unchecked.defaultof< ^T>

  // // [NOT IMPLEMENTED]
  // // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.take?view=net-6.0
  // let inline take (count: int) (src: seq< ^T>) = src.Take(count)
  // let inline take' (range: System.Range) (src: seq< ^T>) = src.Take(range)

  // // [NOT IMPLEMENTED]
  // // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.takelast?view=net-6.0
  // let inline takeLast (count: int) (src: seq< ^T>) = src.TakeLast(count)
  
  // // [NOT IMPLEMENTED]
  // // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.takewhile?view=net-6.0
  // let inline takeWhile ([<InlineIfLambda>]predicate: ^T -> bool) (src: seq< ^T>) = src.TakeWhile(predicate)
  // let inline takeWhile' ([<InlineIfLambda>]predicate: ^T -> int -> bool) (src: seq< ^T>) = src.TakeWhile(predicate)

  // // [NOT IMPLEMENTED]
  // // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.thenby?view=net-6.0
  // let inline thenBy ([<InlineIfLambda>]selector: ^T -> ^Key) (src: IOrderedEnumerable< ^T>) = src.ThenBy(selector)
  // let inline thenBy' (comparer: IComparer< ^Key>) ([<InlineIfLambda>]selector: ^T -> ^Key) (src: IOrderedEnumerable< ^T>) = src.ThenBy(selector, comparer)

  // // [NOT IMPLEMENTED]
  // // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.thenbydescending?view=net-6.0
  // let inline thenByDescending([<InlineIfLambda>]selector: ^T -> ^Key) (src: IOrderedEnumerable< ^T>) = src.ThenByDescending (selector)
  // let inline thenByDescending' (comparer: IComparer< ^Key>) ([<InlineIfLambda>]selector: ^T -> ^Key) (src: IOrderedEnumerable< ^T>) = src.ThenByDescending (selector, comparer)

  // // [NOT IMPLEMENTED]
  // // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.tolookup?view=net-6.0
  // let inline toLookup ([<InlineIfLambda>]selector: ^T -> ^Key) (src: seq< ^T>) = src.ToLookup(selector)
  // let inline toLookup' (comparer: IEqualityComparer< ^Key>) ([<InlineIfLambda>]selector: ^T -> ^Key) (src: seq< ^T>) = src.ToLookup(selector, comparer)
  // let inline toLookup2 ([<InlineIfLambda>]elementSelector: ^T -> ^Key) ([<InlineIfLambda>]keySelector: ^T -> ^Key) (src: seq< ^T>) = src.ToLookup(keySelector, elementSelector)
  // let inline toLookup2' (comparer: IEqualityComparer< ^Key>) ([<InlineIfLambda>]elementSelector: ^T -> ^Key) ([<InlineIfLambda>]keySelector: ^T -> ^Key) (src: seq< ^T>) = src.ToLookup(keySelector, elementSelector, comparer)

  // // [NOT IMPLEMENTED]
  // // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.trygetnonenumeratedcount?view=net-6.0
  // let inline tryGetNonEnumeratedCount (count: outref<int>) (src: seq< ^T>) = src.TryGetNonEnumeratedCount(&count)

  // // [NOT IMPLEMENTED]
  // // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.union?view=net-6.0
  // let inline union (snd: seq< ^T>) (fst: seq< ^T>) = fst.Union snd
  // let inline union' (comparer: IEqualityComparer< ^T>) (snd: seq< ^T>) (fst: seq< ^T>) = fst.Union(snd, comparer)

  // // [NOT IMPLEMENTED]
  // // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.unionby?view=net-6.0
  // let inline unionBy ([<InlineIfLambda>]selector: ^T -> ^Key) (snd: seq< ^T>) (fst: seq< ^T>) = fst.UnionBy (snd, selector)
  // let inline unionBy' (comparer: IEqualityComparer< ^Key>) ([<InlineIfLambda>]selector: ^T -> ^Key) (snd: seq< ^T>) (fst: seq< ^T>) = fst.UnionBy (snd, selector, comparer)

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.where?view=net-6.0
  let inline where ([<InlineIfLambda>] predicate: ^T -> bool) (source: seq< ^T>) : seq< ^T> =
    match source with
    | :? WhereIterator< ^T> as iterator -> iterator.where predicate
    | :? WhereArrayIterator< ^T> as iterator -> iterator.where predicate
    | :? WhereResizeArrayIterator< ^T> as iterator -> iterator.where predicate
    | :? WhereListIterator< ^T> as iterator -> iterator.where predicate
    | :? array< ^T> as ary ->
      if ary.Length = 0 then System.Array.Empty< ^T>() 
      else new WhereArrayIterator< ^T>(ary, predicate)
    | :? ResizeArray< ^T> as ls -> new WhereResizeArrayIterator<'T> (ls, predicate)
    | :? list< ^T> as ls -> new WhereListIterator< ^T>(ls, predicate)
    | _ -> new WhereIterator< ^T> (source, predicate)

  // TODO
  let inline where' ([<InlineIfLambda>]predicate: ^T -> int -> bool) (src: seq< ^T>) =
    let mutable i = -1
    seq {
      for v in src do
        i <- i + 1
        if predicate v i then
          yield v
    }

  // // [NOT IMPLEMENTED]
  // // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.zip?view=net-6.0
  // let inline zip (snd: seq< ^Fst>) (fst: seq< ^Snd>) = fst.Zip(snd)
  // let inline zip' (snd: seq< ^Snd>) ([<InlineIfLambda>]selector: ^Fst -> ^Snd -> ^Result) (fst: seq< ^Fst>) = fst.Zip(snd, selector)
  // let inline zip3 (snd: seq< ^Snd>) (thd: seq< ^Thd>) (fst: seq< ^Fst>) = fst.Zip(snd, thd)