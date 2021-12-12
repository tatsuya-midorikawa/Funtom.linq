open Funtom.Linq
open System.Linq

seq { 0..9 } |> Seq.iter (fun x -> printf $"{x} ")
printfn ""
Enumerable.Range(0, 10) |> Seq.iter (fun x -> printf $"{x} ")