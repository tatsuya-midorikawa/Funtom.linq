open BenchmarkDotNet.Attributes
open BenchmarkDotNet.Running
open Funtom.linq
open System.Linq
open System
open System.Collections
open Bogus

let fake = Faker()

type Sample = { Num1: int; Num2: int }

[<PlainExporter; MemoryDiagnoser>]
type Benchmark () =
//  let xs = [ 0..10000 ]
//  let ys = [| 0..10000 |]
//  let zs = ResizeArray [| 0..10000 |]
//  let ss = seq { 0..10000 }
  
  let mutable xs = List.empty
  let mutable ys = Array.empty
  let mutable zs = ResizeArray()
  let mutable ss = Seq.empty
  
  let mutable xs' = List.empty
  let mutable ys' = Array.empty
  let mutable zs' = ResizeArray()
  let mutable ss' = Seq.empty
  
  let mutable xs'' : list<Sample> = List.empty
  let mutable ys'' : array<Sample> = Array.empty
  let mutable zs'' : ResizeArray<Sample> = ResizeArray()
  let mutable ss'' : seq<Sample> = Seq.empty
  
  [<GlobalSetup>]
  member this.Setup() =
    xs <- [for _ in 1..10000 do fake.Random.Int()]
    ys <- [|for _ in 1..10000 do fake.Random.Int()|]
    zs <- ResizeArray([|for _ in 1..10000 do fake.Random.Int()|])
    ss <- [|for _ in 1..10000 do fake.Random.Int()|] |> Seq.ofArray

    xs' <- [1..10000]
    ys' <- [|1..10000|]
    zs' <- ResizeArray([|1..10000|])
    ss' <- [|1..10000|] |> Seq.ofArray
    
    xs'' <- [for _ in 1..10000 do { Num1 = fake.Random.Int(); Num2 = fake.Random.Int() } ]
    ys'' <- [|for _ in 1..10000 do { Num1 = fake.Random.Int(); Num2 = fake.Random.Int() } |]
    zs'' <- ResizeArray([|for _ in 1..10000 do { Num1 = fake.Random.Int(); Num2 = fake.Random.Int() } |])
    ss'' <- [|for _ in 1..10000 do { Num1 = fake.Random.Int(); Num2 = fake.Random.Int() } |] |> Seq.ofArray

  // [<Benchmark>]
  // member __.Linq_Reverse_list() = 
  //   let mutable acc = 0
  //   for v in xs.Reverse() do acc <- acc + v
  //   acc

  // [<Benchmark>]
  // member __.Funtom_reverse_list() =
  //   let mutable acc = 0
  //   for v in xs |> Linq.reverse do acc <- acc + v
  //   acc
  
  // [<Benchmark>]
  // member __.Linq_Reverse_array() =
  //   let mutable acc = 0
  //   for v in ys.Reverse() do acc <- acc + v
  //   acc

  // [<Benchmark>]
  // member __.Funtom_reverse_array() =
  //   let mutable acc = 0
  //   for v in ys |> Linq.reverse do acc <- acc + v
  //   acc
  
  // [<Benchmark>]
  // member __.Linq_Reverse_resizearray() =
  //   let mutable acc = 0
  //   for v in (zs :> seq<int>).Reverse() do 
  //     acc <- acc + v
  //   acc

  // [<Benchmark>]
  // member __.Funtom_reverse_resizearray() =
  //   let mutable acc = 0
  //   for v in zs |> Linq.reverse do acc <- acc + v
  //   acc
  
  // [<Benchmark>]
  // member __.Linq_Reverse_seq() =
  //   let mutable acc = 0
  //   for v in ss.Reverse() do acc <- acc + v
  //   acc

  // [<Benchmark>]
  // member __.Funtom_reverse_seq() =
  //   let mutable acc = 0
  //   for v in ss |> Linq.reverse do acc <- acc + v
  //   acc



  //[<Benchmark>]
  //member __.Linq_Except_list() = 
  //  let mutable acc = 0
  //  for v in xs.Except(ys) do acc <- acc + v
  //  acc

  //[<Benchmark>]
  //member __.Funtom_except_list() =
  //  let mutable acc = 0
  //  for v in xs |> Linq.except(ys) do acc <- acc + v
  //  acc
  
  //[<Benchmark>]
  //member __.Linq_Except_array() =
  //  let mutable acc = 0
  //  for v in ys.Except(zs) do acc <- acc + v
  //  acc

  //[<Benchmark>]
  //member __.Funtom_except_array() =
  //  let mutable acc = 0
  //  for v in ys |> Linq.except(zs) do acc <- acc + v
  //  acc
  
  //[<Benchmark>]
  //member __.Linq_Except_resizearray() =
  //  let mutable acc = 0
  //  for v in zs.Except(ss) do acc <- acc + v
  //  acc

  //[<Benchmark>]
  //member __.Funtom_except_resizearray() =
  //  let mutable acc = 0
  //  for v in zs |> Linq.except(ss) do acc <- acc + v
  //  acc
  
  //[<Benchmark>]
  //member __.Linq_Except_seq() =
  //  let mutable acc = 0
  //  for v in ss.Except(xs) do acc <- acc + v
  //  acc

  //[<Benchmark>]
  //member __.Funtom_except_seq() =
  //  let mutable acc = 0
  //  for v in ss |> Linq.except(xs) do acc <- acc + v
  //  acc



  //[<Benchmark>]
  //member __.Linq_ElementAt_list() = xs.ElementAt(2000)

  //[<Benchmark>]
  //member __.Funtom_elementAt_list() = xs |> Linq.elementAt 2000
  
  //[<Benchmark>]
  //member __.Linq_ElementAt_array() = ys.ElementAt(2000)

  //[<Benchmark>]
  //member __.Funtom_elementAt_array() = ys |> Linq.elementAt 2000
  
  //[<Benchmark>]
  //member __.Linq_ElementAt_resizearray() = zs.ElementAt(2000)

  //[<Benchmark>]
  //member __.Funtom_elementAt_resizearray() = zs |> Linq.elementAt 2000
  
  //[<Benchmark>]
  //member __.Linq_ElementAt_seq() = ss.ElementAt(2000)

  //[<Benchmark>]
  //member __.Funtom_elementAt_seq() = ss |> Linq.elementAt 2000


    
  //[<Benchmark>]
  //member __.Linq_ElementAtOrDefault_list() = xs.ElementAtOrDefault(2000)

  //[<Benchmark>]
  //member __.Funtom_elementAtOrDefault_list() = xs |> Linq.elementAtOrDefault 2000
  
  //[<Benchmark>]
  //member __.Linq_ElementAtOrDefault_array() = ys.ElementAtOrDefault(2000)

  //[<Benchmark>]
  //member __.Funtom_elementAtOrDefault_array() = ys |> Linq.elementAtOrDefault 2000
  
  //[<Benchmark>]
  //member __.Linq_ElementAtOrDefault_resizearray() = zs.ElementAtOrDefault(2000)

  //[<Benchmark>]
  //member __.Funtom_elementAtOrDefault_resizearray() = zs |> Linq.elementAtOrDefault 2000

  // [<Benchmark>]
  // member __.Linq_ElementAtOrDefault_seq() = ss.ElementAtOrDefault(2000)

  // [<Benchmark>]
  // member __.Funtom_elementAtOrDefault_seq() = ss |> Linq.elementAtOrDefault 2000



  //[<Benchmark>]
  //member __.System_Linq_distinctBy_list() = 
  //  let mutable acc = 0
  //  for v in xs''.DistinctBy(fun v -> v.Num1) do acc <- acc + v.Num2
  //  acc

  //[<Benchmark>]
  //member __.Funtom_Linq_distinctBy_list() =
  //  let mutable acc = 0
  //  for v in xs'' |> Linq.distinctBy (fun v -> v.Num1) do acc <- acc + v.Num2
  //  acc

  //[<Benchmark>]
  //member __.System_Linq_distinctBy_array() =
  //  let mutable acc = 0
  //  for v in ys''.DistinctBy(fun v -> v.Num1) do acc <- acc + v.Num2
  //  acc

  //[<Benchmark>]
  //member __.Funtom_Linq_distinctBy_array() =
  //  let mutable acc = 0
  //  for v in ys'' |> Linq.distinctBy (fun v -> v.Num1) do acc <- acc + v.Num2
  //  acc

  //[<Benchmark>]
  //member __.System_Linq_distinctBy_resizablearray() =
  //  let mutable acc = 0
  //  for v in zs''.DistinctBy(fun v -> v.Num1) do acc <- acc + v.Num2
  //  acc

  //[<Benchmark>]
  //member __.Funtom_Linq_distinctBy_resizablearray() =
  //  let mutable acc = 0
  //  for v in zs'' |> Linq.distinctBy (fun v -> v.Num1) do acc <- acc + v.Num2
  //  acc

  //[<Benchmark>]
  //member __.System_Linq_distinctBy_seq() =
  //  let mutable acc = 0
  //  for v in ss''.DistinctBy(fun v -> v.Num1) do acc <- acc + v.Num2
  //  acc

  //[<Benchmark>]
  //member __.Funtom_Linq_distinctBy_seq() =
  //  let mutable acc = 0
  //  for v in ss'' |> Linq.distinctBy (fun v -> v.Num1) do acc <- acc + v.Num2
  //  acc




  //[<Benchmark>]
  //member __.System_Linq_distinct_list() = 
  //  let mutable acc = 0
  //  for v in xs.Distinct() do acc <- acc + v
  //  acc

  //[<Benchmark>]
  //member __.Funtom_Linq_distinct_list() =
  //  let mutable acc = 0
  //  for v in xs |> Linq.distinct do acc <- acc + v
  //  acc

  //[<Benchmark>]
  //member __.System_Linq_distinct_array() =
  //  let mutable acc = 0
  //  for v in ys.Distinct() do acc <- acc + v
  //  acc

  //[<Benchmark>]
  //member __.Funtom_Linq_distinct_array() =
  //  let mutable acc = 0
  //  for v in ys |> Linq.distinct do acc <- acc + v
  //  acc

  //[<Benchmark>]
  //member __.System_Linq_distinct_resizablearray() =
  //  let mutable acc = 0
  //  for v in zs.Distinct() do acc <- acc + v
  //  acc

  //[<Benchmark>]
  //member __.Funtom_Linq_distinct_resizablearray() =
  //  let mutable acc = 0
  //  for v in zs |> Linq.distinct do acc <- acc + v
  //  acc

  //[<Benchmark>]
  //member __.System_Linq_distinct_seq() =
  //  let mutable acc = 0
  //  for v in ss.Distinct() do acc <- acc + v
  //  acc

  //[<Benchmark>]
  //member __.Funtom_Linq_distinct_seq() =
  //  let mutable acc = 0
  //  for v in ss |> Linq.distinct do acc <- acc + v
  //  acc

    


  //[<Benchmark>]
  //member __.System_Linq_contains_list() = xs'.Contains(10000)

  //[<Benchmark>]
  //member __.Funtom_Linq_contains_list() = xs' |> Linq.contains (10000)

  //[<Benchmark>]
  //member __.System_Linq_contains_array() = ys'.Contains(10000)

  //[<Benchmark>]
  //member __.Funtom_Linq_contains_array() = ys' |> Linq.contains (10000)

  //[<Benchmark>]
  //member __.System_Linq_contains_resizablearray() = zs'.Contains(10000)

  //[<Benchmark>]
  //member __.Funtom_Linq_contains_resizablearray() = zs' |> Linq.contains 10000

  //[<Benchmark>]
  //member __.System_Linq_contains_seq() = ss'.Contains(10000)

  //[<Benchmark>]
  //member __.Funtom_Linq_contains_seq() = ss' |> Linq.contains 10000





  //[<Benchmark>]
  //member __.System_Linq_concat_list() =
  //  let ls = xs.Concat(xs)
  //  let mutable acc = 0
  //  for l in ls do
  //    acc <- acc + l
  //  acc

  //[<Benchmark>]
  //member __.Funtom_Linq_concat_list() =
  //  let ls = xs |> Linq.concat xs
  //  let mutable acc = 0
  //  for l in ls do
  //    acc <- acc + l
  //  acc

  //[<Benchmark>]
  //member __.System_Linq_concat_array() =
  //  let ls = ys.Concat(ys)
  //  let mutable acc = 0
  //  for l in ls do
  //    acc <- acc + l
  //  acc

  //[<Benchmark>]
  //member __.Funtom_Linq_concat_array() =
  //  let ls = ys |> Linq.concat ys
  //  let mutable acc = 0
  //  for l in ls do
  //    acc <- acc + l
  //  acc

  //[<Benchmark>]
  //member __.System_Linq_concat_resizablearray() =
  //  let ls = zs.Concat(zs)
  //  let mutable acc = 0
  //  for l in ls do
  //    acc <- acc + l
  //  acc

  //[<Benchmark>]
  //member __.Funtom_Linq_concat_resizablearray() =
  //  let ls = zs |> Linq.concat zs
  //  let mutable acc = 0
  //  for l in ls do
  //    acc <- acc + l
  //  acc

  //[<Benchmark>]
  //member __.System_Linq_concat_seq() =
  //  let ls = ss.Concat(ss)
  //  let mutable acc = 0
  //  for l in ls do
  //    acc <- acc + l
  //  acc

  //[<Benchmark>]
  //member __.Funtom_Linq_concat_seq() =
  //  let ls = ss |> Linq.concat ss
  //  let mutable acc = 0
  //  for l in ls do
  //    acc <- acc + l
  //  acc

  //[<Benchmark>]
  //member __.System_Linq_count_seq() =
  //  ss.Count()

  //[<Benchmark>]
  //member __.Funtom_Linq_count_seq() =
  //  ss |> Linq.count
    

  //[<Benchmark>]
  //member __.System_Linq_count_predicate_seq() =
  //  ss.Count(fun x -> x % 2 = 0)

  //[<Benchmark>]
  //member __.Funtom_Linq_count_predicate_seq() =
  //  ss |> Linq.count' (fun x -> x % 2 = 0)


  //[<Benchmark>]
  //member __.System_Linq_chunk_seq() =
  //  let ps = ys.Chunk(10)
  //  let mutable acc = 0
  //  for rs in ps do
  //    for r in rs do
  //      acc <- acc + r
  //  acc

  //[<Benchmark>]
  //member __.Funtom_Linq_chunk_seq() =
  //  let ps = ys |> Linq.chunk 10
  //  let mutable acc = 0
  //  for rs in ps do
  //    for r in rs do
  //      acc <- acc + r
  //  acc


  //[<Benchmark>]
  //member __.Funtom_Linq_append() =
  //  let mutable ls : seq<int> = [||]
  //  for i = 0 to 1000 do
  //    ls <- ls |> Linq.append i
  //  ls |> Linq.toArray

  //[<Benchmark>]
  //member __.System_Linq_append() =
  //  let mutable ls : seq<int> = [||]
  //  for i = 0 to 1000 do
  //    ls <- ls.Append i
  //  ls |> Linq.toArray

  //[<Benchmark>]
  //member __.Funtom_Linq_append2() =
  //  let mutable ls : seq<int> = [||]
  //  for i = 0 to 1000 do
  //    ls <- ls |> Linq.append i

  //  let mutable acc = 0
  //  for x in ls do
  //    acc <- acc + x
  //  acc

  //[<Benchmark>]
  //member __.System_Linq_append2() =
  //  let mutable ls : seq<int> = [||]
  //  for i = 0 to 1000 do
  //    ls <- ls.Append i

  //  let mutable acc = 0
  //  for x in ls do
  //    acc <- acc + x
  //  acc

  // // ====================
  // // map

  // [<Benchmark>]
  // member __.System_Linq_select_fslist() =
  //  let mutable acc = 0
  //  for x in xs.Select(fun v -> v / 3) do
  //    acc <- acc + x
  //  acc

  // [<Benchmark>]
  // member __.Funtom_Linq_select_fslist() =
  //  let mutable acc = 0
  //  for x in xs |> Linq.select (fun v -> v / 3) do
  //    acc <- acc + x
  //  acc

  // [<Benchmark>]
  // member __.Funtom_Linq2_select_fslist() =
  //  let mutable acc = 0
  //  for x in xs |> Linq2.select (fun v -> v / 3) do
  //    acc <- acc + x
  //  acc
    
  // [<Benchmark>]
  // member __.Fsharp_map_fslist() =
  //  let mutable acc = 0
  //  for x in xs |> List.map (fun v -> v / 3) do
  //    acc <- acc + x
  //  acc
  
  // //---

  // [<Benchmark>]
  // member __.System_Linq_select_array() =
  //   let mutable acc = 0
  //   for y in ys.Select(fun v -> v / 3) do
  //     acc <- acc + y
  //   acc

  // [<Benchmark>]
  // member __.Funtom_Linq_select_array() =
  //   let mutable acc = 0
  //   for y in ys |> Linq.select (fun v -> v / 3) do
  //     acc <- acc + y
  //   acc

  // [<Benchmark>]
  // member __.Funtom_Linq2_select_array() =
  //   let mutable acc = 0
  //   for y in ys |> Linq2.select (fun v -> v / 3) do
  //     acc <- acc + y
  //   acc
    
  // [<Benchmark>]
  // member __.Fsharp_map_array() =
  //  let mutable acc = 0
  //  for y in ys |> Array.map (fun v -> v / 3) do
  //    acc <- acc + y
  //  acc
    
  // //---

  // [<Benchmark>]
  // member __.System_Linq_select_resizearray() =
  //  let mutable acc = 0
  //  for z in zs.Select(fun v -> v / 3) do
  //    acc <- acc + z
  //  acc

  // [<Benchmark>]
  // member __.Funtom_Linq_select_resizearray() =
  //  let mutable acc = 0
  //  for z in zs |> Linq.select (fun v -> v / 3) do
  //    acc <- acc + z
  //  acc

  // [<Benchmark>]
  // member __.Funtom_Linq2_select_resizearray() =
  //  let mutable acc = 0
  //  for z in zs |> Linq2.select (fun v -> v / 3) do
  //    acc <- acc + z
  //  acc
    
  // [<Benchmark>]
  // member __.Fsharp_map_resizearray() =
  //  let mutable acc = 0
  //  for z in zs |> FSharpx.Collections.ResizeArray.map (fun v -> v / 3) do
  //    acc <- acc + z
  //  acc
   
  //---

  [<Benchmark>]
  member __.System_Linq_select_seq() =
   let mutable acc = 0
   for s in ss.Select(fun v -> v / 3) do
     acc <- acc + s
   acc

  [<Benchmark>]
  member __.Funtom_Linq_select_seq() =
   let mutable acc = 0
   for s in ss |> Linq.select (fun v -> v / 3) do
     acc <- acc + s
   acc

  [<Benchmark>]
  member __.Funtom_Linq2_select_seq() =
   let mutable acc = 0
   for s in ss |> Linq2.select (fun v -> v / 3) do
     acc <- acc + s
   acc
  
  [<Benchmark>]
  member __.Fsharp_map_seq() =
   let mutable acc = 0
   for s in ss |> Seq.map (fun v -> v / 3) do
     acc <- acc + s
   acc
  
  // //---

  // [<Benchmark>]
  // member __.System_Linq_select_seq() =
  //  let mutable acc = 0
  //  for s in ss.Where(fun v -> v % 2 = 0).Select(fun v -> v * 3) do
  //    acc <- acc + s
  //  acc

  // [<Benchmark>]
  // member __.Funtom_Linq_select_seq() =
  //  let mutable acc = 0
  //  for s in ss |> Linq.where (fun v -> v % 2 = 0) |> Linq.select (fun v -> v * 3) do
  //    acc <- acc + s
  //  acc


  // // ====================
  // // map to array

  // [<Benchmark>]
  // member __.System_Linq_select_fslist_toarray() =
  //  xs.Select(fun v -> v / 2).ToArray()

  // [<Benchmark>]
  // member __.Funtom_Linq_select_fslist_toarray() =
  //  xs |> Linq.select (fun v -> v / 2) |> Linq.toArray
    
  // [<Benchmark>]
  // member __.System_Linq_select_array_toarray() =
  //  ys.Select(fun v -> v / 2).ToArray()

  // [<Benchmark>]
  // member __.Funtom_Linq_select_array_toarray() =
  //  ys |> Linq.select (fun v -> v / 2) |> Linq.toArray
    
  // [<Benchmark>]
  // member __.System_Linq_select_resizearray_toarray() =
  //  zs.Select(fun v -> v / 2).ToArray()

  // [<Benchmark>]
  // member __.Funtom_Linq_select_resizearray_toarray() =
  //  zs |> Linq.select (fun v -> v / 2) |> Linq.toArray

  // [<Benchmark>]
  // member __.System_Linq_select_seq_toarray() =
  //  ss.Select(fun v -> v / 2).ToArray()

  // [<Benchmark>]
  // member __.Funtom_Linq_select_seq_toarray() =
  //  ss |> Linq.select (fun v -> v / 2) |> Linq.toArray

    
  //[<Benchmark>]
  //member __.System_Linq_toarray_seq() =
  //  ss.ToArray()

  //[<Benchmark>]
  //member __.Funtom_Linq_toarray_seq() =
  //  ss |> Linq.toArray


  //[<Benchmark>]
  //member __.System_Linq_aggregate_fslist() =
  //  xs.Aggregate(0, fun v next -> v + next)

  //[<Benchmark>]
  //member __.Funtom_Linq_aggregate_fslist() =
  //  xs |> Linq.aggregate 0 (fun v next -> v + next)
    
  //[<Benchmark>]
  //member __.System_Linq_aggregate_array() =
  //  ys.Aggregate(0, fun v next -> v + next)

  //[<Benchmark>]
  //member __.Funtom_Linq_aggregate_array() =
  //  ys |> Linq.aggregate 0 (fun v next -> v + next)
    
  //[<Benchmark>]
  //member __.System_Linq_aggregate_resizearray() =
  //  zs.Aggregate(0, fun v next -> v + next)

  //[<Benchmark>]
  //member __.Funtom_Linq_aggregate_resizearray() =
  //  zs |> Linq.aggregate 0 (fun v next -> v + next)
    
  //[<Benchmark>]
  //member __.System_Linq_aggregate_seq() =
  //  ss.Aggregate(0, fun v next -> v + next)

  //[<Benchmark>]
  //member __.Funtom_Linq_aggregate_seq() =
  //  ss |> Linq.aggregate 0 (fun v next -> v + next)


  //[<Benchmark>]
  //member __.System_Linq_all_fslist() =
  //  xs.All(fun x -> x < 10000)

  //[<Benchmark>]
  //member __.Funtom_Linq_all_fslist() =
  //  xs |> Linq.all (fun x -> x < 10000)
    
  //[<Benchmark>]
  //member __.System_Linq_all_array() =
  //  ys.All(fun x -> x < 10000)

  //[<Benchmark>]
  //member __.Funtom_Linq_all_array() =
  //  ys |> Linq.all (fun x -> x < 10000)
    
  //[<Benchmark>]
  //member __.System_Linq_all_resizearray() =
  //  zs.All(fun x -> x < 10000)

  //[<Benchmark>]
  //member __.Funtom_Linq_all_resizearray() =
  //  zs |> Linq.all (fun x -> x < 10000)
    
  //[<Benchmark>]
  //member __.System_Linq_all_seq() =
  //  ss.All(fun x -> x < 10000)

  //[<Benchmark>]
  //member __.Funtom_Linq_all_seq() =
  //  ss |> Linq.all (fun x -> x < 10000)


  //[<Benchmark>]
  //member __.System_Linq_any_fslist() =
  //  xs.Any(fun x -> 10000 < x)

  //[<Benchmark>]
  //member __.Funtom_Linq_any_fslist() =
  //  xs |> Linq.any' (fun x -> 10000 < x)
    
  //[<Benchmark>]
  //member __.System_Linq_any_array() =
  //  ys.Any(fun x -> 10000 < x)

  //[<Benchmark>]
  //member __.Funtom_Linq_any_array() =
  //  ys |> Linq.any' (fun x -> 10000 < x)
    
  //[<Benchmark>]
  //member __.System_Linq_any_resizearray() =
  //  zs.Any(fun x -> 10000 < x)

  //[<Benchmark>]
  //member __.Funtom_Linq_any_resizearray() =
  //  zs |> Linq.any' (fun x -> 10000 < x)
    
  //[<Benchmark>]
  //member __.System_Linq_any_seq() =
  //  ss.Any(fun x -> 10000 < x)

  //[<Benchmark>]
  //member __.Funtom_Linq_any_seq() =
  //  ss |> Linq.any' (fun x -> 10000 < x)



  //[<Benchmark>]
  //member __.System_Linq_any_fslist() =
  //  xs.Any()

  //[<Benchmark>]
  //member __.Funtom_Linq_any_fslist() =
  //  xs |> Linq.any
    
  //[<Benchmark>]
  //member __.System_Linq_any_array() =
  //  ys.Any()

  //[<Benchmark>]
  //member __.Funtom_Linq_any_array() =
  //  ys |> Linq.any
    
  //[<Benchmark>]
  //member __.System_Linq_any_resizearray() =
  //  zs.Any()

  //[<Benchmark>]
  //member __.Funtom_Linq_any_resizearray() =
  //  zs |> Linq.any
    
  //[<Benchmark>]
  //member __.System_Linq_any_seq() =
  //  ss.Any()

  //[<Benchmark>]
  //member __.Funtom_Linq_any_seq() =
  //  ss |> Linq.any



  //[<Benchmark>]
  //member __.System_Linq_any() =
  //  xs.All(fun x -> x < 10000)

  //[<Benchmark>]
  //member __.Funtom_Linq_any() =
  //  xs |> Linq.all (fun x -> x < 10000)

  //[<Benchmark>]
  //member __.System_Linq_all() =
  //  xs.All(fun x -> x < 10000)

  //[<Benchmark>]
  //member __.Funtom_Linq_all() =
  //  xs |> Linq.all (fun x -> x < 10000)



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