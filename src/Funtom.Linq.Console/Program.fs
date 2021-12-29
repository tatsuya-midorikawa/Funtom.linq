open Funtom.Linq
open System.Linq
open System.Collections.Generic
open Funtom.Linq.Core
open System.Runtime.CompilerServices
open System
open System.Runtime.InteropServices

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

//[| 0..10 |]
//ResizeArray([| 0 .. 10 |]) 
[ 0..10 ]
|> Core.select ((*) 2)
|> Core.select ((*) 2)
|> Seq.iter (printfn "%d")


//type R () =
//  member __.MoveNext() = true

//type T () =
//  member __.GetEnumerator() = R()

//type R' () =
//  member __.MoveNext() = true

//type T' () =
//  member __.GetEnumerator() = R()
  
//type T'' () =
//  member __.GetEnumerator() = R'()

//let inline fx< ^T, ^R when ^T : (member GetEnumerator: unit -> ^R) and ^R : (member MoveNext: unit -> bool)> (v: ^T) : ^R =
//  (^T: (member GetEnumerator: unit -> ^R) v)


//let a = fx (T())
//let b = fx (T'())
//let c = fx (T''())