﻿namespace Funtom.linq

[<AutoOpen>]
module internal Common = 
  let inline defaultof<'T> = Unchecked.defaultof<'T>
  let inline isDefault<'T when 'T : equality> (src: 'T) = src = Unchecked.defaultof<'T>
  let inline isNotDefault<'T when 'T : equality> (src: 'T) = src <> Unchecked.defaultof<'T>
  let inline check<'T> (op: 'T -> 'T -> 'T) lhs rhs = op lhs rhs