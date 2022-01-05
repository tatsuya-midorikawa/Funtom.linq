open Funtom.Linq
open System.Linq
open System.Collections
open System.Collections.Generic
open Funtom.Linq.Core
open System.Runtime.CompilerServices
open System
open System.Runtime.InteropServices
open FSharp.Linq.RuntimeHelpers
open System.Diagnostics


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


//let xs = [ 0 .. 10000 ]
//let ys = [| 0 .. 10000 |]
//let zs = ResizeArray([| 0 .. 10000 |])
//let ss = seq { 0 .. 10000 }

//xs |> Linq.min |> printfn "%d"
//ys |> Linq.min |> printfn "%d"
//zs |> Linq.min |> printfn "%d"
//ss |> Linq.min |> printfn "%d"

//xs |> Linq.max |> printfn "%d"
//ys |> Linq.max |> printfn "%d"
//zs |> Linq.max |> printfn "%d"
//ss |> Linq.max |> printfn "%d"

//let xs = [ 0 .. 10000 ]
//let ys = [| 0 .. 10000 |]
//let zs = ResizeArray([| 0 .. 10000 |])
//let ss = seq { 0 .. 10000 }

//xs |> Seq.min |> printfn "%d"
//ys |> Seq.min |> printfn "%d"
//zs |> Seq.min |> printfn "%d"
//ss |> Seq.min |> printfn "%d"

////let xs = ResizeArray([| 0 .. 10 |]) 
//let xs = [| 0 .. 10 |]

//xs
//  .Where((<) 5)
//  //.Select((*) 2)
//|> printfn "%A"

//xs
////|> Linq.where ((<) 5)
//|> Linq.select0 ((*) 2)
//|> Linq.select0 ((*) 2)
//|> Linq.toArray
//|> printfn "%A"

//let inline S f g x = (f x) (g x)
//let inline K x y = x
//let inline I x = x
//let rec Y f = 
//  printfn $"%d{StackTrace().FrameCount}"
//  f (fun x -> Y f x)

//let fib40 =
//  Y (fun fib f1 f2 n -> if n = 0 then f1 else fib f2 (f1 + f2) (n - 1)) 0 1 40
  
//let inline fn< ^T, ^U, ^V, ^V2, ^V3
//  when ^T: (member GetEnumerator: unit -> ^U)
//  and ^U: (member MoveNext: unit -> bool)
//  and ^U: (member get_Current: unit -> ^V)> (f: ^V -> ^V2) (xs: ^T) =
//  Y (fun g x -> if (^U: (member MoveNext: unit -> bool) x) then true else false ) xs

//  //fun (f': ^V2 -> ^V3) ->
//  //  let iter = (^T: (member GetEnumerator: unit -> ^U) xs)
//  //  let acc = ResizeArray< ^V3> []
//  //  while (^U: (member MoveNext: unit -> bool) iter) do
//  //    let v = (^U: (member get_Current: unit -> ^V) iter)
//  //    acc.Add(f' (f v))
//  //  acc :> seq< ^V3>

//let xs = ArrayList([| 10 :> obj; "test"; 20.5; 11; 20; "foo" |])
//xs
//|> Core.ofType<int>
//|> Seq.iter (fun x -> printfn "%d" x)
//printfn "---"
//xs
//|> Linq.ofType<int>
//|> Seq.iter (fun x -> printfn "%d" x)

//let xs = ArrayList([| ""; ""; ""; ""; ""; ""; ""; ""; ""; ""; ""; ""; ""; ""; ""; ""; ""; ""; ""; ""; ""; ""; ""; ""; ""; ""; ""; |])
//let ys = xs |> Core.cast<int> |> Linq.toArray
//let ys = xs.Cast<int>().ToArray()

//let xs = [| 0..10 |]
//ResizeArray [| 0..10 |]
//|> Linq.aggregate 0 (fun v next -> v + next)
//|> printfn "%d"

//[| 0..10 |]
//|> Linq.aggregate 0 (fun v next -> v + next)
//|> printfn "%d"

//[ 0..10 ]
//|> Linq.aggregate 0 (fun v next -> v + next)
//|> printfn "%d"

//seq { 0..10 }
//|> Linq.aggregate 0 (fun v next -> v + next)
//|> printfn "%d"


//xs.Aggregate (fun v next -> v + next)
//|> printfn "%d"

//[| 0..5 |].All(fun x -> x < 0) |> printfn "%b"
//[| 0..5 |] |> Linq.all (fun x -> x < 0) |> printfn "%b"


[| 0..10 |].Append(11).ToArray() |> printfn "%A"
[| 0..10 |] |> Linq.append 11 |> Linq.toArray |> printfn "%A"