﻿namespace rec Funtom.Linq.Iterator

open System.Collections.Generic
open Funtom.Linq

[<AutoOpen>]
module OrderedEnumerable =
  // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/OrderedEnumerable.cs#L240
  [<AbstractClass>]
  type EnumerableSorter<'element> () =
    // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/OrderedEnumerable.cs#L242
    abstract member ComputeKeys : ('element[] * int) -> unit
    // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/OrderedEnumerable.cs#L244
    abstract member CompareAnyKeys : (int * int) -> int
    // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/OrderedEnumerable.cs#L246
    member private __.ComputeMap (elements: 'element[], count: int) : int[] =
      __.ComputeKeys(elements, count)
      let map = Array.zeroCreate<int>(count)
      for i = 0 to map.Length - 1 do
        map[i] <- i
      map
    // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/OrderedEnumerable.cs#L258
    member __.Sort (elements: 'element[], count: int) : int[] =
      let map = __.ComputeMap(elements, count)
      __.QuickSort(map, 0, count - 1)
      map
    // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/OrderedEnumerable.cs#L265
    member __.Sort (elements: 'element[], count: int, min: int, max: int) : int[] =
      let map = __.ComputeMap(elements, count)
      __.PartialQuickSort(map, 0, count - 1, min, max)
      map
    // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/OrderedEnumerable.cs#L272
    member __.ElementAt (elements: 'element[], count: int, idx: int) : 'element =
      let map = __.ComputeMap(elements, count)
      if idx = 0
      then elements[__.Min(map, count)]
      else elements[__.QuickSelect(map, count - 1, idx)]
    // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/OrderedEnumerable.cs#L280
    abstract member QuickSort : (int[] * int * int) -> unit
    // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/OrderedEnumerable.cs#L284
    abstract member PartialQuickSort : (int[] * int * int * int * int) -> unit
    // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/OrderedEnumerable.cs#L288
    abstract member QuickSelect : (int[] * int * int) -> int
    // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/OrderedEnumerable.cs#L290
    abstract member Min : (int[] * int) -> int

  // WIP: 実装中
  // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/OrderedEnumerable.cs#L293
  type EnumerableSorter<'element, 'key> (keySelector: 'element -> 'key, comparer: IComparer<'key>, descending: bool, ?next: EnumerableSorter<'element>) =
    inherit EnumerableSorter<'element> ()


  // WIP
  // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/OrderedEnumerable.cs#L11
  [<AbstractClass>]
  type OrderedEnumerable<'element> (src: seq<'element>) =
    
  