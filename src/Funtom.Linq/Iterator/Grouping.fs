namespace rec Funtom.Linq.Iterator

open System.Collections
open System.Collections.Generic
open Funtom.Linq
open Funtom.Linq.Interfaces

// https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/Grouping.cs#L50
type Grouping<'Key, 'Element> (key: 'Key, hashCode: int) =
  let mutable elements = Array.zeroCreate<'Element>(1)
  let mutable hashNext = defaultof<Grouping<'Key, 'Element>>
  let mutable next = defaultof<Grouping<'Key, 'Element>>
  let mutable count = 0

  member __.Add (element: 'Element) =
    if elements.Length = count then
      System.Array.Resize(&elements, check (*) count 2)
    elements[count] <- element
    count <- count + 1

  member __.Trim () =
    if elements.Length <> count then
      System.Array.Resize(&elements, count)

  member __.GetEnumerator() : IEnumerator<'Element> =
    seq { for i in 0..(count - 1) -> elements[i] }
    |> (fun xs -> xs.GetEnumerator())
    
  abstract member Key : 'Key with get
  default __.Key with get() = key
  member __.Count with get() = count
  member __.IsReadOnly with get() = true

  interface IEnumerable with member __.GetEnumerator () = __.GetEnumerator ()
  interface IEnumerable<'Element> with member __.GetEnumerator () = __.GetEnumerator ()
  interface IGrouping<'Key, 'Element> with member __.Key with get() = __.Key
  interface ICollection<'Element> with 
    member __.Count with get() = __.Count
    member __.IsReadOnly with get() = __.IsReadOnly
    member __.Add(item) = raise (System.NotSupportedException "")
    member __.Clear() = raise (System.NotSupportedException "")
    member __.Contains(item) = 0 <= System.Array.IndexOf(elements, item, 0, count)
    member __.CopyTo(array, index) = System.Array.Copy(elements, 0, array, index, count)
    member __.Remove(item) = raise (System.NotSupportedException "")
  interface IList<'Element> with
    member __.IndexOf(item) = System.Array.IndexOf(elements, item, 0, count)
    member __.Insert(index, item) = raise (System.NotSupportedException "")
    member __.RemoveAt(index) = raise (System.NotSupportedException "")
    member __.Item 
      with get index = if index < 0 || count <= index then raise (System.ArgumentOutOfRangeException "") else elements[index]
      and set index value = raise (System.NotSupportedException "")
    
// https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/Lookup.cs#L65
type Lookup<'Key, 'Element> private (comparer: IEqualityComparer<'Key>) =
  let mutable groupings = Array.zeroCreate<Grouping<'Key, 'Element>>(7)
  let mutable lastGrouping = defaultof<Grouping<'Key, 'Element>>
  let mutable count = 0

  abstract member Key : 'Key with get
  default __.Key with get() = key
  member __.Count with get() = count
  member __.IsReadOnly with get() = true

  interface IEnumerable with member __.GetEnumerator () = __.GetEnumerator ()
  interface IEnumerable<'Element> with member __.GetEnumerator () = __.GetEnumerator ()
  interface ILookup<'Key, 'Element> with
    