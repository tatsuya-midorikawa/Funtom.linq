namespace Funtom.Linq.Iterator

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

  interface IEnumerable with member __.GetEnumerator () = __.GetEnumerator ()
  interface IEnumerable<'Element> with member __.GetEnumerator () = __.GetEnumerator ()
  interface IGrouping<'Key, 'Element> with member __.Key with get() = __.Key
  // TODO
  interface ICollection<'Element> with 
    member __.Count with get() = __.Count