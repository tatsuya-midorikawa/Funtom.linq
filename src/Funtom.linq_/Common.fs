namespace Funtom.linq

open System

[<AutoOpen>]
module internal Common = 
  let inline defaultof<'T> = Unchecked.defaultof<'T>
  let inline isDefault<'T when 'T : equality> (src: 'T) = src = Unchecked.defaultof<'T>
  let inline isNotDefault<'T when 'T : equality> (src: 'T) = src <> Unchecked.defaultof<'T>
  let inline check<'T> (op: 'T -> 'T -> 'T) lhs rhs = op lhs rhs

  let inline combine_predicates ([<InlineIfLambda>] p1: 'src -> bool) ([<InlineIfLambda>] p2: 'src -> bool) (x: 'src) = p1 x && p2 x
  let inline combine_selectors ([<InlineIfLambda>] lhs: 'T -> 'U) ([<InlineIfLambda>] rhs: 'U -> 'V) = lhs >> rhs
  let inline get_tid () = Environment.CurrentManagedThreadId