namespace Funtom.Linq.Iterator

open Funtom.Linq
open System.Collections
open System.Collections.Generic

module ElementAt =
  open Basis
  open Interfaces
  open Enumerable

  // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/ElementAt.cs#L128
  let tryGetElement<'T> (source: seq<'T>, index: int) =
    let mutable out = defaultof<'T>
    if 0 <= index
    then
      use e = source.GetEnumerator()
      let rec loop (i: int) =
        if e.MoveNext()
        then
          if i = 0 then out <- e.Current; true else loop(i - 1) 
        else
          false
      let r = loop (index)
      (r, out)
    else
      (false, out)