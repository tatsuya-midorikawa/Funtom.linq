namespace rec Funtom.linq.iterator

open System
open System.Collections.Generic
open Funtom.linq

[<AutoOpen>]
module OrderedEnumerable =
  // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/OrderedEnumerable.cs#L240
  [<AbstractClass>]
  type EnumerableSorter<'element> () =
    // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/OrderedEnumerable.cs#L242
    abstract member ComputeKeys : 'element[] * int -> unit
    // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/OrderedEnumerable.cs#L244
    abstract member CompareAnyKeys : int * int -> int
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
    abstract member QuickSort : int[] * int * int -> unit
    // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/OrderedEnumerable.cs#L284
    abstract member PartialQuickSort : int[] * int * int * int * int -> unit
    // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/OrderedEnumerable.cs#L288
    abstract member QuickSelect : int[] * int * int -> int
    // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/OrderedEnumerable.cs#L290
    abstract member Min : int[] * int -> int

  // // [NOT IMPLEMENTED] : WIP : 実装中
  // // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/OrderedEnumerable.cs#L293
  // type EnumerableSorter<'element, 'key> (keySelector: 'element -> 'key, comparer: IComparer<'key>, descending: bool, next: EnumerableSorter<'element>) =
  //   inherit EnumerableSorter<'element> ()
  //   let mutable keys: 'key[] = defaultof<_>


  //   // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/OrderedEnumerable.cs#L309
  //   override __.ComputeKeys (elements: 'element[], count: int) : unit =
  //     keys <- Array.zeroCreate count
  //     for i = 0 to (keys.Length - 1) do
  //       keys[i] <- keySelector(elements[i])
  //     if next <> defaultof<_> then
  //       next.ComputeKeys(elements, count)

  //   // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/OrderedEnumerable.cs#L320
  //   override __.CompareAnyKeys (i: int, j: int) : int =
  //     let c = comparer.Compare(keys[i], keys[j])
  //     if c = 0
  //     then if next  = defaultof<_> then i - j else next.CompareAnyKeys(i, j)
  //     else if descending <> (0 < c) then 1 else -1

  //   // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/OrderedEnumerable.cs#L341
  //   member private __.CompareKeys (i: int, j: int) = if i = j then 0 else __.CompareAnyKeys(i, j)

  //   // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/OrderedEnumerable.cs#L343
  //   override __.QuickSort (keys: int[], lo: int, hi: int) =
  //     Span<int>(keys, lo, hi - lo + 1).Sort(fun i j -> __.CompareAnyKeys(i, j))

  //   // wip
  //   // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/OrderedEnumerable.cs#L348
  //   override __.PartialQuickSort (map: int[], left: int, right: int, min'idx: int, max'idx: int) =
  //     ()

  // // [NOT IMPLEMENTED] : WIP
  // // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/OrderedEnumerable.cs#L11
  // [<AbstractClass>]
  // type OrderedEnumerable<'element> (src: seq<'element>) =
    
  