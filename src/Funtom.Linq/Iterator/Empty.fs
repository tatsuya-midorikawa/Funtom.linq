namespace Funtom.Linq.Iterator

open System
open System.Collections
open System.Collections.Generic
open Funtom.Linq.Common.Interfaces


module Empty =
  type EmptyPartition<'T> private () =
    static member Instance with get() = new EmptyPartition<'T>()

    member __.GetEnumerator() = __
    member __.MoveNext() = false
    member __.Current with get() = Unchecked.defaultof<'T>
    member __.Reset() = ()
    member __.Dispose() = ()
    member __.Skip(count: int) = __
    member __.Take(count: int) = __
    member __.TryGetElementAt(index: int, found: outref<bool>) = found <- false; Unchecked.defaultof<'T>
    member __.TryGetFirst(found: outref<bool>) = found <- false; Unchecked.defaultof<'T>
    member __.TryGetLast(found: outref<bool>) = found <- false; Unchecked.defaultof<'T>
    member __.ToArray() = System.Array.Empty<'T>()
    member __.ToList() = ResizeArray<'T>()
    member __.GetCount(onlyIfCheap: bool) = 0

    interface IDisposable with
      member __.Dispose() = __.Dispose()

    interface IEnumerator with
      member __.MoveNext() = __.MoveNext()
      member __.Current with get() = __.Current
      member __.Reset() = __.Reset()
    
    interface IEnumerator<'T> with
      member __.Current with get() = __.Current

    interface IEnumerable with
      member __.GetEnumerator() = __.GetEnumerator()

    interface IEnumerable<'T> with
      member __.GetEnumerator() = __.GetEnumerator()
      
    interface IListProvider<'T> with
      member __.ToArray() = __.ToArray()
      member __.ToList() = __.ToList()
      member __.GetCount(onlyIfCheap: bool) = __.GetCount(onlyIfCheap)

    interface IPartition<'T> with
      member __.Skip(count: int) = __.Skip(count)
      member __.Take(count: int) = __.Take(count)
      member __.TryGetElementAt(index: int, found: outref<bool>) = __.TryGetElementAt(index, &found)
      member __.TryGetFirst(found: outref<bool>) = __.TryGetFirst(&found)
      member __.TryGetLast(found: outref<bool>) = __.TryGetLast(&found)