namespace Funtom.linq.iterator

open Funtom.linq
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
      let mutable i = index
      let rec loop () =
        if e.MoveNext()
        then
          if i <> 0 then i <- i - 1; loop() else out <- e.Current; true
        else
          false
      (loop (), out)
    else
      (false, out)
