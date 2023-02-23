namespace Funtom.Linq

open System
open System.Collections
open System.Collections.Generic
open System.Runtime.CompilerServices

// [NOT IMPLEMENTED]
[<Extension>]
type LinqExtensions =
  static member private foo = ()
  // [<Extension>]
  // static member inline aggregate(src: seq<'source>, [<InlineIfLambda>]fx: 'source -> 'source -> 'source) = 
  //   src.Aggregate(fx)
  // [<Extension>]
  // static member inline aggregate(src: seq<'source>, seed: 'accumlate, [<InlineIfLambda>]fx: 'accumlate -> 'source -> 'accumlate) = 
  //   src.Aggregate(seed, fx)
  // [<Extension>]
  // static member inline aggregate(src: seq<'source>, seed: 'accumlate, [<InlineIfLambda>]fx: 'accumlate -> 'source -> 'accumlate, [<InlineIfLambda>]selector: 'accumlate -> 'result) = 
  //   src.Aggregate(seed, fx, selector)
    
  // [<Extension>]
  // static member inline all(src: seq<'source>, [<InlineIfLambda>]predicate: 'source -> bool) = src.All(predicate)
    
  // [<Extension>]
  // static member inline any(src: seq<'source>) = src.Any()
  // [<Extension>]
  // static member inline any(src: seq<'source>, [<InlineIfLambda>]predicate: 'source -> bool) = src.Any(predicate)
  
  // [<Extension>]
  // static member inline append(src: seq<'source>, element: 'source) = src.Append(element)
    
  // [<Extension>]
  // static member inline asEnumerable(src: seq<'source>) = src.AsEnumerable()
    
  // [<Extension>]
  // static member inline average(src: seq<single>) = src.Average()
  // [<Extension>]
  // static member inline average(src: seq<double>) = src.Average()
  // [<Extension>]
  // static member inline average(src: seq<int>) = src.Average()
  // [<Extension>]
  // static member inline average(src: seq<int64>) = src.Average()
  // [<Extension>]
  // static member inline average(src: seq<decimal>) = src.Average()
  // [<Extension>]
  // static member inline average(src: seq<Nullable<single>>) =src.Average()
  // [<Extension>]
  // static member inline average(src: seq<Nullable<double>>) = src.Average()
  // [<Extension>]
  // static member inline average(src: seq<Nullable<int>>) = src.Average()
  // [<Extension>]
  // static member inline average(src: seq<Nullable<int64>>) = src.Average()
  // [<Extension>]
  // static member inline average(src: seq<Nullable<decimal>>) = src.Average()
  // [<Extension>]
  // static member inline average(src: seq<'source>, [<InlineIfLambda>]selector: 'source -> single) = src.Average(selector)
  // [<Extension>]
  // static member inline average(src: seq<'source>, [<InlineIfLambda>]selector: 'source -> double) = src.Average(selector)
  // [<Extension>]
  // static member inline average(src: seq<'source>, [<InlineIfLambda>]selector: 'source -> int) = src.Average(selector)
  // [<Extension>]
  // static member inline average(src: seq<'source>, [<InlineIfLambda>]selector: 'source -> int64) = src.Average(selector)
  // [<Extension>]
  // static member inline average(src: seq<'source>, [<InlineIfLambda>]selector: 'source -> decimal) = src.Average(selector)
  // [<Extension>]
  // static member inline average(src: seq<'source>, [<InlineIfLambda>]selector: 'source -> Nullable<single>) = src.Average(selector)
  // [<Extension>]
  // static member inline average(src: seq<'source>, [<InlineIfLambda>]selector: 'source -> Nullable<double>) = src.Average(selector)
  // [<Extension>]
  // static member inline average(src: seq<'source>, [<InlineIfLambda>]selector: 'source -> Nullable<int>) = src.Average(selector)
  // [<Extension>]
  // static member inline average(src: seq<'source>, [<InlineIfLambda>]selector: 'source -> Nullable<int64>) = src.Average(selector)
  // [<Extension>]
  // static member inline average(src: seq<'source>, [<InlineIfLambda>]selector: 'source -> Nullable<decimal>) = src.Average(selector)
  
  // [<Extension>]
  // static member inline cast<'result>(src: IEnumerable) = src.Cast<'result>()
  
  // [<Extension>]
  // static member inline chunk(src: seq<'source>, size: int) = src.Chunk(size)
  
  // [<Extension>]
  // static member inline concat(fst: seq<'source>, snd: seq<'source>) = fst.Concat(snd)
  
  // [<Extension>]
  // static member inline contains(src: seq<'source>, target: 'source) = src.Contains(target)
  // [<Extension>]
  // static member inline contains(src: seq<'source>, target: 'source, comparer: IEqualityComparer<'source>) = src.Contains(target, comparer)
  
  // [<Extension>]
  // static member inline count(src: seq<'source>) = Linq.count src
  // [<Extension>]
  // static member inline count(src: seq<'source>, [<InlineIfLambda>]predicate: 'source -> bool) = Linq.count' predicate src
  
  // [<Extension>]
  // static member inline defaultIfEmpty(src: seq<'source>) = src.DefaultIfEmpty()
  // [<Extension>]
  // static member inline defaultIfEmpty(src: seq<'source>, target: 'source) = src.DefaultIfEmpty(target)
  
  // [<Extension>]
  // static member inline distinct(src: seq<'source>) = src.Distinct()
  // [<Extension>]
  // static member inline distinct(src: seq<'source>, comparer: IEqualityComparer<'source>) = src.Distinct(comparer)
  
  // [<Extension>]
  // static member inline distinctBy(src: seq<'source>, [<InlineIfLambda>]selector: 'source -> 'key) = src.DistinctBy(selector)
  // [<Extension>]
  // static member inline distinctBy(src: seq<'source>, [<InlineIfLambda>]selector: 'source -> 'key, comparer: IEqualityComparer<'key>) = src.DistinctBy(selector, comparer)
  
  // [<Extension>]
  // static member inline elementAt(src: seq<'source>, index: Index) = src.ElementAt index
  // [<Extension>]
  // static member inline elementAt(src: seq<'source>, index: int) = Linq.elementAt index src
  
  // [<Extension>]
  // static member inline elementAtOrDefault(src: seq<'source>, index: Index) = src.ElementAtOrDefault index
  // [<Extension>]
  // static member inline elementAtOrDefault(src: seq<'source>, index: int) = Linq.elementAtOrDefault index src
  
  // [<Extension>]
  // static member inline except(fst: seq<'source>, snd: seq<'source>) = fst.Except(snd)
  // [<Extension>]
  // static member inline except(fst: seq<'source>, snd: seq<'source>, comparer: IEqualityComparer<'source>) = fst.Except(snd, comparer)
  
  // [<Extension>]
  // static member inline exceptBy(fst: seq<'source>, snd: seq<'key>, [<InlineIfLambda>]selector: 'source -> 'key) = fst.ExceptBy(snd, selector)
  // [<Extension>]
  // static member inline exceptBy(fst: seq<'source>, snd: seq<'key>, [<InlineIfLambda>]selector: 'source -> 'key, comparer: IEqualityComparer<'key>) = fst.ExceptBy(snd, selector, comparer)
  
  // [<Extension>]
  // static member inline first(src: seq<'source>) = Linq.first src
  // [<Extension>]
  // static member inline first(src: seq<'source>, [<InlineIfLambda>]predicate: 'source -> bool) = Linq.first' predicate src

  // [<Extension>]
  // static member inline firstOrDefault(src: seq<'source>) = src.FirstOrDefault()
  // [<Extension>]
  // static member inline firstOrDefault(src: seq<'source>, [<InlineIfLambda>]predicate: 'source -> bool) = src.FirstOrDefault(predicate)
  // [<Extension>]
  // static member inline firstOrDefault(src: seq<'source>, defaultValue: 'source) = src.FirstOrDefault(defaultValue)
  // [<Extension>]
  // static member inline firstOrDefault(src: seq<'source>, [<InlineIfLambda>]predicate: 'source -> bool, defaultValue: 'source) = src.FirstOrDefault(predicate, defaultValue)

  // [<Extension>]
  // static member inline groupBy(src: seq<'source>, [<InlineIfLambda>]keySelector: 'source -> 'key, [<InlineIfLambda>]elementSelector: 'source -> 'element, [<InlineIfLambda>]resultSelector: 'key -> seq<'element> -> 'result, comparer: IEqualityComparer<'key>) = 
  //   src.GroupBy(keySelector, elementSelector, resultSelector, comparer)
  // [<Extension>]
  // static member inline groupBy(src: seq<'source>, [<InlineIfLambda>]keySelector: 'source -> 'key, [<InlineIfLambda>]elementSelector: 'source -> 'element, [<InlineIfLambda>]resultSelector: 'key -> seq<'element> -> 'result) = 
  //   src.GroupBy(keySelector, elementSelector, resultSelector)
  // [<Extension>]
  // static member inline groupBy(src: seq<'source>, [<InlineIfLambda>]keySelector: 'source -> 'key, [<InlineIfLambda>]elementSelector: 'source -> 'element) = 
  //   src.GroupBy(keySelector, elementSelector)
  // [<Extension>]
  // static member inline groupBy(src: seq<'source>, [<InlineIfLambda>]keySelector: 'source -> 'key, [<InlineIfLambda>]elementSelector: 'source -> 'element, comparer: IEqualityComparer<'key>) = 
  //   src.GroupBy(keySelector, elementSelector, comparer)
  // [<Extension>]
  // static member inline groupBy(src: seq<'source>, [<InlineIfLambda>]keySelector: 'source -> 'key, [<InlineIfLambda>]resultSelector: 'key -> seq<'source> -> 'result, comparer: IEqualityComparer<'key>) = 
  //   src.GroupBy(keySelector, resultSelector, comparer)
  // [<Extension>]
  // static member inline groupBy(src: seq<'source>, [<InlineIfLambda>]keySelector: 'source -> 'key, [<InlineIfLambda>]resultSelector: 'key -> seq<'source> -> 'result) = 
  //   src.GroupBy(keySelector, resultSelector)
  // [<Extension>]
  // static member inline groupBy(src: seq<'source>, [<InlineIfLambda>]keySelector: 'source -> 'key) = 
  //   src.GroupBy(keySelector)
  
  // [<Extension>]
  // static member inline groupBy(src: seq<'source>, [<InlineIfLambda>]keySelector: 'source -> 'key, comparer: IEqualityComparer<'key>) = 
  //   src.GroupBy(keySelector, comparer)
  
  // [<Extension>]
  // static member inline groupJoin(outer: seq<'outer>, inner: seq<'inner>, [<InlineIfLambda>]outerSelector: 'outer -> 'key, [<InlineIfLambda>]innerSelector: 'inner -> 'key, [<InlineIfLambda>]resultSelector: 'outer -> seq<'inner> -> 'result) = 
  //   outer.GroupJoin(inner, outerSelector, innerSelector, resultSelector)
  // [<Extension>]
  // static member inline groupJoin(outer: seq<'outer>, inner: seq<'inner>, [<InlineIfLambda>]outerSelector: 'outer -> 'key, [<InlineIfLambda>]innerSelector: 'inner -> 'key, [<InlineIfLambda>]resultSelector: 'outer -> seq<'inner> -> 'result, comparer: IEqualityComparer<'key>) = 
  //   outer.GroupJoin(inner, outerSelector, innerSelector, resultSelector, comparer)

  // [<Extension>]
  // static member inline intersect(fst: seq<'source>, snd: seq<'source>) = fst.Intersect(snd)
  // [<Extension>]
  // static member inline intersect(fst: seq<'source>, snd: seq<'source>, comparer: IEqualityComparer<'source>) = fst.Intersect(snd, comparer)

  // [<Extension>]
  // static member inline intersectBy(fst: seq<'source>, snd: seq<'key>, [<InlineIfLambda>]selector: 'source -> 'key) = fst.IntersectBy(snd, selector)
  // [<Extension>]
  // static member inline intersectBy(fst: seq<'source>, snd: seq<'key>, [<InlineIfLambda>]selector: 'source -> 'key, comparer: IEqualityComparer<'key>) = fst.IntersectBy(snd, selector, comparer)

  // [<Extension>]
  // static member inline join(outer: seq<'outer>, inner: seq<'inner>, [<InlineIfLambda>]outerSelector: 'outer -> 'key, [<InlineIfLambda>]innerSelector: 'inner -> 'key, [<InlineIfLambda>]resultSelector: 'outer -> 'inner -> 'result) = 
  //   outer.Join(inner, outerSelector, innerSelector, resultSelector)
  // [<Extension>]
  // static member inline join(outer: seq<'outer>, inner: seq<'inner>, [<InlineIfLambda>]outerSelector: 'outer -> 'key, [<InlineIfLambda>]innerSelector: 'inner -> 'key, [<InlineIfLambda>]resultSelector: 'outer -> 'inner -> 'result, comparer: IEqualityComparer<'key>) = 
  //   outer.Join(inner, outerSelector, innerSelector, resultSelector, comparer)
  
  // [<Extension>]
  // static member inline last(src: seq<'source>) = Linq.last src
  // [<Extension>]
  // static member inline last(src: seq<'source>, [<InlineIfLambda>]predicate: 'source -> bool) = Linq.last' predicate src

  // [<Extension>]
  // static member inline lastOrDefault(src: seq<'source>) = src.LastOrDefault()
  // [<Extension>]
  // static member inline lastOrDefault(src: seq<'source>, [<InlineIfLambda>]predicate: 'source -> bool) = src.LastOrDefault(predicate)
  // [<Extension>]
  // static member inline lastOrDefault(src: seq<'source>, defaultValue: 'source) = src.LastOrDefault(defaultValue)
  // [<Extension>]
  // static member inline lastOrDefault(src: seq<'source>, [<InlineIfLambda>]predicate: 'source -> bool, defaultValue: 'source) = src.LastOrDefault(predicate, defaultValue)
  
  // [<Extension>]
  // static member inline longCount(src: seq<'source>) = Linq.longCount src
  // [<Extension>]
  // static member inline longCount(src: seq<'source>, [<InlineIfLambda>]predicate: 'source -> bool) = Linq.longCount' predicate src

  // [<Extension>]
  // static member inline max(src: seq<single>) = src.Max()
  // [<Extension>]
  // static member inline max(src: seq<double>) = src.Max()
  // [<Extension>]
  // static member inline max(src: seq<int>) = src.Max()
  // [<Extension>]
  // static member inline max(src: seq<int64>) = src.Max()
  // [<Extension>]
  // static member inline max(src: seq<decimal>) = src.Max()
  // [<Extension>]
  // static member inline max(src: seq<Nullable<single>>) = src.Max()
  // [<Extension>]
  // static member inline max(src: seq<Nullable<double>>) = src.Max()
  // [<Extension>]
  // static member inline max(src: seq<Nullable<int>>) = src.Max()
  // [<Extension>]
  // static member inline max(src: seq<Nullable<int64>>) = src.Max()
  // [<Extension>]
  // static member inline max(src: seq<Nullable<decimal>>) = src.Max()
  // [<Extension>]
  // static member inline max(src: seq<'source>, [<InlineIfLambda>]selector: 'source -> single) = src.Max(selector)
  // [<Extension>]
  // static member inline max(src: seq<'source>, [<InlineIfLambda>]selector: 'source -> double) = src.Max(selector)
  // [<Extension>]
  // static member inline max(src: seq<'source>, [<InlineIfLambda>]selector: 'source -> int) = src.Max(selector)
  // [<Extension>]
  // static member inline max(src: seq<'source>, [<InlineIfLambda>]selector: 'source -> int64) = src.Max(selector)
  // [<Extension>]
  // static member inline max(src: seq<'source>, [<InlineIfLambda>]selector: 'source -> decimal) = src.Max(selector)
  // [<Extension>]
  // static member inline max(src: seq<'source>, [<InlineIfLambda>]selector: 'source -> Nullable<single>) = src.Max(selector)
  // [<Extension>]
  // static member inline max(src: seq<'source>, [<InlineIfLambda>]selector: 'source -> Nullable<double>) = src.Max(selector)
  // [<Extension>]
  // static member inline max(src: seq<'source>, [<InlineIfLambda>]selector: 'source -> Nullable<int>) = src.Max(selector)
  // [<Extension>]
  // static member inline max(src: seq<'source>, [<InlineIfLambda>]selector: 'source -> Nullable<int64>) = src.Max(selector)
  // [<Extension>]
  // static member inline max(src: seq<'source>, [<InlineIfLambda>]selector: 'source -> Nullable<decimal>) = src.Max(selector)
  // [<Extension>]
  // static member inline max(src: seq<'source>, [<InlineIfLambda>]selector: 'source -> 'result) = src.Max(selector)
  // [<Extension>]
  // static member inline max(src: seq<'source>, comparer: IComparer<'source>) = src.Max(comparer)
  
  // [<Extension>]
  // static member inline maxBy(src: seq<'source>, [<InlineIfLambda>]selector: 'source -> 'key) = src.MaxBy(selector)
  // [<Extension>]
  // static member inline maxBy(src: seq<'source>, [<InlineIfLambda>]selector: 'source -> 'key, comparer: IComparer<'key>) = src.MaxBy(selector, comparer)
  
  // [<Extension>]
  // static member inline min(src: seq<single>) = src.Min()
  // [<Extension>]
  // static member inline min(src: seq<double>) = src.Min()
  // [<Extension>]
  // static member inline min(src: seq<int>) = src.Min()
  // [<Extension>]
  // static member inline min(src: seq<int64>) = src.Min()
  // [<Extension>]
  // static member inline min(src: seq<decimal>) = src.Min()
  // [<Extension>]
  // static member inline min(src: seq<Nullable<single>>) = src.Min()
  // [<Extension>]
  // static member inline min(src: seq<Nullable<double>>) = src.Min()
  // [<Extension>]
  // static member inline min(src: seq<Nullable<int>>) = src.Min()
  // [<Extension>]
  // static member inline min(src: seq<Nullable<int64>>) = src.Min()
  // [<Extension>]
  // static member inline min(src: seq<Nullable<decimal>>) = src.Min()
  // [<Extension>]
  // static member inline min(src: seq<'source>, [<InlineIfLambda>]selector: 'source -> single) = src.Min(selector)
  // [<Extension>]
  // static member inline min(src: seq<'source>, [<InlineIfLambda>]selector: 'source -> double) = src.Min(selector)
  // [<Extension>]
  // static member inline min(src: seq<'source>, [<InlineIfLambda>]selector: 'source -> int) = src.Min(selector)
  // [<Extension>]
  // static member inline min(src: seq<'source>, [<InlineIfLambda>]selector: 'source -> int64) = src.Min(selector)
  // [<Extension>]
  // static member inline min(src: seq<'source>, [<InlineIfLambda>]selector: 'source -> decimal) = src.Min(selector)
  // [<Extension>]
  // static member inline min(src: seq<'source>, [<InlineIfLambda>]selector: 'source -> Nullable<single>) = src.Min(selector)
  // [<Extension>]
  // static member inline min(src: seq<'source>, [<InlineIfLambda>]selector: 'source -> Nullable<double>) = src.Min(selector)
  // [<Extension>]
  // static member inline min(src: seq<'source>, [<InlineIfLambda>]selector: 'source -> Nullable<int>) = src.Min(selector)
  // [<Extension>]
  // static member inline min(src: seq<'source>, [<InlineIfLambda>]selector: 'source -> Nullable<int64>) = src.Min(selector)
  // [<Extension>]
  // static member inline min(src: seq<'source>, [<InlineIfLambda>]selector: 'source -> Nullable<decimal>) = src.Min(selector)
  // [<Extension>]
  // static member inline min(src: seq<'source>, [<InlineIfLambda>]selector: 'source -> 'result) = src.Min(selector)
  // [<Extension>]
  // static member inline min(src: seq<'source>, comparer: IComparer<'source>) = src.Min(comparer)
  
  // [<Extension>]
  // static member inline minBy(src: seq<'source>, [<InlineIfLambda>]selector: 'source -> 'key) = src.MinBy(selector)
  // [<Extension>]
  // static member inline minBy(src: seq<'source>, [<InlineIfLambda>]selector: 'source -> 'key, comparer: IComparer<'key>) = src.MinBy(selector, comparer)
  
  // [<Extension>]
  // static member inline ofType<'T>(src: IEnumerable) = src.OfType<'T>()
  
  // [<Extension>]
  // static member inline orderBy(src: seq<'source>, [<InlineIfLambda>]selector: 'source -> 'key) = src.OrderBy(selector)
  // [<Extension>]
  // static member inline orderBy(src: seq<'source>, [<InlineIfLambda>]selector: 'source -> 'key, comparer: IComparer<'key>) = src.OrderBy(selector, comparer)
  
  // [<Extension>]
  // static member inline orderByDescending(src: seq<'source>, [<InlineIfLambda>]selector: 'source -> 'key) = src.OrderByDescending(selector)
  // [<Extension>]
  // static member inline orderByDescending(src: seq<'source>, [<InlineIfLambda>]selector: 'source -> 'key, comparer: IComparer<'key>) = src.OrderByDescending(selector, comparer)

  // [<Extension>]
  // static member inline prepend(src: seq<'source>, element: 'source) = src.Prepend(element)
    
  // [<Extension>]
  // static member inline reverse(src: seq<'source>) = src.Reverse()

  // [<Extension>]
  // static member inline select(src: seq<'source>, [<InlineIfLambda>]selector: 'source -> 'result) = src.Select(selector)
  // [<Extension>]
  // static member inline select(src: seq<'source>, [<InlineIfLambda>]selector: 'source -> int -> 'result) = src.Select(selector)

  // [<Extension>]
  // static member inline selectMany(src: seq<'source>, [<InlineIfLambda>]selector: 'source -> seq<'result>) = src.SelectMany(selector)
  // [<Extension>]
  // static member inline selectMany(src: seq<'source>, [<InlineIfLambda>]selector: 'source -> int -> seq<'result>) = src.SelectMany(selector)
  // [<Extension>]
  // static member inline selectMany(src: seq<'source>, [<InlineIfLambda>]collectionSelector: 'source -> seq<'collection>, [<InlineIfLambda>]resultSelector: 'source -> 'collection -> 'result) = src.SelectMany(collectionSelector, resultSelector)
  // [<Extension>]
  // static member inline selectMany(src: seq<'source>, [<InlineIfLambda>]collectionSelector: 'source -> int -> seq<'collection>, [<InlineIfLambda>]resultSelector: 'source -> 'collection -> 'result) = src.SelectMany(collectionSelector, resultSelector)

  // [<Extension>]
  // static member inline sequenceEqual(fst: seq<'source>, snd: seq<'source>) = fst.SequenceEqual(snd)
  // [<Extension>]
  // static member inline sequenceEqual(fst: seq<'source>, snd: seq<'source>, comparer: IEqualityComparer<'source>) = fst.SequenceEqual(snd, comparer)
  
  // [<Extension>]
  // static member inline single(src: seq<'source>) = src.Single()
  // [<Extension>]
  // static member inline single(src: seq<'source>, [<InlineIfLambda>]predicate: 'source -> bool) = src.Single(predicate)
  
  // [<Extension>]
  // static member inline singleOrDefault(src: seq<'source>) = src.SingleOrDefault()
  // [<Extension>]
  // static member inline singleOrDefault(src: seq<'source>, [<InlineIfLambda>]predicate: 'source -> bool) = src.SingleOrDefault(predicate)
  // [<Extension>]
  // static member inline singleOrDefault(src: seq<'source>, defaultValue: 'source) = src.SingleOrDefault(defaultValue)
  // [<Extension>]
  // static member inline singleOrDefault(src: seq<'source>, [<InlineIfLambda>]predicate: 'source -> bool, defaultValue: 'source) = src.SingleOrDefault(predicate, defaultValue)
  
  // [<Extension>]
  // static member inline skip(src: seq<'source>, count: int) = src.Skip(count)

  // [<Extension>]
  // static member inline skipLast(src: seq<'source>, count: int) = src.SkipLast(count)

  // [<Extension>]
  // static member inline skipWhile(src: seq<'source>, [<InlineIfLambda>]predicate: 'source -> bool) = src.SkipWhile(predicate)
  // [<Extension>]
  // static member inline skipWhile(src: seq<'source>, [<InlineIfLambda>]predicate: 'source -> int -> bool) = src.SkipWhile(predicate)

  // [<Extension>]
  // static member inline sum(src: seq<single>) = src.Sum()
  // [<Extension>]
  // static member inline sum(src: seq<double>) = src.Sum()
  // [<Extension>]
  // static member inline sum(src: seq<int>) = src.Sum()
  // [<Extension>]
  // static member inline sum(src: seq<int64>) = src.Sum()
  // [<Extension>]
  // static member inline sum(src: seq<decimal>) = src.Sum()
  // [<Extension>]
  // static member inline sum(src: seq<Nullable<single>>) = src.Sum()
  // [<Extension>]
  // static member inline sum(src: seq<Nullable<double>>) = src.Sum()
  // [<Extension>]
  // static member inline sum(src: seq<Nullable<int>>) = src.Sum()
  // [<Extension>]
  // static member inline sum(src: seq<Nullable<int64>>) = src.Sum()
  // [<Extension>]
  // static member inline sum(src: seq<Nullable<decimal>>) = src.Sum()
  // [<Extension>]
  // static member inline sum(src: seq<'source>, [<InlineIfLambda>]selector: 'source -> single) = src.Sum(selector)
  // [<Extension>]
  // static member inline sum(src: seq<'source>, [<InlineIfLambda>]selector: 'source -> double) = src.Sum(selector)
  // [<Extension>]
  // static member inline sum(src: seq<'source>, [<InlineIfLambda>]selector: 'source -> int) = src.Sum(selector)
  // [<Extension>]
  // static member inline sum(src: seq<'source>, [<InlineIfLambda>]selector: 'source -> int64) = src.Sum(selector)
  // [<Extension>]
  // static member inline sum(src: seq<'source>, [<InlineIfLambda>]selector: 'source -> decimal) = src.Sum(selector)
  // [<Extension>]
  // static member inline sum(src: seq<'source>, [<InlineIfLambda>]selector: 'source -> Nullable<single>) = src.Sum(selector)
  // [<Extension>]
  // static member inline sum(src: seq<'source>, [<InlineIfLambda>]selector: 'source -> Nullable<double>) = src.Sum(selector)
  // [<Extension>]
  // static member inline sum(src: seq<'source>, [<InlineIfLambda>]selector: 'source -> Nullable<int>) = src.Sum(selector)
  // [<Extension>]
  // static member inline sum(src: seq<'source>, [<InlineIfLambda>]selector: 'source -> Nullable<int64>) = src.Sum(selector)
  // [<Extension>]
  // static member inline sum(src: seq<'source>, [<InlineIfLambda>]selector: 'source -> Nullable<decimal>) = src.Sum(selector)

  // [<Extension>]
  // static member inline take(src: seq<'source>, count: int) = src.Take(count)
  // [<Extension>]
  // static member inline take(src: seq<'source>, range: Range) = src.Take(range)
  
  // [<Extension>]
  // static member inline takeLast(src: seq<'source>, count: int) = src.TakeLast(count)

  // [<Extension>]
  // static member inline takeWhile(src: seq<'source>, [<InlineIfLambda>]predicate: 'source -> bool) = src.TakeWhile(predicate)
  // [<Extension>]
  // static member inline takeWhile(src: seq<'source>, [<InlineIfLambda>]predicate: 'source -> int -> bool) = src.TakeWhile(predicate)

  // [<Extension>]
  // static member inline thenBy(src: IOrderedEnumerable<'source>, [<InlineIfLambda>]selector: 'source -> 'key) = src.ThenBy(selector)
  // [<Extension>]
  // static member inline thenBy(src: IOrderedEnumerable<'source>, [<InlineIfLambda>]selector: 'source -> 'key, comparer: IComparer<'key>) = src.ThenBy(selector, comparer)

  // [<Extension>]
  // static member inline thenByDescending(src: IOrderedEnumerable<'source>, [<InlineIfLambda>]selector: 'source -> 'key) = src.ThenByDescending(selector)
  // [<Extension>]
  // static member inline thenByDescending(src: IOrderedEnumerable<'source>, [<InlineIfLambda>]selector: 'source -> 'key, comparer: IComparer<'key>) = src.ThenByDescending(selector, comparer)
  
  // [<Extension>]
  // static member inline toArray(src: seq<'source>) = src.ToArray()

  // [<Extension>]
  // static member inline toDictionary(src: seq<'source>, [<InlineIfLambda>]selector: 'source -> 'key) = src.ToDictionary(selector)
  // [<Extension>]
  // static member inline toDictionary(src: seq<'source>, [<InlineIfLambda>]selector: 'source -> 'key, comparer: IEqualityComparer<'key>) = src.ToDictionary(selector, comparer)
  // [<Extension>]
  // static member inline toDictionary(src: seq<'source>, [<InlineIfLambda>]keySelector: 'source -> 'key, [<InlineIfLambda>]elementSelector: 'source -> 'element) = src.ToDictionary(keySelector, elementSelector)
  // [<Extension>]
  // static member inline toDictionary(src: seq<'source>, [<InlineIfLambda>]keySelector: 'source -> 'key, [<InlineIfLambda>]elementSelector: 'source -> 'element, comparer: IEqualityComparer<'key>) = src.ToDictionary(keySelector, elementSelector, comparer)
  
  // [<Extension>]
  // static member inline toHashSet(src: seq<'source>) = src.ToHashSet()
  // [<Extension>]
  // static member inline toHashSet(src: seq<'source>, comparer: IEqualityComparer<'source>) = src.ToHashSet(comparer)
  
  // [<Extension>]
  // static member inline toList(src: seq<'source>) = src.ToList()

  // [<Extension>]
  // static member inline toLookup(src: seq<'source>, [<InlineIfLambda>]selector: 'source -> 'key) = src.ToLookup(selector)
  // [<Extension>]
  // static member inline toLookup(src: seq<'source>, [<InlineIfLambda>]selector: 'source -> 'key, comparer: IEqualityComparer<'key>) = src.ToLookup(selector, comparer)
  // [<Extension>]
  // static member inline toLookup(src: seq<'source>, [<InlineIfLambda>]keySelector: 'source -> 'key, [<InlineIfLambda>]elementSelector: 'source -> 'key) = src.ToLookup(keySelector, elementSelector)
  // [<Extension>]
  // static member inline toLookup(src: seq<'source>, [<InlineIfLambda>]keySelector: 'source -> 'key, [<InlineIfLambda>]elementSelector: 'source -> 'key, comparer: IEqualityComparer<'key>) = src.ToLookup(keySelector, elementSelector, comparer)

  // [<Extension>]
  // static member inline tryGetNonEnumeratedCount(src: seq<'source>, count: outref<int>) = src.TryGetNonEnumeratedCount(&count)
  
  // [<Extension>]
  // static member inline union(fst: seq<'source>, snd: seq<'source>) = fst.Union(snd)
  // [<Extension>]
  // static member inline union(fst: seq<'source>, snd: seq<'source>, comparer: IEqualityComparer<'source>) = fst.Union(snd, comparer)

  // [<Extension>]
  // static member inline unionBy(fst: seq<'source>, snd: seq<'source>, [<InlineIfLambda>]selector: 'source -> 'key) = fst.UnionBy(snd, selector)
  // [<Extension>]
  // static member inline unionBy(fst: seq<'source>, snd: seq<'source>, [<InlineIfLambda>]selector: 'source -> 'key, comparer: IEqualityComparer<'key>) = fst.UnionBy(snd, selector, comparer)

  // [<Extension>]
  // static member inline where(src: seq<'source>, [<InlineIfLambda>]predicate: 'source -> bool) = src.Where(predicate)
  // [<Extension>]
  // static member inline where(src: seq<'source>, [<InlineIfLambda>]predicate: 'source -> int -> bool) = src.Where(predicate)
  
  // [<Extension>]
  // static member inline zip(fst: seq<'fst>, snd: seq<'snd>) = fst.Zip(snd)
  // [<Extension>]
  // static member inline zip(fst: seq<'fst>, snd: seq<'snd>, [<InlineIfLambda>]selector: 'fst -> 'snd -> 'result) = fst.Zip(snd, selector)
  // [<Extension>]
  // static member inline zip(fst: seq<'fst>, snd: seq<'snd>, thd: seq<'thd>) = fst.Zip(snd, thd)