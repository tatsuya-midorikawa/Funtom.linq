module Tests

open System
open FsCheck.Xunit
open Funtom.Linq

module Common =
    let inline filterPredicate x = x % 2 = 0
    let inline mapFn x = x / 2
    let mapStrFn (x:string) = x.Length
    let mapTwoStrFn (x: string, y: string) = x.Length * y.Length
    let inline folder state curr = state + curr


[<Property>]
let ``Reverse of reverse of a list is the original list ``(xs:int list) =
    let revRev = Linq.reverse(Linq.reverse xs)
    (revRev |> List.ofSeq) = xs
  

[<Property>]
let ``Where then select on a list produce the same result``(xs:int list) =
    let a = xs |> List.filter Common.filterPredicate |> List.map Common.mapFn
    let b = xs |> Linq.where Common.filterPredicate |> Linq.select Common.mapFn |> List.ofSeq
    a = b
    
[<Property>]
let ``Map string list works the same`` (xs: string list) =
    let a = xs |> List.map Common.mapStrFn
    let b = xs |> Linq.select Common.mapStrFn |> List.ofSeq
    a = b
    

[<Property>]
let ``Fold int list works the same`` (xs: int list) =
    let a = xs |> List.fold Common.folder -1
    let b = xs |> Linq.aggregate -1 Common.folder
    a = b

[<Property>]
let ``Fold string list works the same`` (xs: string list) =
    let a = xs |> List.fold Common.folder "a"
    let b = xs |> Linq.aggregate "a" Common.folder
    a = b
    

[<Property>]
let ``Reverse of reverse of an array is the original array ``(xs:int array) =
    let revRev = Linq.reverse(Linq.reverse xs)
    (revRev |> Array.ofSeq) = xs
  

[<Property>]
let ``Where then select on a array produce the same result``(xs:int array) =
    let a = xs |> Array.filter Common.filterPredicate |> Array.map Common.mapFn
    let b = xs |> Linq.where Common.filterPredicate |> Linq.select Common.mapFn |> Array.ofSeq
    a = b
    
[<Property>]
let ``Map string array works the same`` (xs: string array) =
    let a = xs |> Array.map Common.mapStrFn
    let b = xs |> Linq.select Common.mapStrFn |> Array.ofSeq
    a = b
    

[<Property>]
let ``Fold int array works the same`` (xs: int array) =
    let a = xs |> Array.fold Common.folder -1
    let b = xs |> Linq.aggregate -1 Common.folder
    a = b

[<Property>]
let ``Fold string array works the same`` (xs: string array) =
    let a = xs |> Array.fold Common.folder "a"
    let b = xs |> Linq.aggregate "a" Common.folder
    a = b
    
