namespace Funtom.Linq

open System

module Span =

  let inline sum (span: Span< ^T>) =
    let mutable iter = span.GetEnumerator()
    if iter.MoveNext() then
      let mutable acc = iter.Current
      while iter.MoveNext() do
        acc <- acc + iter.Current
      acc
    else
      Unchecked.defaultof< ^T>