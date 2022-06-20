namespace rec Funtom.Linq.Iterator

open System.Collections.Generic
open Funtom.Linq

[<AutoOpen>]
module OrderedEnumerable =
  // WIP
  // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/OrderedEnumerable.cs#L240
  [<AbstractClass>]
  type EnumerableSorter<'element> () =
    // TODO: https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/OrderedEnumerable.cs#L242
    abstract member ComputeKeys : ('element[] * int) -> unit
    // TODO: https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/OrderedEnumerable.cs#L244
    abstract member CompareAnyKeys : (int * int) -> int
    // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/OrderedEnumerable.cs#L246
    member private __.ComputeMap (elements: 'element[], count: int) : int[] =
      __.ComputeKeys(elements, count)
      let map = Array.zeroCreate<int>(count)
      for i = 0 to map.Length - 1 do
        map[i] <- i
      map
    // WIP : 実装中
    // TODO: https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/OrderedEnumerable.cs#L258
    member __.Sort (elemetns: 'element[], count: int) : int[] =
      raise (System.NotImplementedException "")
    // TODO: https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/OrderedEnumerable.cs#L265
    member __.Sort (elemetns: 'element[], count: int, min: int, max: int) : int[] =
      raise (System.NotImplementedException "")
    // TODO: https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/OrderedEnumerable.cs#L272
    member __.ElementAt (elemetns: 'element[], count: int, idx: int) : 'element =
      raise (System.NotImplementedException "")
    // TODO: https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/OrderedEnumerable.cs#L280
    abstract member QuickSort : (int[] * int * int) -> unit
    // TODO: https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/OrderedEnumerable.cs#L284
    abstract member PartialQuickSort : (int[] * int * int * int * int) -> unit
    // TODO: https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/OrderedEnumerable.cs#L288
    abstract member QuickSelect : (int[] * int * int) -> int
    // TODO: https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/OrderedEnumerable.cs#L290
    abstract member Min : (int[] * int) -> int

  // WIP
  // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/OrderedEnumerable.cs#L293
  type EnumerableSorter<'element, 'key> () =

  // WIP
  // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/OrderedEnumerable.cs#L11
  [<AbstractClass>]
  type OrderedEnumerable<'element> (src: seq<'element>) =
    
  