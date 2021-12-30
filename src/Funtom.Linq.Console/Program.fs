open Funtom.Linq
open System.Linq
open System.Collections.Generic
open Funtom.Linq.Core
open System.Runtime.CompilerServices
open System
open System.Runtime.InteropServices
open FSharp.Linq.RuntimeHelpers


let inline eval q = LeafExpressionConverter.EvaluateQuotation q

//seq { 0..9 } |> Seq.iter (fun x -> printf $"{x} ")
//printfn ""
//Enumerable.Range(0, 10) 
//|> Seq.iter (fun x -> printf $"{x} ")

let is_even v = v % 2 = 0

//let xs = 
//  [0;1;2;3;4;5;]
//  |> Seq.map ((%) <| 2)
//  |> Seq.map ((*) 2)
//  |> Seq.toArray

//let xs = 
//  ResizeArray([| 0 .. 10 |]) |> Core.where is_even |> Linq.toArray

//let xs =
//  [| 0..10 |] |> Core.where is_even |> Linq.toArray

//let xs =
//  [ 0..10 ] 
//  |> Linq.where' (fun v i -> 
//    printfn $"%d{i}: %d{v}"
//    v % 2 = 0)
//  |> Linq.toArray

//printfn "----------"
//let ys =
//  [ 0..10 ].Where(fun v i -> 
//    printfn $"%d{i}: %d{v}"
//    v % 2 = 0).ToArray()

//printfn "%A" xs

//let tmp = 0

////[| 0..10 |]
////ResizeArray([| 0 .. 10 |]) 
//[ 0..10 ]
//|> Linq.select ((*) 2)
//|> Linq.select ((*) 2)
//|> Seq.iter (printfn "%d")

//Linq.sum [0..10]
//|> printfn "%d"

//Linq.sum [0.0..0.2..1.0]
//|> printfn "%f"

//let xs : int[] = [||] 
//xs.Sum() |> printfn "%d"


let xs = [ 0 .. 10000 ]
let ys = [| 0 .. 10000 |]
let zs = ResizeArray([| 0 .. 10000 |])
let ss = seq { 0 .. 10000 }

xs |> Seq.max |> printfn "%d"
ys |> Seq.max |> printfn "%d"
zs |> Seq.max |> printfn "%d"
ss |> Seq.max |> printfn "%d"