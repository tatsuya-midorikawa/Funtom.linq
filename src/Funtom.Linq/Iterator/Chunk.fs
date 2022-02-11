namespace Funtom.Linq.Iterator

open System.Collections
open System.Collections.Generic

module Chunk =
  let chunkIterator<'T>(src: seq<'T>, size: int) =
    if size < 1 then
      raise (System.ArgumentOutOfRangeException "size")
    use e = src.GetEnumerator()
    seq {
      while e.MoveNext() do
        let mutable chunk = Array.zeroCreate size
        chunk[0] <- e.Current
        let mutable i = 1
        while i < size do
          if not (e.MoveNext()) then
            System.Array.Resize(&chunk, i)
            i <- size
          else
            chunk[i] <- e.Current
            i <- i + 1
        yield chunk
    }
