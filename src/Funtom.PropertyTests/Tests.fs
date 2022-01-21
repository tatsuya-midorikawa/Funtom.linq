module Tests

open System
open FsCheck.Xunit
open Funtom.Linq

module Common =
    let inline filterPredicate x = x % 2 = 0
    let inline mapFn x = x / 2



[<Property>]
let ``Reverse of reverse of a list is the original list ``(xs:int list) =
    let revRev = Linq.reverse(Linq.reverse xs)
    (revRev |> List.ofSeq) = xs
  

[<Property>]
let ``Where then select on a list produce the same result``(xs:int list) =
    let a = xs |> List.filter Common.filterPredicate |> List.map Common.mapFn
    let b = xs |> Linq.where Common.filterPredicate |> Linq.select Common.mapFn |> List.ofSeq
    a = b