namespace Funtom.Linq

open System
open System.Collections.Generic
open Funtom.Linq.Common.Interfaces

module Enumerable =

  /// <summary>
  /// 
  /// </summary>
  /// <see href="https://github.com/JonHanna/corefx/blob/f0d3761d8e875f6fa17e43ab715b746bbcb68716/src/Common/src/System/Collections/Generic/EnumerableHelpers.cs">EnumerableHelpers.ToArray()</see>
  let inline toArray<'T> (source: seq<'T>) =
    match source with
    | :? ICollection<'T> as collection ->
      let count = collection.Count
      if count <> 0 then
        let acc = Array.zeroCreate count
        collection.CopyTo(acc, 0)
        acc
      else
        Array.empty<'T>
    | _ ->
      use iter = source.GetEnumerator()
      if iter.MoveNext() then
        let maxArrayLength = Array.MaxLength  
        let defaultCapacity = 8
        let mutable acc = Array.create defaultCapacity Unchecked.defaultof< ^T>
        acc[0] <- iter.Current
        let mutable count = 1
        while iter.MoveNext() do
          if count = acc.Length then
            let mutable newLength = count <<< 1
            if uint newLength > uint maxArrayLength then
              newLength <- if maxArrayLength <= count then count + 1 else maxArrayLength
            Array.Resize(&acc, newLength)
          acc[count] <- iter.Current
          count <- count + 1
        Array.Resize(&acc, count)
        acc
      else
        Array.empty<'T>

  /// <summary>
  /// 
  /// </summary>
  /// <see href="https://github.com/JonHanna/corefx/blob/master/src/Common/src/System/Collections/Generic/EnumerableHelpers.Linq.cs#L22">bool TryGetCount<T>(IEnumerable<T> source, out int count)</see>
  let inline tryGetCount<'T> (source: seq<'T>, count: outref<int>) =
    match source with
    | :? ICollection<'T> as collection ->
      count <- collection.Count
      true
    | :? IListProvider<'T> as provider ->
      count <- provider.GetCount(true)
      0 <= count
    | _ ->
      count <- -1
      false
      
  /// <summary>
  /// 
  /// </summary>
  /// <see href="https://github.com/JonHanna/corefx/blob/master/src/Common/src/System/Collections/Generic/EnumerableHelpers.Linq.cs#L74">void IterativeCopy<T>(IEnumerable<T> source, T[] array, int arrayIndex, int count)</see>
  let inline iterativeCopy<'T> (source: seq<'T>, array: array<'T>, arrayIndex: int, count: int) =
    let mutable index = arrayIndex
    let endIndex = arrayIndex + count
    for v in source do
      array[index] <- v
      index <- index + 1
    assert(arrayIndex = endIndex)
      
  /// <summary>
  /// 
  /// </summary>
  /// <see href="https://github.com/JonHanna/corefx/blob/master/src/Common/src/System/Collections/Generic/EnumerableHelpers.Linq.cs#L49">void Copy<T>(IEnumerable<T> source, T[] array, int arrayIndex, int count)</see>
  let inline copy<'T> (source: seq<'T>, array: 'T[], arrayIndex: int, count: int) =
    match source with
    | :? ICollection<'T> as collection -> collection.CopyTo(array, arrayIndex)
    | _ -> iterativeCopy(source, array, arrayIndex, count)

