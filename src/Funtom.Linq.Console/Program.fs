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
  [ 0..10 ] |> Core.where is_even |> Linq.toArray


printfn "%A" xs

let tmp = 0