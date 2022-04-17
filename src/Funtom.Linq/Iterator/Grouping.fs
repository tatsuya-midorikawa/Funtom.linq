namespace Funtom.Linq.Iterator

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

  // TODO
  member __.GetEnumerator() =
    ()