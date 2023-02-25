open Funtom.linq
open System.Linq
open System.Collections
open System.Collections.Generic
open Funtom.linq.Core
open System.Runtime.CompilerServices
open System
open System.Runtime.InteropServices
open FSharp.Linq.RuntimeHelpers
open System.Diagnostics


let xs = [ 1..10000 ]
let ys = [| 1..10000 |]
let zs = ResizeArray([| 1..10000 |])
let ss = [| 1..10000 |] |> Seq.ofArray

// [| 0..10 |]
// |> Array.map (fun x -> x * 2)
// |> printfn "%A"

// [| 0..10 |]
// |> Linq2.select (fun x -> x * 2)
// |> (fun x -> x.ToArray())
// |> printfn "%A"



//let mutable n = 0
//let f () =
//  seq {
//    n <- n + 1
//    yield n
//    while n < 10 do
//      n <- n + 1
//      yield n      
//  }

//f()
//|> Seq.toArray
//|> printfn "%A"

//xs.ElementAt 200 |> printfn "%d"
//xs |> Linq.elementAt 200 |> printfn "%d"
//printfn ""
//ys.ElementAt 200 |> printfn "%d"
//ys |> Linq.elementAt 200 |> printfn "%d"
//printfn ""
//zs.ElementAt 200 |> printfn "%d"
//zs |> Linq.elementAt 200 |> printfn "%d"
//printfn ""
//ss.ElementAt 200 |> printfn "%d"
//ss |> Linq.elementAt 200 |> printfn "%d"

//[0..10]
//|> Linq.except [5]
//|> Linq.toArray
//|> printfn "%A"

//[0..10].Except([5]).ToArray()
//|> printfn "%A"

//[| {| Name="aaa"; Age=10 |}; {| Name="bbb"; Age=10 |}; {| Name="ccc"; Age=20 |} |]
//|> Linq.exceptBy ([| 0..10 |], fun x -> x.Age)
//|> Linq.toArray
//|> printfn "%A"

//[| {| Name="aaa"; Age=10 |}; {| Name="bbb"; Age=10 |}; {| Name="ccc"; Age=20 |} |].ExceptBy([| 0..10 |], fun x -> x.Age).ToArray()
//|> printfn "%A"

//[0..10]
//|> Linq.intersect [5]
//|> Linq.toArray
//|> printfn "%A"

//[0..10].intersect([5]).ToArray()
//|> printfn "%A"


//[0..10]
//|> Linq.reverse
//|> Linq.toArray
//|> printfn "%A"

//[0..10].Reverse().ToArray()
//|> printfn "%A"


//type Type = Rock = 0 | Gas = 1 | Liquid = 2 | Ice = 3
//type Planet = { Name: string; Type: Type }

//let xs = ResizeArray<Planet>([|
//  { Name = "Mercury"; Type = Type.Rock }
//  { Name = "Venus"; Type = Type.Rock }
//  { Name = "Mars"; Type = Type.Rock }
//  { Name = "Earth"; Type = Type.Rock }
//  { Name = "Jupiter"; Type = Type.Gas }
//  { Name = "Saturn"; Type = Type.Gas }
//  { Name = "Uranus"; Type = Type.Liquid }
//  { Name = "Pluto"; Type = Type.Ice }
//|])

//xs.DistinctBy (fun x -> x.Type)
//|> Seq.iter (printfn "%A")

//printfn ""

//xs
//|> Linq.distinctBy (fun x -> x.Type)
//|> Seq.iter (printfn "%A ")

//printfn "---"

//xs.DistinctBy(fun x -> x.Type).ToArray()
//|> Seq.iter (printfn "%A")

//printfn ""

//xs
//|> Linq.distinctBy (fun x -> x.Type)
//|> Linq.toArray
//|> Seq.iter (printfn "%A ")



//let xs = [21; 46; 46; 55; 17; 21; 55; 55;]
//let xs = [| 21; 46; 46; 55; 17; 21; 55; 55; |]
//let xs = ResizeArray<int>([| 21; 46; 46; 55; 17; 21; 55; 55; |])

//xs.Distinct()
//|> Seq.iter (printf "%d ")

//printfn ""

//xs
//|> Linq.distinct
//|> Seq.iter (printf "%d ")


//let xs : int[] = [||]

//xs.DefaultIfEmpty(10)
//|> printfn "%A"

//xs
//|> Linq.defaultIfEmpty' 10
//|> printfn "%A"

//[ 0..10 ]
//|> Linq.concat [ 11..20 ]
//|> Linq.toArray
//|> printfn "%A"

//let a = [| 0..10 |]
//a
//|> Linq.concat a
//|> Linq.toArray
//|> printfn "%A"

//a
//|> Linq.contains 10
//|> printfn "%b"

//a
//|> Linq.contains 11
//|> printfn "%b"

//ResizeArray [| 0..10 |]
//|> Linq.concat (ResizeArray [| 11..20 |])
//|> Linq.toArray
//|> printfn "%A"

//seq { 0..10 }
//|> Linq.concat (seq { 11..20 })
//|> Linq.toArray
//|> printfn "%A"


//match box(1,"", 2.0) with
//| :? (int * string * double) -> true
//| _ -> false
//|> printfn "%b"

//match (box 1,box "", box 2.0) with
//| (:? int), (:? string), (:? double) -> true
//| _ -> false
//|> printfn "%b"

//let xs = [ 0..10 ]
//xs
//|> Linq.append 1
//|> Linq.append 2
//|> Linq.append 3
//|> Linq.toArray
//|> printfn "%A"

//xs
//|> Linq.prepend 1
//|> Linq.prepend 2
//|> Linq.prepend 3
//|> Linq.toArray
//|> printfn "%A"

//seq { 0..10 }
////|> Linq.select (fun v -> v * 2)
//|> Linq.select (fun v -> v / 2)
//|> Linq.toArray
//|> printfn "%A"


//seq { 0..97 }
//|> Linq.chunk 5
//|> Seq.iter (fun xs -> xs |> Seq.iter (printf "%d, "); printfn "")


//xs.Prepend(1).Prepend(2).Prepend(3).ToArray()
//|> printfn "%A"


//let inline eval q = LeafExpressionConverter.EvaluateQuotation q

////seq { 0..9 } |> Seq.iter (fun x -> printf $"{x} ")
////printfn ""
////Enumerable.Range(0, 10) 
////|> Seq.iter (fun x -> printf $"{x} ")

//let is_even v = v % 2 = 0

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


//[| 0..10 |].Append(11).ToArray() |> printfn "%A"
//[| 0..10 |] |> Linq.append 11 |> Linq.toArray |> printfn "%A"

//1 <<< 1 |> printfn "%d"

//Checked.(+) 1 1
//|> printfn "%d"