namespace Funtom.Linq.Iterator

module Chunk =
  type ChunkIterator<'T>(src: seq<'T>, size: int) =
    
