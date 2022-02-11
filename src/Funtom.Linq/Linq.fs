namespace Funtom.Linq

open System
open System.Linq
open System.Collections
open System.Collections.Generic
open System.Runtime.CompilerServices
open System.Diagnostics
open Funtom.Linq.Core
open Funtom.Linq.Iterator
open Empty
open Basis
open Select
open AppendPrepend
open Chunk
open Funtom.Linq.Common.Interfaces

module Linq =
  // TODO: seq<'T> に対する ToArray() が非常に遅いので、要高速化
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.toarray?view=net-6.0
  let inline toArray (src: seq<'T>) =
    match src with
    | :? IListProvider<'T> as provider -> provider.ToArray()
    | _ -> Enumerable.toArray src

  // TODO
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.todictionary?view=net-6.0
  let inline toDictionary ([<InlineIfLambda>]selector: ^T -> ^Key) (src: seq< ^T>) = 
    src.ToDictionary(selector)
  let inline toDictionary' (comparer: IEqualityComparer< ^Key>) ([<InlineIfLambda>]selector: ^T -> ^Key) (src: seq< ^T>) = 
    src.ToDictionary(selector, comparer)
  let inline toDictionary2 ([<InlineIfLambda>]elementSelector: ^T -> ^Element) ([<InlineIfLambda>]keySelector: ^T -> ^Key) (src: seq< ^T>) = 
    src.ToDictionary(keySelector, elementSelector)
  let inline toDictionary2' (comparer: IEqualityComparer< ^Key>) ([<InlineIfLambda>]elementSelector: ^T -> ^Element) ([<InlineIfLambda>]keySelector: ^T -> ^Key) (src: seq< ^T>) =
    src.ToDictionary(keySelector, elementSelector, comparer)
  
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.tohashset?view=net-6.0
  let inline toHashSet (src: seq< ^T>) = HashSet< ^T>(src, null)
  let inline toHashSet' (comparer: IEqualityComparer< ^T>) (src: seq< ^T>) = HashSet< ^T>(src, comparer)
  
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.tolist?view=net-6.0
  let inline toList (src: seq< ^T>) = 
    match src with
    | :? IListProvider< ^T> as provider -> provider.ToList()
    | _ -> ResizeArray(src)

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
  let inline asEnumerable (src: seq< ^T>) = src.AsEnumerable()
    
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.cast?view=net-6.0
  let inline cast (source: IEnumerable) : IEnumerable< ^T> = 
    match source with
    | :? IEnumerable< ^T> as src -> src
    | _ -> CastIterator< ^T> source

  // TODO: Performance tuning
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.chunk?view=net-6.0
  let inline chunk (size: int) (src: seq< ^T>) = chunkIterator(src, size)

  // TODO
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.concat?view=net-6.0
  let inline concat (fst: seq< ^T>) (snd: seq< ^T>) = fst.Concat snd

  // TODO
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.contains?view=net-6.0
  let inline contains (target: ^T) (src: seq< ^T>) = src.Contains target
  
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.count?view=net-6.0
  let inline count (src: seq< ^T>) = Enumerable.count src
  let inline count'< ^T> ([<InlineIfLambda>]predicate: ^T -> bool) (src: seq< ^T>) = Enumerable.count' (src, predicate)
  
  // TODO
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.defaultifempty?view=net-6.0
  let inline defaultIfEmpty (src: seq< ^T>) = src.DefaultIfEmpty()
  let inline defaultIfEmpty' (defaultValue: ^T) (src: seq< ^T>) = src.DefaultIfEmpty defaultValue

  // TODO
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.distinct?view=net-6.0
  let inline distinct (src: seq< ^T>) = src.Distinct()
  let inline distinct' (comparer: IEqualityComparer< ^T>) (src: seq< ^T>) = src.Distinct comparer

  // TODO
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.distinctby?view=net-6.0
  let inline distinctBy ([<InlineIfLambda>]selector: ^T -> ^U) (src: seq< ^T>) = src.DistinctBy selector
  let inline distinctBy' ([<InlineIfLambda>]selector: ^T -> ^U) (comparer: IEqualityComparer< ^U>) (src: seq< ^T>) = src.DistinctBy(selector, comparer)
  
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.elementat?view=net-6.0
  let inline elementAt (index: int) (src: seq< ^T>) =
    match src with
    | :? IList< ^T> as xs -> xs[index]
    | :? IReadOnlyList< ^T> as xs -> xs[index]
    // TODO
    | _ -> src.ElementAt index
  
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.elementatordefault?view=net-6.0
  let inline elementAtOrDefault (index: int) (src: seq< ^T>) =
    match src with
    | :? IList< ^T> as xs -> if index < 0 || xs.Count <= index then Unchecked.defaultof< ^T> else xs[index]
    | :? IReadOnlyList< ^T> as xs -> if index < 0 || xs.Count <= index then Unchecked.defaultof< ^T> else xs[index]
    // TODO
    | _ -> src.ElementAtOrDefault index

  // TODO
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.empty?view=net-6.0
  let inline empty () = Enumerable.Empty< ^T>()

  // TODO
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.except?view=net-6.0
  let inline except (fst: seq< ^T>) (snd: seq< ^T>) = fst.Except snd
  let inline except' (comparer: IEqualityComparer< ^T>) (fst: seq< ^T>) (snd: seq< ^T>) = fst.Except (snd, comparer)
  
  // TODO
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.except?view=net-6.0
  let inline exceptBy ([<InlineIfLambda>]selector: ^T -> ^U) (fst: seq< ^T>) (snd: seq< ^U>) = fst.ExceptBy(snd, selector)
  let inline exceptBy' ([<InlineIfLambda>]selector: ^T -> ^U) (comparer: IEqualityComparer< ^U>)  (fst: seq< ^T>) (snd: seq< ^U>) = fst.ExceptBy(snd, selector, comparer)

  // TODO
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.first?view=net-6.0
  let inline first (src: seq< ^T>) =
    match src with
    | :? IList< ^T> as xs -> xs[0]
    | :? IReadOnlyList< ^T> as xs -> xs[0]
    | _ -> src.First()
  let inline first'< ^T> (predicate: ^T -> bool) (src: seq< ^T>) = src.First predicate

  // TODO
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.firstordefault?view=net-6.0
  let inline firstOrDefault (src: seq< ^T>) = src.FirstOrDefault()
  let inline firstOrDefault' ([<InlineIfLambda>]predicate: ^T -> bool) (src: seq< ^T>) = src.FirstOrDefault predicate
  let inline firstOrDefaultWith (defaultValue: ^T) (src: seq< ^T>) = src.FirstOrDefault(defaultValue)
  let inline firstOrDefaultWith' (defaultValue: ^T) ([<InlineIfLambda>]predicate: ^T -> bool) (src: seq< ^T>) = src.FirstOrDefault(predicate, defaultValue)

  // TODO
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.groupby?view=net-6.0
  let inline groubBy ([<InlineIfLambda>]keySelector: ^Source -> ^Key) ([<InlineIfLambda>]resultSelector: ^Key -> seq< ^Source> -> ^Result) (source: seq< ^Source>) =
    source.GroupBy(keySelector, resultSelector)
  let inline groubBy' ([<InlineIfLambda>]keySelector: ^Source -> ^Key) ([<InlineIfLambda>]resultSelector: ^Key -> seq< ^Source> -> ^Result) (comparer: IEqualityComparer< ^Key>) (source: seq< ^Source>) =
    source.GroupBy(keySelector, resultSelector, comparer)
  let inline groubByElement ([<InlineIfLambda>]keySelector: ^Source -> ^Key) ([<InlineIfLambda>]elementSelector: ^Source -> ^Element) (source: seq< ^Source>) =
    source.GroupBy(keySelector, elementSelector)
  let inline groubByElement' ([<InlineIfLambda>]keySelector: ^Source -> ^Key) ([<InlineIfLambda>]elementSelector: ^Source -> ^Element) (comparer: IEqualityComparer< ^Key>) (source: seq< ^Source>) =
    source.GroupBy(keySelector, elementSelector, comparer)
  let inline groubBy2 ([<InlineIfLambda>]keySelector: ^Source -> ^Key) ([<InlineIfLambda>]elementSelector: ^Source -> ^Element) ([<InlineIfLambda>]resultSelector: ^Key -> seq< ^Element> -> ^Result) (source: seq< ^Source>) =
    source.GroupBy(keySelector, elementSelector, resultSelector)
  let inline groubBy2' ([<InlineIfLambda>]keySelector: ^Source -> ^Key) ([<InlineIfLambda>]elementSelector: ^Source -> ^Element) ([<InlineIfLambda>]resultSelector: ^Key -> seq< ^Element> -> ^Result) (comparer: IEqualityComparer< ^Key>) (source: seq< ^Source>) =
    source.GroupBy(keySelector, elementSelector, resultSelector, comparer)
    
  // TODO
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.groupjoin?view=net-6.0
  let inline groupJoin (inner: seq< ^Inner>)  ([<InlineIfLambda>]outerKeySelector: ^Outer -> ^Key) ([<InlineIfLambda>]innerKeySelector: ^Inner -> ^Key) ([<InlineIfLambda>]resultSelector: ^Outer -> seq< ^Inner> -> ^Result) (outer: seq< ^Outer>) =
    outer.GroupJoin(inner, outerKeySelector, innerKeySelector, resultSelector)
  let inline groupJoin' (inner: seq< ^Inner>)  ([<InlineIfLambda>]outerKeySelector: ^Outer -> ^Key) ([<InlineIfLambda>]innerKeySelector: ^Inner -> ^Key) ([<InlineIfLambda>]resultSelector: ^Outer -> seq< ^Inner> -> ^Result) (comparer: IEqualityComparer< ^Key>) (outer: seq< ^Outer>) =
    outer.GroupJoin(inner, outerKeySelector, innerKeySelector, resultSelector, comparer)
  let inline groupJoin2 ([<InlineIfLambda>]outerKeySelector: ^Outer -> ^Key) ([<InlineIfLambda>]innerKeySelector: ^Inner -> ^Key) ([<InlineIfLambda>]resultSelector: ^Outer -> seq< ^Inner> -> ^Result) (outer: seq< ^Outer>, inner: seq< ^Inner>) =
    outer.GroupJoin(inner, outerKeySelector, innerKeySelector, resultSelector)
  let inline groupJoin2' ([<InlineIfLambda>]outerKeySelector: ^Outer -> ^Key) ([<InlineIfLambda>]innerKeySelector: ^Inner -> ^Key) ([<InlineIfLambda>]resultSelector: ^Outer -> seq< ^Inner> -> ^Result) (comparer: IEqualityComparer< ^Key>) (outer: seq< ^Outer>, inner: seq< ^Inner>) =
    outer.GroupJoin(inner, outerKeySelector, innerKeySelector, resultSelector, comparer)

  // TODO
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.intersect?view=net-6.0
  let inline intersect (second: seq< ^Source>) (first: seq< ^Source>)  = first.Intersect second
  let inline intersect' (second: seq< ^Source>) (first: seq< ^Source>) (comparer: IEqualityComparer< ^Source>) = first.Intersect (second, comparer)
  let inline intersect2 (first: seq< ^Source>, second: seq< ^Source>) = first.Intersect second
  let inline intersect2' (comparer: IEqualityComparer< ^Source>) (first: seq< ^Source>, second: seq< ^Source>) = first.Intersect (second, comparer)

  // TODO
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.intersectby?view=net-6.0
  let inline intersectBy (second: seq< ^Key>) ([<InlineIfLambda>]keySelector: ^Source -> ^Key) (first: seq< ^Source>)  = first.IntersectBy (second, keySelector)
  let inline intersectBy' (second: seq< ^Key>) ([<InlineIfLambda>]keySelector: ^Source -> ^Key) (comparer: IEqualityComparer< ^Key>) (first: seq< ^Source>) = first.IntersectBy (second, keySelector, comparer)
  let inline intersectBy2 ([<InlineIfLambda>]keySelector: ^Source -> ^Key) (first: seq< ^Source>, second: seq< ^Key>)  = first.IntersectBy (second, keySelector)
  let inline intersectBy2' ([<InlineIfLambda>]keySelector: ^Source -> ^Key) (comparer: IEqualityComparer< ^Key>) (first: seq< ^Source>, second: seq< ^Key>) = first.IntersectBy (second, keySelector, comparer)

  // TODO
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.join?view=net-6.0
  let inline join (inner: seq< ^Inner>) ([<InlineIfLambda>]outerKeySelector: ^Outer -> ^Key) ([<InlineIfLambda>]innerKeySelector: ^Inner -> ^Key) ([<InlineIfLambda>]resultSelector: ^Outer -> ^Inner -> ^Result) (outer: seq< ^Outer>) =
    outer.Join (inner, outerKeySelector, innerKeySelector, resultSelector)
  let inline join' (inner: seq< ^Inner>) ([<InlineIfLambda>]outerKeySelector: ^Outer -> ^Key) ([<InlineIfLambda>]innerKeySelector: ^Inner -> ^Key) ([<InlineIfLambda>]resultSelector: ^Outer -> ^Inner -> ^Result) (comparer: IEqualityComparer< ^Key>) (outer: seq< ^Outer>) =
    outer.Join (inner, outerKeySelector, innerKeySelector, resultSelector, comparer)
  let inline join2 ([<InlineIfLambda>]outerKeySelector: ^Outer -> ^Key) ([<InlineIfLambda>]innerKeySelector: ^Inner -> ^Key) ([<InlineIfLambda>]resultSelector: ^Outer -> ^Inner -> ^Result) (outer: seq< ^Outer>, inner: seq< ^Inner>) =
    outer.Join (inner, outerKeySelector, innerKeySelector, resultSelector)
  let inline join2' ([<InlineIfLambda>]outerKeySelector: ^Outer -> ^Key) ([<InlineIfLambda>]innerKeySelector: ^Inner -> ^Key) ([<InlineIfLambda>]resultSelector: ^Outer -> ^Inner -> ^Result) (comparer: IEqualityComparer< ^Key>) (outer: seq< ^Outer>, inner: seq< ^Inner>) =
    outer.Join (inner, outerKeySelector, innerKeySelector, resultSelector, comparer)

  // TODO
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.last?view=net-6.0
  let inline last (src: seq< ^T>) =
    match src with
    | :? IList< ^T> as xs -> xs[xs.Count - 1]
    | :? IReadOnlyList< ^T> as xs -> xs[xs.Count - 1]
    | _ -> src.Last()
  let inline last'< ^T> ([<InlineIfLambda>]predicate: ^T -> bool) (src: seq< ^T>) = src.Last predicate
  
  // TODO
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.lastordefault?view=net-6.0
  let inline lastOrDefault (src: seq< ^T>) = src.LastOrDefault()
  let inline lastOrDefault' ([<InlineIfLambda>]predicate: ^T -> bool) (src: seq< ^T>) = src.LastOrDefault predicate
  let inline lastOrDefaultWith (defaultValue: ^T) (src: seq< ^T>) = src.LastOrDefault(defaultValue)
  let inline lastOrDefaultWith' (defaultValue: ^T) ([<InlineIfLambda>]predicate: ^T -> bool) (src: seq< ^T>) = src.LastOrDefault(predicate, defaultValue)

  // TODO
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.longcount?view=net-6.0
  let inline longCount (src: seq< ^T>) : int64 =
    match src with
    | :? ICollection as xs -> xs.Count
    | :? ICollection< ^T> as xs -> xs.Count
    | :? IReadOnlyCollection< ^T> as xs -> xs.Count
    | _ -> src.LongCount()
  let inline longCount' ([<InlineIfLambda>]predicate: ^T -> bool) (src: seq< ^T>) = src.LongCount predicate
  
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
      | _ -> Unchecked.defaultof< ^T>
    | :? array< ^T> as ary -> 
      if 0 < ary.Length then
        let mutable v = ary[0]
        for i = 1 to ary.Length - 1 do
          let current = ary[i]
          if v < current then v <- current
        v
      else
        Unchecked.defaultof< ^T>
    | :? ResizeArray< ^T> as ls -> 
      if 0 < ls.Count then
        let mutable v = ls[0]
        for i = 1 to ls.Count - 1 do
          let current = ls[i]
          if v < current then v <- current
        v
      else
        Unchecked.defaultof< ^T>
    | _ ->
      use iter = src.GetEnumerator()
      if iter.MoveNext() then
        let mutable v = iter.Current
        while iter.MoveNext() do
          let c = iter.Current
          if v < c then v <- c
        v
      else
        Unchecked.defaultof< ^T>

  // TODO
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.maxby?view=net-6.0
  let inline maxBy ([<InlineIfLambda>]selector: ^T -> ^Key) (src: seq< ^T>) = src.MaxBy(selector)
  let inline maxBy' ([<InlineIfLambda>]selector: ^T -> ^Key) (comparer: IComparer< ^Key>) (src: seq< ^T>) = src.MaxBy(selector, comparer)
  
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

  // TODO
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.minby?view=net-6.0
  let inline minBy ([<InlineIfLambda>]selector: ^T -> ^Key) (src: seq< ^T>) = src.MinBy(selector)
  let inline minBy' ([<InlineIfLambda>]selector: ^T -> ^Key) (comparer: IComparer< ^Key>) (src: seq< ^T>) = src.MinBy(selector, comparer)

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.oftype?view=net-6.0
  let inline ofType< ^T> (src: IEnumerable) = OfTypeIterator< ^T> src
  
  // TODO
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.orderby?view=net-6.0
  let inline orderBy ([<InlineIfLambda>]selector: ^T -> ^Key) (src: seq< ^T>) = src.OrderBy(selector)
  let inline orderBy' ([<InlineIfLambda>]selector: ^T -> ^Key) (comparer: IComparer< ^Key>) (src: seq< ^T>) = src.OrderBy(selector, comparer)

  // TODO
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.orderbydescending?view=net-6.0
  let inline orderByDescending ([<InlineIfLambda>]selector: 'source -> 'key) (src: seq<'source>) = src.OrderByDescending(selector)
  let inline orderByDescending' ([<InlineIfLambda>]selector: 'source -> 'key) (comparer: IComparer<'key>) (src: seq<'source>) = src.OrderByDescending(selector, comparer)

  // TODO: It is a very slow function compared to System.Linq,
  //       so performance tuning should be done.
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.prepend?view=net-6.0
  // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/AppendPrepend.cs#L23
  let inline prepend (element: ^T) (src: seq< ^T>) =
    match src with
    | :? AppendPrependIterator< ^T> as appendable -> appendable.Prepend(element)
    | _ -> new AppendPrepend1Iterator< ^T>(src, element, false)

  // TODO
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.reverse?view=net-6.0
  let inline reverse (src: seq< ^T>) = src.Reverse()
  
  // src: https://github.com/dotnet/runtime/blob/release/6.0/src/libraries/System.Linq/src/System/Linq/Select.cs#L13
  let inline select<'T, 'U> ([<InlineIfLambda>] selector: 'T -> 'U) (source: seq< 'T>) : seq< 'U> =
    match source with
    | :? Iterator<'T> as iter -> iter.Select selector
    | :? IList<'T> as ilist ->
      match ilist with
      | :? array<'T> as ary -> if ary.Length = 0 then Array.Empty<'U>() else new SelectArrayIterator<'T, 'U>(ary, selector)
      | :? ResizeArray<'T> as list -> new SelectListIterator<'T, 'U>(list, selector)
      | _ -> new SelectIListIterator<'T, 'U>(ilist, selector)
    | :? Funtom.Linq.Common.Interfaces.IPartition<'T> as partition ->
      match partition with
      | :? EmptyPartition<'T> as empty -> EmptyPartition<'U>.Instance
      | _ -> new SelectIPartitionIterator<'T, 'U>(partition, selector)
    | _ -> new SelectEnumerableIterator<'T, 'U>(source, selector)

  //// https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.select?view=net-6.0
  //let inline select ([<InlineIfLambda>] selector: ^T -> ^Result) (source: seq< ^T>) : seq< ^Result> =
  //  match source with
  //  | :? array< ^T> as ary -> SelectArrayIterator (ary, selector)
  //  | :? ResizeArray< ^T> as ls -> SelectListIterator.create selector ls
  //  | :? list< ^T> as ls -> SelectFsListIterator.create selector ls
  //  | _ -> source.Select selector //SelectEnumerableIterator.create selector source
    //match source with
    //| :? array< ^T> as ary -> ary.Select selector //SelectArrayIterator.create selector ary
    //| :? ResizeArray< ^T> as ls -> ls.Select selector // SelectListIterator.create selector ls
    //| :? list< ^T> as ls -> SelectFsListIterator.create selector ls
    //| _ -> source.Select selector //SelectEnumerableIterator.create selector source

  // TODO
  //let inline select<'T, 'U> ([<InlineIfLambda>]selector: 'T -> 'U) (src: seq<'T>): seq<'U> = src.Select selector
  let inline select' ([<InlineIfLambda>]selector: ^T -> int -> ^Result) (src: seq< ^T>): seq< ^Result> =
    let mutable i = -1
    seq {
      for v in src do
        i <- i + 1
        yield (selector v i)
    }

  // TODO
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.selectmany?view=net-6.0
  let inline selectMany ([<InlineIfLambda>]selector: ^T -> seq< ^Result>) (src: seq< ^T>) = src.SelectMany(selector)
  let inline selectMany' ([<InlineIfLambda>]selector: ^T -> int -> seq< ^Result>) (src: seq< ^T>) = src.SelectMany(selector)
  let inline selectMany2 ([<InlineIfLambda>]resultSelector: ^T -> ^Collection -> ^Result) ([<InlineIfLambda>]collectionSelector: ^T -> seq< ^Collection>) (src: seq< ^T>) = src.SelectMany(collectionSelector, resultSelector)
  let inline selectMany2' ([<InlineIfLambda>]resultSelector: ^T -> ^Collection -> ^Result) ([<InlineIfLambda>]collectionSelector: ^T -> int -> seq< ^Collection>) (src: seq< ^T>) = src.SelectMany(collectionSelector, resultSelector)

  // TODO
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.sequenceequal?view=net-6.0
  let inline sequenceEqual (snd: seq< ^T>) (fst: seq< ^T>) = fst.SequenceEqual(snd)
  let inline sequenceEqual' (comparer: IEqualityComparer< ^T>) (snd: seq< ^T>) (fst: seq< ^T>) = fst.SequenceEqual(snd, comparer)

  // TODO
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.single?view=net-6.0
  let inline single (src: seq< ^T>) = src.Single()
  let inline single' ([<InlineIfLambda>]predicate: ^T -> bool) (src: seq< ^T>) = src.Single(predicate)

  // TODO
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.singleordefault?view=net-6.0
  let inline singleOrDefault (src: seq< ^T>) = src.SingleOrDefault()
  let inline singleOrDefault' ([<InlineIfLambda>]predicate: ^T -> bool) (src: seq< ^T>) = src.SingleOrDefault(predicate)
  let inline singleOrDefaultWith (defaultValue: ^T) (src: seq< ^T>) = src.SingleOrDefault(defaultValue)
  let inline singleOrDefaultWith' (defaultValue: ^T) ([<InlineIfLambda>]predicate: ^T -> bool) (src: seq< ^T>) = src.SingleOrDefault(predicate, defaultValue)

  // TODO
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.skip?view=net-6.0
  let inline skip (count: int) (src: seq< ^T>) = src.Skip(count)

  // TODO
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.skiplast?view=net-6.0
  let inline skipLast (count: int) (src: seq< ^T>) = src.SkipLast(count)

  // TODO
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.skipwhile?view=net-6.0
  let inline skipWhile ([<InlineIfLambda>]predicate: ^T -> bool) (src: seq< ^T>) = src.SkipWhile(predicate)
  let inline skipWhile' ([<InlineIfLambda>]predicate: ^T -> int -> bool) (src: seq< ^T>) = src.SkipWhile(predicate)

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

  // TODO
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.take?view=net-6.0
  let inline take (count: int) (src: seq< ^T>) = src.Take(count)
  let inline take' (range: System.Range) (src: seq< ^T>) = src.Take(range)

  // TODO
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.takelast?view=net-6.0
  let inline takeLast (count: int) (src: seq< ^T>) = src.TakeLast(count)
  
  // TODO
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.takewhile?view=net-6.0
  let inline takeWhile ([<InlineIfLambda>]predicate: ^T -> bool) (src: seq< ^T>) = src.TakeWhile(predicate)
  let inline takeWhile' ([<InlineIfLambda>]predicate: ^T -> int -> bool) (src: seq< ^T>) = src.TakeWhile(predicate)

  // TODO
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.thenby?view=net-6.0
  let inline thenBy ([<InlineIfLambda>]selector: ^T -> ^Key) (src: IOrderedEnumerable< ^T>) = src.ThenBy(selector)
  let inline thenBy' (comparer: IComparer< ^Key>) ([<InlineIfLambda>]selector: ^T -> ^Key) (src: IOrderedEnumerable< ^T>) = src.ThenBy(selector, comparer)

  // TODO
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.thenbydescending?view=net-6.0
  let inline thenByDescending([<InlineIfLambda>]selector: ^T -> ^Key) (src: IOrderedEnumerable< ^T>) = src.ThenByDescending (selector)
  let inline thenByDescending' (comparer: IComparer< ^Key>) ([<InlineIfLambda>]selector: ^T -> ^Key) (src: IOrderedEnumerable< ^T>) = src.ThenByDescending (selector, comparer)

  // TODO
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.tolookup?view=net-6.0
  let inline toLookup ([<InlineIfLambda>]selector: ^T -> ^Key) (src: seq< ^T>) = src.ToLookup(selector)
  let inline toLookup' (comparer: IEqualityComparer< ^Key>) ([<InlineIfLambda>]selector: ^T -> ^Key) (src: seq< ^T>) = src.ToLookup(selector, comparer)
  let inline toLookup2 ([<InlineIfLambda>]elementSelector: ^T -> ^Key) ([<InlineIfLambda>]keySelector: ^T -> ^Key) (src: seq< ^T>) = src.ToLookup(keySelector, elementSelector)
  let inline toLookup2' (comparer: IEqualityComparer< ^Key>) ([<InlineIfLambda>]elementSelector: ^T -> ^Key) ([<InlineIfLambda>]keySelector: ^T -> ^Key) (src: seq< ^T>) = src.ToLookup(keySelector, elementSelector, comparer)

  // TODO
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.trygetnonenumeratedcount?view=net-6.0
  let inline tryGetNonEnumeratedCount (count: outref<int>) (src: seq< ^T>) = src.TryGetNonEnumeratedCount(&count)

  // TODO
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.union?view=net-6.0
  let inline union (snd: seq< ^T>) (fst: seq< ^T>) = fst.Union snd
  let inline union' (comparer: IEqualityComparer< ^T>) (snd: seq< ^T>) (fst: seq< ^T>) = fst.Union(snd, comparer)

  // TODO
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.unionby?view=net-6.0
  let inline unionBy ([<InlineIfLambda>]selector: ^T -> ^Key) (snd: seq< ^T>) (fst: seq< ^T>) = fst.UnionBy (snd, selector)
  let inline unionBy' (comparer: IEqualityComparer< ^Key>) ([<InlineIfLambda>]selector: ^T -> ^Key) (snd: seq< ^T>) (fst: seq< ^T>) = fst.UnionBy (snd, selector, comparer)

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

  // TODO
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.zip?view=net-6.0
  let inline zip (snd: seq< ^Fst>) (fst: seq< ^Snd>) = fst.Zip(snd)
  let inline zip' (snd: seq< ^Snd>) ([<InlineIfLambda>]selector: ^Fst -> ^Snd -> ^Result) (fst: seq< ^Fst>) = fst.Zip(snd, selector)
  let inline zip3 (snd: seq< ^Snd>) (thd: seq< ^Thd>) (fst: seq< ^Fst>) = fst.Zip(snd, thd)