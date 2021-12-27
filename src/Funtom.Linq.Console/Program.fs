open Funtom.Linq
open System.Linq

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

let xs =
  [ 0..10 ] 
  |> Core.wherei (fun v i -> 
    printfn $"%d{i}: %d{v}"
    v % 2 = 0)
  |> Linq.toArray

printfn "----------"
let ys =
  [ 0..10 ].Where(fun v i -> 
    printfn $"%d{i}: %d{v}"
    v % 2 = 0).ToArray()

printfn "%A" xs

let tmp = 0