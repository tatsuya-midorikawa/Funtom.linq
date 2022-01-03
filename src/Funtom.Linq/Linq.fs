namespace Funtom.Linq

open System.Linq
open System.Collections
open System.Collections.Generic
open Funtom.Linq.Core
open System.Runtime.CompilerServices

module Linq =
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.toarray?view=net-6.0
  let inline toArray<'T> (src: seq<'T>) = src.ToArray()

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.todictionary?view=net-6.0
  let inline toDictionary ([<InlineIfLambda>]selector: 'source -> 'key) (src: seq<'source>) = src.ToDictionary(selector)
  let inline toDictionary' (comparer: IEqualityComparer<'key>) ([<InlineIfLambda>]selector: 'source -> 'key) (src: seq<'source>) = src.ToDictionary(selector, comparer)
  let inline toDictionary2 ([<InlineIfLambda>]elementSelector: 'source -> 'element) ([<InlineIfLambda>]keySelector: 'source -> 'key) (src: seq<'source>) = src.ToDictionary(keySelector, elementSelector)
  let inline toDictionary2' (comparer: IEqualityComparer<'key>) ([<InlineIfLambda>]elementSelector: 'source -> 'element) ([<InlineIfLambda>]keySelector: 'source -> 'key) (src: seq<'source>) = src.ToDictionary(keySelector, elementSelector, comparer)
  
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.tohashset?view=net-6.0
  let inline toHashSet (src: seq<'source>) = src.ToHashSet()
  let inline toHashSet' (comparer: IEqualityComparer<'source>) (src: seq<'source>) = src.ToHashSet(comparer)
  
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.tolist?view=net-6.0
  let inline toList<'T> (src: seq<'T>) = src.ToList()

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.aggregate?view=net-6.0
  let inline aggregate<'source, 'accumulate, 'result> ([<InlineIfLambda>]fx: 'accumulate -> 'source -> 'accumulate) ([<InlineIfLambda>]selector: 'accumulate -> 'result) (seed: 'accumulate) (src: seq<'source>) =
    src.Aggregate(seed, fx, selector)
  
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.all?view=net-6.0
  let inline all<'T> ([<InlineIfLambda>]predicate: 'T -> bool) (src: seq<'T>) = src.All predicate
  
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.any?view=net-6.0#System_Linq_Enumerable_Any__1_System_Collections_Generic_IEnumerable___0__
  let inline any<'T> (src: seq<'T>) = src.Any()
  
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.any?view=net-6.0#System_Linq_Enumerable_Any__1_System_Collections_Generic_IEnumerable___0__System_Func___0_System_Boolean__
  let inline any'<'T> ([<InlineIfLambda>]predicate: 'T -> bool) (src: seq<'T>) = src.Any predicate

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.append?view=net-6.0
  let inline append<'T> (element: 'T) (src: seq<'T>) = src.Append element

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.asenumerable?view=net-6.0
  let inline asEnumerable<'T> (src: seq<'T>) = src.AsEnumerable()
    
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.cast?view=net-6.0
  let inline cast<'T> (src: IEnumerable) = src.Cast<'T>()

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.chunk?view=net-6.0
  let inline chunk<'T> (size: int) (src: seq<'T>) = src.Chunk size

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.concat?view=net-6.0
  let inline concat (fst: seq<'T>) (snd: seq<'T>) = fst.Concat snd

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.contains?view=net-6.0
  let inline contains (target: 'T) (src: seq<'T>) = src.Contains target
  
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.count?view=net-6.0
  let inline count<'T> (src: seq<'T>) =
    match src with
    | :? ICollection as xs -> xs.Count
    | :? ICollection<'T> as xs -> xs.Count
    | :? IReadOnlyCollection<'T> as xs -> xs.Count
    | _ -> src.Count()
  let inline count'<'T> ([<InlineIfLambda>]predicate: 'T -> bool) (src: seq<'T>) = Enumerable.Count (src, predicate)
  
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.defaultifempty?view=net-6.0
  let inline defaultIfEmpty (src: seq<'T>) = src.DefaultIfEmpty()
  let inline defaultIfEmpty' (defaultValue: 'T) (src: seq<'T>) = src.DefaultIfEmpty defaultValue

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.distinct?view=net-6.0
  let inline distinct<'T> (src: seq<'T>) = src.Distinct()
  let inline distinct'<'T> (comparer: IEqualityComparer<'T>) (src: seq<'T>) = src.Distinct comparer

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.distinctby?view=net-6.0
  let inline distinctBy<'T, 'U> (selector: 'T -> 'U) (src: seq<'T>) = src.DistinctBy selector
  let inline distinctBy'<'T, 'U> (selector: 'T -> 'U) (comparer: IEqualityComparer<'U>) (src: seq<'T>) = src.DistinctBy(selector, comparer)

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.elementat?view=net-6.0
  let inline elementAt<'T> (index: int) (src: seq<'T>) =
    match src with
    | :? IList<'T> as xs -> xs[index]
    | :? IReadOnlyList<'T> as xs -> xs[index]
    | _ -> src.ElementAt index
  
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.elementatordefault?view=net-6.0
  let inline elementAtOrDefault<'T> (index: int) (src: seq<'T>) =
    match src with
    | :? IList<'T> as xs -> if index < 0 || xs.Count <= index then Unchecked.defaultof<'T> else xs[index]
    | :? IReadOnlyList<'T> as xs -> if index < 0 || xs.Count <= index then Unchecked.defaultof<'T> else xs[index]
    | _ -> src.ElementAtOrDefault index

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.empty?view=net-6.0
  let inline empty<'T> () = Enumerable.Empty<'T>()

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.except?view=net-6.0
  let inline except<'T> (fst: seq<'T>) (snd: seq<'T>) = fst.Except snd
  let inline except'<'T> (comparer: IEqualityComparer<'T>) (fst: seq<'T>) (snd: seq<'T>) = fst.Except (snd, comparer)
  
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.except?view=net-6.0
  let inline exceptBy<'T, 'U> (selector: 'T -> 'U) (fst: seq<'T>) (snd: seq<'U>) = fst.ExceptBy(snd, selector)
  let inline exceptBy'<'T, 'U> (selector: 'T -> 'U) (comparer: IEqualityComparer<'U>)  (fst: seq<'T>) (snd: seq<'U>) = fst.ExceptBy(snd, selector, comparer)

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.first?view=net-6.0
  let inline first<'T> (src: seq<'T>) =
    match src with
    | :? IList<'T> as xs -> xs[0]
    | :? IReadOnlyList<'T> as xs -> xs[0]
    | _ -> src.First()
  let inline first'<'T> (predicate: 'T -> bool) (src: seq<'T>) = src.First predicate

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.firstordefault?view=net-6.0
  let inline firstOrDefault<'T> (src: seq<'T>) = src.FirstOrDefault()
  let inline firstOrDefault'<'T> (predicate: 'T -> bool) (src: seq<'T>) = src.FirstOrDefault predicate
  let inline firstOrDefaultWith<'T> (defaultValue: 'T) (src: seq<'T>) = src.FirstOrDefault(defaultValue)
  let inline firstOrDefaultWith'<'T> (defaultValue: 'T) (predicate: 'T -> bool) (src: seq<'T>) = src.FirstOrDefault(predicate, defaultValue)

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.groupby?view=net-6.0
  let inline groubBy< ^Source, ^Key, ^Element, ^Result> (keySelector: ^Source -> ^Key) (resultSelector: ^Key -> seq< ^Source> -> ^Result) (source: seq< ^Source>) =
    source.GroupBy(keySelector, resultSelector)
  let inline groubBy'< ^Source, ^Key, ^Element, ^Result> (keySelector: ^Source -> ^Key) (resultSelector: ^Key -> seq< ^Source> -> ^Result) (comparer: IEqualityComparer< ^Key>) (source: seq< ^Source>) =
    source.GroupBy(keySelector, resultSelector, comparer)
  let inline groubByElement< ^Source, ^Key, ^Element, ^Result> (keySelector: ^Source -> ^Key) (elementSelector: ^Source -> ^Element) (source: seq< ^Source>) =
    source.GroupBy(keySelector, elementSelector)
  let inline groubByElement'< ^Source, ^Key, ^Element, ^Result> (keySelector: ^Source -> ^Key) (elementSelector: ^Source -> ^Element) (comparer: IEqualityComparer< ^Key>) (source: seq< ^Source>) =
    source.GroupBy(keySelector, elementSelector, comparer)
  let inline groubBy2< ^Source, ^Key, ^Element, ^Result> (keySelector: ^Source -> ^Key) (elementSelector: ^Source -> ^Element) (resultSelector: ^Key -> seq< ^Element> -> ^Result) (source: seq< ^Source>) =
    source.GroupBy(keySelector, elementSelector, resultSelector)
  let inline groubBy2'< ^Source, ^Key, ^Element, ^Result> (keySelector: ^Source -> ^Key) (elementSelector: ^Source -> ^Element) (resultSelector: ^Key -> seq< ^Element> -> ^Result) (comparer: IEqualityComparer< ^Key>) (source: seq< ^Source>) =
    source.GroupBy(keySelector, elementSelector, resultSelector, comparer)

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.groupjoin?view=net-6.0
  let inline groupJoin< ^Outer, ^Inner, ^Key, ^Result> (inner: seq< ^Inner>)  (outerKeySelector: ^Outer -> ^Key) (innerKeySelector: ^Inner -> ^Key) (resultSelector: ^Outer -> seq< ^Inner> -> ^Result) (outer: seq< ^Outer>) =
    outer.GroupJoin(inner, outerKeySelector, innerKeySelector, resultSelector)
  let inline groupJoin'< ^Outer, ^Inner, ^Key, ^Result> (inner: seq< ^Inner>)  (outerKeySelector: ^Outer -> ^Key) (innerKeySelector: ^Inner -> ^Key) (resultSelector: ^Outer -> seq< ^Inner> -> ^Result) (comparer: IEqualityComparer< ^Key>) (outer: seq< ^Outer>) =
    outer.GroupJoin(inner, outerKeySelector, innerKeySelector, resultSelector, comparer)
  let inline groupJoin2< ^Outer, ^Inner, ^Key, ^Result> (outerKeySelector: ^Outer -> ^Key) (innerKeySelector: ^Inner -> ^Key) (resultSelector: ^Outer -> seq< ^Inner> -> ^Result) (outer: seq< ^Outer>, inner: seq< ^Inner>) =
    outer.GroupJoin(inner, outerKeySelector, innerKeySelector, resultSelector)
  let inline groupJoin2'< ^Outer, ^Inner, ^Key, ^Result> (outerKeySelector: ^Outer -> ^Key) (innerKeySelector: ^Inner -> ^Key) (resultSelector: ^Outer -> seq< ^Inner> -> ^Result) (comparer: IEqualityComparer< ^Key>) (outer: seq< ^Outer>, inner: seq< ^Inner>) =
    outer.GroupJoin(inner, outerKeySelector, innerKeySelector, resultSelector, comparer)

  // (TODO) Intersect
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.intersect?view=net-6.0

  // (TODO) IntersectBy
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.intersectby?view=net-6.0

  // (TODO) Join
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.join?view=net-6.0


  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.last?view=net-6.0
  let inline last<'T> (src: seq<'T>) =
    match src with
    | :? IList<'T> as xs -> xs[xs.Count - 1]
    | :? IReadOnlyList<'T> as xs -> xs[xs.Count - 1]
    | _ -> src.Last()
  let inline last'<'T> (predicate: 'T -> bool) (src: seq<'T>) = src.Last predicate
  
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.lastordefault?view=net-6.0
  let inline lastOrDefault<'T> (src: seq<'T>) = src.LastOrDefault()
  let inline lastOrDefault'<'T> (predicate: 'T -> bool) (src: seq<'T>) = src.LastOrDefault predicate
  let inline lastOrDefaultWith<'T> (defaultValue: 'T) (src: seq<'T>) = src.LastOrDefault(defaultValue)
  let inline lastOrDefaultWith'<'T> (defaultValue: 'T) (predicate: 'T -> bool) (src: seq<'T>) = src.LastOrDefault(predicate, defaultValue)

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.longcount?view=net-6.0
  let inline longCount<'T> (src: seq<'T>) : int64 =
    match src with
    | :? ICollection as xs -> xs.Count
    | :? ICollection<'T> as xs -> xs.Count
    | :? IReadOnlyCollection<'T> as xs -> xs.Count
    | _ -> src.LongCount()
  let inline longCount'<'T> ([<InlineIfLambda>]predicate: 'T -> bool) (src: seq<'T>) = src.LongCount predicate
  
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
      let iter = src.GetEnumerator()
      if iter.MoveNext() then
        let mutable v = iter.Current
        while iter.MoveNext() do
          let c = iter.Current
          if v < c then v <- c
        v
      else
        Unchecked.defaultof< ^T>

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.maxby?view=net-6.0
  let inline maxBy ([<InlineIfLambda>]selector: 'source -> 'key) (src: seq<'source>) = src.MaxBy(selector)
  let inline maxBy' ([<InlineIfLambda>]selector: 'source -> 'key) (comparer: IComparer<'key>) (src: seq<'source>) = src.MaxBy(selector, comparer)
  
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
      let iter = src.GetEnumerator()
      if iter.MoveNext() then
        let mutable v = iter.Current
        while iter.MoveNext() do
          let c = iter.Current
          if c < v then v <- c
        v
      else
        Unchecked.defaultof< ^T>

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.minby?view=net-6.0
  let inline minBy ([<InlineIfLambda>]selector: 'source -> 'key) (src: seq<'source>) = src.MinBy(selector)
  let inline minBy' ([<InlineIfLambda>]selector: 'source -> 'key) (comparer: IComparer<'key>) (src: seq<'source>) = src.MinBy(selector, comparer)

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.oftype?view=net-6.0
  let inline ofType<'T> (src: IEnumerable) = src.OfType<'T>()
  
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.orderby?view=net-6.0
  let inline orderBy ([<InlineIfLambda>]selector: 'source -> 'key) (src: seq<'source>) = src.OrderBy(selector)
  let inline orderBy' ([<InlineIfLambda>]selector: 'source -> 'key) (comparer: IComparer<'key>) (src: seq<'source>) = src.OrderBy(selector, comparer)

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.orderbydescending?view=net-6.0
  let inline orderByDescending ([<InlineIfLambda>]selector: 'source -> 'key) (src: seq<'source>) = src.OrderByDescending(selector)
  let inline orderByDescending' ([<InlineIfLambda>]selector: 'source -> 'key) (comparer: IComparer<'key>) (src: seq<'source>) = src.OrderByDescending(selector, comparer)

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.prepend?view=net-6.0
  let inline prepend(src: seq<'source>, element: 'source) = src.Prepend(element)

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.reverse?view=net-6.0
  let inline reverse(src: seq<'source>) = src.Reverse()

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.select?view=net-6.0
  let inline select< ^source, ^result> ([<InlineIfLambda>] selector: ^source -> ^result) (source: seq< ^source>) : seq< ^result> =
    match source with
    | :? array< ^source> as ary -> ary.Select selector //SelectArrayIterator.create selector ary
    | :? ResizeArray< ^source> as ls -> ls.Select selector // SelectListIterator.create selector ls
    | :? list< ^source> as ls -> SelectFsListIterator.create selector ls
    | _ -> source.Select selector //SelectEnumerableIterator.create selector source

  //let inline select<'T, 'U> ([<InlineIfLambda>]selector: 'T -> 'U) (src: seq<'T>): seq<'U> = src.Select selector
  let inline select'<'T, 'U> ([<InlineIfLambda>]selector: 'T -> int -> 'U) (src: seq<'T>): seq<'U> = src.Select selector

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.selectmany?view=net-6.0
  let inline selectMany ([<InlineIfLambda>]selector: 'source -> seq<'result>) (src: seq<'source>) = src.SelectMany(selector)
  let inline selectMany' ([<InlineIfLambda>]selector: 'source -> int -> seq<'result>) (src: seq<'source>) = src.SelectMany(selector)
  let inline selectMany2 ([<InlineIfLambda>]resultSelector: 'source -> 'collection -> 'result) ([<InlineIfLambda>]collectionSelector: 'source -> seq<'collection>) (src: seq<'source>) = src.SelectMany(collectionSelector, resultSelector)
  let inline selectMany2' ([<InlineIfLambda>]resultSelector: 'source -> 'collection -> 'result) ([<InlineIfLambda>]collectionSelector: 'source -> int -> seq<'collection>) (src: seq<'source>) = src.SelectMany(collectionSelector, resultSelector)

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.sequenceequal?view=net-6.0
  let inline sequenceEqual (snd: seq<'source>) (fst: seq<'source>) = fst.SequenceEqual(snd)
  let inline sequenceEqual' (comparer: IEqualityComparer<'source>) (snd: seq<'source>) (fst: seq<'source>) = fst.SequenceEqual(snd, comparer)

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.single?view=net-6.0
  let inline single (src: seq<'source>) = src.Single()
  let inline single' ([<InlineIfLambda>]predicate: 'source -> bool) (src: seq<'source>) = src.Single(predicate)

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.singleordefault?view=net-6.0
  let inline singleOrDefault (src: seq<'source>) = src.SingleOrDefault()
  let inline singleOrDefault' ([<InlineIfLambda>]predicate: 'source -> bool) (src: seq<'source>) = src.SingleOrDefault(predicate)
  let inline singleOrDefaultWith (defaultValue: 'source) (src: seq<'source>) = src.SingleOrDefault(defaultValue)
  let inline singleOrDefaultWith' (defaultValue: 'source) ([<InlineIfLambda>]predicate: 'source -> bool) (src: seq<'source>) = src.SingleOrDefault(predicate, defaultValue)

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.skip?view=net-6.0
  let inline skip (count: int) (src: seq<'source>) = src.Skip(count)

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.skiplast?view=net-6.0
  let inline skipLast (count: int) (src: seq<'source>) = src.SkipLast(count)

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.skipwhile?view=net-6.0
  let inline skipWhile ([<InlineIfLambda>]predicate: 'source -> bool) (src: seq<'source>) = src.SkipWhile(predicate)
  let inline skipWhile' ([<InlineIfLambda>]predicate: 'source -> int -> bool) (src: seq<'source>) = src.SkipWhile(predicate)

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
      let iter = src.GetEnumerator()
      if iter.MoveNext() then
        let mutable acc = iter.Current
        while iter.MoveNext() do
          acc <- acc + iter.Current
        acc
      else
        Unchecked.defaultof< ^T>

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.take?view=net-6.0
  let inline take (count: int) (src: seq<'source>) = src.Take(count)
  let inline take' (range: System.Range) (src: seq<'source>) = src.Take(range)

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.takelast?view=net-6.0
  let inline takeLast (count: int) (src: seq<'source>) = src.TakeLast(count)
  
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.takewhile?view=net-6.0
  let inline takeWhile ([<InlineIfLambda>]predicate: 'source -> bool) (src: seq<'source>) = src.TakeWhile(predicate)
  let inline takeWhile' ([<InlineIfLambda>]predicate: 'source -> int -> bool) (src: seq<'source>) = src.TakeWhile(predicate)

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.thenby?view=net-6.0
  let inline thenBy ([<InlineIfLambda>]selector: 'source -> 'key) (src: IOrderedEnumerable<'source>) = src.ThenBy(selector)
  let inline thenBy' (comparer: IComparer<'key>) ([<InlineIfLambda>]selector: 'source -> 'key) (src: IOrderedEnumerable<'source>) = src.ThenBy(selector, comparer)

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.thenbydescending?view=net-6.0
  let inline thenByDescending([<InlineIfLambda>]selector: 'source -> 'key) (src: IOrderedEnumerable<'source>) = src.ThenByDescending (selector)
  let inline thenByDescending' (comparer: IComparer<'key>) ([<InlineIfLambda>]selector: 'source -> 'key) (src: IOrderedEnumerable<'source>) = src.ThenByDescending (selector, comparer)

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.tolookup?view=net-6.0
  let inline toLookup ([<InlineIfLambda>]selector: 'source -> 'key) (src: seq<'source>) = src.ToLookup(selector)
  let inline toLookup' (comparer: IEqualityComparer<'key>) ([<InlineIfLambda>]selector: 'source -> 'key) (src: seq<'source>) = src.ToLookup(selector, comparer)
  let inline toLookup2 ([<InlineIfLambda>]elementSelector: 'source -> 'key) ([<InlineIfLambda>]keySelector: 'source -> 'key) (src: seq<'source>) = src.ToLookup(keySelector, elementSelector)
  let inline toLookup2' (comparer: IEqualityComparer<'key>) ([<InlineIfLambda>]elementSelector: 'source -> 'key) ([<InlineIfLambda>]keySelector: 'source -> 'key) (src: seq<'source>) = src.ToLookup(keySelector, elementSelector, comparer)

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.trygetnonenumeratedcount?view=net-6.0
  let inline tryGetNonEnumeratedCount (count: outref<int>) (src: seq<'source>) = src.TryGetNonEnumeratedCount(&count)

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.union?view=net-6.0
  let inline union (snd: seq<'T>) (fst: seq<'T>) = fst.Union snd
  let inline union' (comparer: IEqualityComparer<'T>) (snd: seq<'T>) (fst: seq<'T>) = fst.Union(snd, comparer)

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.unionby?view=net-6.0
  let inline unionBy ([<InlineIfLambda>]selector: 'source -> 'key) (snd: seq<'source>) (fst: seq<'source>) = fst.UnionBy (snd, selector)
  let inline unionBy' (comparer: IEqualityComparer<'key>) ([<InlineIfLambda>]selector: 'source -> 'key) (snd: seq<'source>) (fst: seq<'source>) = fst.UnionBy (snd, selector, comparer)

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

  let inline where'< ^T> ([<InlineIfLambda>]predicate: ^T -> int -> bool) (src: seq< ^T>) =
    let mutable i = -1
    seq {
      for v in src do
        i <- i + 1
        if predicate v i then
          yield v
    }

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.zip?view=net-6.0
  let inline zip (snd: seq<'fst>) (fst: seq<'snd>) = fst.Zip(snd)
  let inline zip' (snd: seq<'snd>) ([<InlineIfLambda>]selector: 'fst -> 'snd -> 'result) (fst: seq<'fst>) = fst.Zip(snd, selector)
  let inline zip3 (snd: seq<'snd>) (thd: seq<'thd>) (fst: seq<'fst>) = fst.Zip(snd, thd)