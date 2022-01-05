namespace Funtom.Linq

open System
open System.Collections.Generic

module Enumerable =
  /// <summary>
  /// 
  /// </summary>
  /// <see href="https://github.com/JonHanna/corefx/blob/f0d3761d8e875f6fa17e43ab715b746bbcb68716/src/Common/src/System/Collections/Generic/EnumerableHelpers.cs">EnumerableHelpers.ToArray()</see>
  let inline toArray< ^T>(source: seq< ^T>) =
    match source with
    | :? ICollection< ^T> as collection ->
      let count = collection.Count
      if count <> 0 then
        let acc = Array.create count Unchecked.defaultof< ^T>
        collection.CopyTo(acc, 0)
        acc
      else
        Array.empty< ^T>
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
        Array.empty< ^T>