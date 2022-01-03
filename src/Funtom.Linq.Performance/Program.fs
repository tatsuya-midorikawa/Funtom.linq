open BenchmarkDotNet.Attributes
open BenchmarkDotNet.Running
open Funtom.Linq
open System.Linq
open System
open System.Collections

type Benchmark () =
  let xs = [| 0..10000 |]

  [<Benchmark>]
  member __.System_Linq_all() =
    xs.All(fun x -> x < 10000)

  [<Benchmark>]
  member __.Funtom_Linq_all() =
    xs |> Linq.all (fun x -> x < 10000)



  //let xs = [| 0..10000 |]

  //[<Benchmark>]
  //member __.System_Linq_aggregate() =
  //  xs.Aggregate(fun v next -> v + next)

  //[<Benchmark>]
  //member __.Funtom_Linq_aggregate() =
  //  xs |> Linq.aggregate'' (fun v next -> v + next)


  //let xs = ArrayList([| ""; ""; ""; ""; ""; ""; ""; ""; ""; ""; ""; ""; ""; ""; ""; ""; ""; ""; ""; ""; ""; ""; ""; ""; ""; ""; ""; |])
  //let ys = ArrayList([| 10; 10; 10; 10; 10; 10; 10; 10; 10; 10; 10; 10; 10; 10; 10; 10; 10; 10; 10; 10; 10; 10; 10; 10; 10; 10; 10; |])
  
  //[<Benchmark>]
  //member __.Funtom_Linq_cast_class() =
  //  xs |> Linq.cast<string> |> Linq.toArray

  //[<Benchmark>]
  //member __.Dotnet_Linq_cast_class() =
  //  xs.Cast<string>().ToArray()

  //[<Benchmark>]
  //member __.Funtom_Linq_cast_struct() =
  //  ys |> Linq.cast<int> |> Linq.toArray

  //[<Benchmark>]
  //member __.Dotnet_Linq_cast_struct() =
  //  ys.Cast<int>().ToArray()

  //[<Benchmark>]
  //member __.Funtom_Linq_oftype() =
  //  xs |> Linq.ofType<string> |> Linq.toArray

  //[<Benchmark>]
  //member __.Dotnet_Linq_oftype() =
  //  xs.OfType<string>().ToArray()

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
  
  
  //let xs = ResizeArray([| 0 .. 10000 |]) 

  //let xs = [| 0 .. 10000 |]
  
  //[<Benchmark>]
  //member __.Fsharp_filter() =
  //  xs
  //  |> Seq.filter (fun x -> x % 2 = 0)
  //  |> Seq.filter (fun x -> x % 3 = 0)
  //  |> Seq.filter (fun x -> x % 5 = 0)
  //  |> Linq.toArray

  //[<Benchmark>]
  //member __.Funtom_where() =
  //  xs
  //  |> Linq.where' (fun x i -> x % 2 = 0)
  //  //|> Linq.where' (fun x i -> x % 3 = 0)
  //  //|> Linq.where' (fun x i -> x % 5 = 0)
  //  |> Linq.toArray

  ////[<Benchmark>]
  ////member __.Funtom_Core_where() =
  ////  xs
  ////  |> Core.wherei (fun x i -> x % 2 = 0)
  ////  //|> Core.wherei (fun x i -> x % 3 = 0)
  ////  //|> Core.wherei (fun x i -> x % 5 = 0)
  ////  |> Linq.toArray

  //[<Benchmark>]
  //member __.Linq_Where() =
  //  xs
  //    .Where(fun x i -> x % 2 = 0)
  //    //.Where(fun x i -> x % 3 = 0)
  //    //.Where(fun x i -> x % 5 = 0)
  //    .ToArray()
  
  
  //let xs = [ 0 .. 10000 ]
  //let ys = [| 0 .. 10000 |]
  //let zs = ResizeArray([| 0 .. 10000 |])
  //let ss = seq { 0 .. 10000 }

  //[<Benchmark>]
  //member __.Fsharp_Seq_min_fslist() =
  //  xs |> Seq.min

  //[<Benchmark>]
  //member __.Funtom_Linq_min_fslist() =
  //  xs |> Linq.min

  //[<Benchmark>]
  //member __.Linq_Min_fslist() =
  //  xs.Min()

  //[<Benchmark>]
  //member __.Fsharp_Seq_min_array() =
  //  ys |> Seq.min

  //[<Benchmark>]
  //member __.Funtom_Linq_min_array() =
  //  ys |> Linq.min

  //[<Benchmark>]
  //member __.Linq_Min_array() =
  //  ys.Min()

  //[<Benchmark>]
  //member __.Fsharp_Seq_min_resizearry() =
  //  zs |> Seq.min

  //[<Benchmark>]
  //member __.Funtom_Linq_min_resizearry() =
  //  zs |> Linq.min

  //[<Benchmark>]
  //member __.Linq_Min_resizearry() =
  //  zs.Min()

  //[<Benchmark>]
  //member __.Fsharp_Seq_min_seq() =
  //  ss |> Seq.min

  //[<Benchmark>]
  //member __.Funtom_Linq_min_seq() =
  //  ss |> Linq.min

  //[<Benchmark>]
  //member __.Linq_Min_seq() =
  //  ss.Min()

  //[<Benchmark>]
  //member __.Fsharp_Seq_max_fslist() =
  //  xs |> Seq.max

  //[<Benchmark>]
  //member __.Funtom_Linq_max_fslist() =
  //  xs |> Linq.max

  //[<Benchmark>]
  //member __.Linq_Max_fslist() =
  //  xs.Max()

  //[<Benchmark>]
  //member __.Fsharp_Seq_max_array() =
  //  ys |> Seq.max

  //[<Benchmark>]
  //member __.Funtom_Linq_max_array() =
  //  ys |> Linq.max

  //[<Benchmark>]
  //member __.Linq_Max_array() =
  //  ys.Max()

  //[<Benchmark>]
  //member __.Fsharp_Seq_max_resizearry() =
  //  zs |> Seq.max

  //[<Benchmark>]
  //member __.Funtom_Linq_max_resizearry() =
  //  zs |> Linq.max

  //[<Benchmark>]
  //member __.Linq_Max_resizearry() =
  //  zs.Max()

  //[<Benchmark>]
  //member __.Fsharp_Seq_max_seq() =
  //  ss |> Seq.max

  //[<Benchmark>]
  //member __.Funtom_Linq_max_seq() =
  //  ss |> Linq.max

  //[<Benchmark>]
  //member __.Linq_Max_seq() =
  //  ss.Max()
  
  //[<Benchmark>]
  //member __.Fsharp_Seq_sum_fslist() =
  //  xs |> Seq.sum

  //[<Benchmark>]
  //member __.Funtom_Linq_sum_fslist() =
  //  xs |> Linq.sum

  //[<Benchmark>]
  //member __.Linq_Sum_fslist() =
  //  xs.Sum()

  //[<Benchmark>]
  //member __.Fsharp_Seq_sum_array() =
  //  ys |> Seq.sum

  //[<Benchmark>]
  //member __.Funtom_Linq_sum_array() =
  //  ys |> Linq.sum

  //[<Benchmark>]
  //member __.Linq_Sum_array() =
  //  ys.Sum()

  //[<Benchmark>]
  //member __.Fsharp_Seq_sum_resizearry() =
  //  zs |> Seq.sum

  //[<Benchmark>]
  //member __.Funtom_Linq_sum_resizearry() =
  //  zs |> Linq.sum

  //[<Benchmark>]
  //member __.Linq_Sum_resizearry() =
  //  zs.Sum()
  


  //let xs = [ 0 .. 10000 ]
  //let ys = [| 0 .. 10000 |]
  //let zs = ResizeArray([| 0 .. 10000 |])
  //let ss = seq { 0 .. 10000 }

  //[<Benchmark>]
  //member __.Fsharp_Seq_sum_fslist() =
  //  xs |> Seq.sum

  //[<Benchmark>]
  //member __.Funtom_sum_fslist() =
  //  xs |> Linq.sum

  //[<Benchmark>]
  //member __.Linq_Sum_fslist() =
  //  xs.Sum()

  //[<Benchmark>]
  //member __.Fsharp_Seq_sum_array() =
  //  ys |> Seq.sum

  //[<Benchmark>]
  //member __.Funtom_sum_array() =
  //  ys |> Linq.sum

  //[<Benchmark>]
  //member __.Linq_Sum_array() =
  //  ys.Sum()
  
  //[<Benchmark>]
  //member __.Fsharp_Seq_sum_resizearray() =
  //  zs |> Seq.sum

  //[<Benchmark>]
  //member __.Funtom_sum_resizearray() =
  //  zs |> Linq.sum

  //[<Benchmark>]
  //member __.Linq_Sum_resizearray() =
  //  zs.Sum()
  
  //[<Benchmark>]
  //member __.Fsharp_Seq_sum_seq() =
  //  ss |> Seq.sum

  //[<Benchmark>]
  //member __.Funtom_sum_seq() =
  //  ss |> Linq.sum

  //[<Benchmark>]
  //member __.Linq_Sum_seq() =
  //  ss.Sum()

  //[<Benchmark>]
  //member __.Fsharp_Seq_map() =
  //  ys 
  //  |> Seq.map (fun x -> x % 2 = 0)
  //  |> Linq.toArray

  //[<Benchmark>]
  //member __.Funtom_select1() =
  //  ys 
  //  |> Linq.select1 (fun x -> x % 2 = 0)
  //  |> Linq.toArray

  //[<Benchmark>]
  //member __.Linq_Select() =
  //  ys
  //    .Select(fun x -> x % 2 = 0)
  //    .ToArray()

  //[<Benchmark>]
  //member __.Fsharp_Seq_filter() =
  //  ys 
  //  |> Seq.filter (fun x -> x % 2 = 0)
  //  |> Linq.toArray

  //[<Benchmark>]
  //member __.Funtom_where() =
  //  ys 
  //  |> Linq.where (fun x -> x % 2 = 0)
  //  |> Linq.toArray

  //[<Benchmark>]
  //member __.Linq_Where() =
  //  ys
  //    .Where(fun x -> x % 2 = 0)
  //    .ToArray()


  //[<Benchmark>]
  //member __.Fsharp_Seq_map_fslist() =
  //  xs |> Seq.map ((*) 2) |> Linq.toArray

  //[<Benchmark>]
  //member __.Funtom_select_fslist() =
  //  xs |> Linq.select ((*) 2) |> Linq.toArray

  //[<Benchmark>]
  //member __.Linq_Select_fslist() =
  //  xs.Select((*) 2).ToArray()
  
  //[<Benchmark>]
  //member __.Fsharp_Seq_map_array() =
  //  ys |> Seq.map ((*) 2) |> Linq.toArray

  //[<Benchmark>]
  //member __.Funtom_select_array() =
  //  ys |> Linq.select ((*) 2) |> Linq.toArray

  //[<Benchmark>]
  //member __.Linq_Select_array() =
  //  ys.Select((*) 2).ToArray()
  
  //[<Benchmark>]
  //member __.Fsharp_Seq_map_resizearry() =
  //  zs |> Seq.map ((*) 2) |> Linq.toArray

  //[<Benchmark>]
  //member __.Funtom_select_resizearry() =
  //  zs |> Linq.select ((*) 2) |> Linq.toArray

  //[<Benchmark>]
  //member __.Linq_Select_resizearry() =
  //  zs.Select((*) 2).ToArray()

  //[<Benchmark>]
  //member __.Fsharp_Seq_map_seq() =
  //  ss |> Seq.map ((*) 2) |> Linq.toArray

  //[<Benchmark>]
  //member __.Funtom_select_seq() =
  //  ss |> Linq.select ((*) 2) |> Linq.toArray

  //[<Benchmark>]
  //member __.Linq_Select_seq() =
  //  ss.Select((*) 2).ToArray()

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