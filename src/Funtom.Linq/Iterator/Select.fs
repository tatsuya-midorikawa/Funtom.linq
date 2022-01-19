namespace Funtom.Linq.Iterator

open System.Collections
open System.Collections.Generic
open Basis

module Select =

  type SelectEnumerableIterator<'T, 'U> (source: seq<'T>, [<InlineIfLambda>]selector: 'T -> 'U) =
    inherit Iterator<'U>()
    member val internal enumerator = Unchecked.defaultof<IEnumerator<'T>> with get, set