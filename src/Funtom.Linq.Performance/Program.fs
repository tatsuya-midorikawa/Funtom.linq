open BenchmarkDotNet.Attributes
open BenchmarkDotNet.Running
open Funtom.Linq
open System.Linq


type Benchmark () =
  //let xs = [ 0 .. 10000 ]

  //[<Benchmark>]
  //member __.Fsharp_A() =
  //  xs.FsWhereA(fun x -> x % 2 = 0).ToArray()

  //[<Benchmark>]
  //member __.Fsharp_B() =
  //  xs.FsWhereB(fun x -> x % 2 = 0).ToArray()

  //[<Benchmark>]
  //member __.Csharp_A() =
  //  xs.WhereA(fun x -> x % 2 = 0).ToArray()

  //[<Benchmark>]
  //member __.Csharp_B() =
  //  xs.WhereB(fun x -> x % 2 = 0).ToArray()

  //[<Benchmark>]
  //member __.Linq() =
  //  xs.Where(fun x -> x % 2 = 0).ToArray()


  //[<Benchmark>]
  //member __.Funtom_first() =
  //  xs |> Linq.first

  //[<Benchmark>]
  //member __.Funtom_method_first() =
  //  xs.first()

  //[<Benchmark>]
  //member __.Linq_First() =
  //  xs.First()

  //[<Benchmark>]
  //member __.Funtom_last() =
  //  xs |> Linq.last

  //[<Benchmark>]
  //member __.Linq_Last() =
  //  xs.Last()

  //[<Benchmark>]
  //member __.Funtom_Extensions_all() =
  //  xs.all(fun x -> x < 2000)

  //[<Benchmark>]
  //member __.Linq_All() =
  //  xs.All(fun x -> x < 2000)

  //[<Benchmark>]
  //member __.Funtom_elementAtOrDefault() =
  //  Linq.elementAtOrDefault 2000 xs

  //[<Benchmark>]
  //member __.Linq_ElementAtOrDefault() =
  //  xs.ElementAtOrDefault(2000)

  //[<Benchmark>]
  //member __.Funtom_contains() =
  //  Linq.contains 999. xs

  //[<Benchmark>]
  //member __.Linq_Contains() =
  //  xs.Contains(999.)

  //[<Benchmark>]
  //member __.Funtom_average() =
  //  Linq.average xs

  //[<Benchmark>]
  //member __.Linq_Average() =
  //  xs.Average()

  //[<Benchmark>]
  //member __.Funtom_count() =
  //  Linq.count xs

  //[<Benchmark>]
  //member __.Linq_Count() =
  //  xs.Count()

  //[<Benchmark>]
  //member __.Fsharp_seq_filter() =
  //  xs
  //  |> Seq.filter (fun x -> x % 2 = 0)
  //  |> Seq.toArray

  //[<Benchmark>]
  //member __.Fsharp_ary_filter() =
  //  xs
  //  |> Array.filter (fun x -> x % 2 = 0)

  //[<Benchmark>]
  //member __.System_ary_findall() =
  //  System.Array.FindAll(xs, fun x -> x % 2 = 0)

  //let xs = [ 0 .. 10000 ]

  //[<Benchmark>]
  //member __.Fsharp_List_filter() =
  //  xs
  //  |> List.filter (fun x -> x % 2 = 0)
  
  //[<Benchmark>]
  //member __.Fsharp_Seq_filter() =
  //  xs
  //  |> Seq.filter (fun x -> x % 2 = 0)
  //  |> Linq.toList

  //[<Benchmark>]
  //member __.Linq_Where() =
  //  xs
  //    .Where(fun x -> x % 2 = 0)
  //    .ToList()

  //let xs = [| 0 .. 10000 |]

  //[<Benchmark>]
  //member __.Fsharp_filter() =
  //  xs
  //  |> Seq.filter (fun x -> x % 2 = 0)
  //  |> Linq.toArray

  //[<Benchmark>]
  //member __.Fsharp_where() =
  //  xs
  //  |> Seq.where (fun x -> x % 2 = 0)
  //  |> Linq.toArray
  
  //let xs = [ 0 .. 10000 ]
  
  let xs = ResizeArray([| 0 .. 10000 |]) 

  //let xs = [| 0 .. 10000 |]
  
  //[<Benchmark>]
  //member __.Fsharp_filter() =
  //  xs
  //  |> Seq.filter (fun x -> x % 2 = 0)
  //  |> Seq.filter (fun x -> x % 3 = 0)
  //  |> Seq.filter (fun x -> x % 5 = 0)
  //  |> Linq.toArray

  [<Benchmark>]
  member __.Funtom_where() =
    xs
    |> Linq.where' (fun x i -> x % 2 = 0)
    //|> Linq.where' (fun x i -> x % 3 = 0)
    //|> Linq.where' (fun x i -> x % 5 = 0)
    |> Linq.toArray

  [<Benchmark>]
  member __.Funtom_Core_where() =
    xs
    |> Core.wherei (fun x i -> x % 2 = 0)
    //|> Core.wherei (fun x i -> x % 3 = 0)
    //|> Core.wherei (fun x i -> x % 5 = 0)
    |> Linq.toArray

  [<Benchmark>]
  member __.Linq_Where() =
    xs
      .Where(fun x i -> x % 2 = 0)
      //.Where(fun x i -> x % 3 = 0)
      //.Where(fun x i -> x % 5 = 0)
      .ToArray()

  //[<Benchmark>]
  //member __.Funtom_wherei() =
  //  xs
  //  |> Linq.wherei (fun x i -> (x * i) % 2 = 0)
  //  |> Linq.toArray

  //[<Benchmark>]
  //member __.Linq_Wherei() =
  //  xs
  //    .Where(fun x i -> (x * i) % 2 = 0)
  //    .ToArray()

  //[<Benchmark>]
  //member __.Funtom_select() =
  //  xs
  //  |> Linq.select (fun x -> x * 2)
  //  |> Linq.toArray

  //[<Benchmark>]
  //member __.Linq_Select() =
  //  xs
  //    .Select(fun x -> x * 2)
  //    .ToArray()

  //[<Benchmark>]
  //member __.Funtom_selecti() =
  //  xs
  //  |> Linq.selecti (fun x i -> x * i)
  //  |> Linq.toArray

  //[<Benchmark>]
  //member __.Linq_Selecti() =
  //  xs
  //    .Select(fun x i -> x * i)
  //    .ToArray()

  //[<Benchmark>]
  //member __.Funtom_all() =
  //  xs |> Linq.all (fun x -> x%2 = 0)

  //[<Benchmark>]
  //member __.Linq_All() =
  //  xs.All(fun x -> x%2 = 0)

  //[<Benchmark>]
  //member __.Funtom_any() =
  //  xs |> Linq.any

  //[<Benchmark>]
  //member __.Linq_Any() =
  //  xs.Any()

  //[<Benchmark>]
  //member __.Funtom_anyfx() =
  //  xs |> Linq.anyfx (fun x -> 500 < x)

  //[<Benchmark>]
  //member __.Linq_AnyFx() =
  //  xs.Any(fun x -> 500 < x)

[<EntryPoint>]
let main args =
  BenchmarkRunner.Run<Benchmark>() |> ignore
  0