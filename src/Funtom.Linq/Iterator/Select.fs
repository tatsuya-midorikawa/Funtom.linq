namespace Funtom.Linq.Iterator

open System.Collections
open System.Collections.Generic
open Basis

//module Select =
//  // src: https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/Select.cs#L98
//  type SelectEnumerableIterator<'T, 'U> (source: seq<'T>, [<InlineIfLambda>]selector: 'T -> 'U) =
//    inherit Iterator<'U>()
//    member val internal enumerator = Unchecked.defaultof<IEnumerator<'T>> with get, set
    