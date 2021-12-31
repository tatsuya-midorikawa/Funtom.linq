![img](https://raw.githubusercontent.com/tatsuya-midorikawa/Funtom.Linq/main/assets/phantom.png)  

# Funtom.Linq  

Funtom.Linq is a library for F# that is compatible with System.Linq.  
This library makes it easier to use pipeline operators and optimizes for FSharp.Core.List<'T> and more.  

## Naming rules  

The method names defined in System.Linq are redefined in camelCase.  
For example, 'Select' becomes 'select', and 'ToList' becomes 'toList'.  

## Usage    

In this section, I will show you how to use the System.Linq 'Where' and 'Select' method as an example.  

1. When using System.Linq
    ```fsharp
    open System.Linq

    let xs =
      [ 0..10 ]
        .Where ((>) 5)
        .Select ((*) 2)
    ```  

2. When using Funtom.Linq
    ```fsharp
    #r "nuget: Funtom.Linq"
    open Funtom.Linq

    let xs =
      [ 0..10 ]
      |> Linq.where ((>) 5)
      |> Linq.select ((*) 2)
    ```  

## Performance  

Funtom.Linq is intended to be implemented in such a way that its performance is almost equal to or better than that of System.Linq.  
Let's compare the performance of 'Select'.  

```fsharp
  let xs = [ 0 .. 10000 ]
  let ys = [| 0 .. 10000 |]
  let zs = ResizeArray([| 0 .. 10000 |])
  let ss = seq { 0 .. 10000 }

  [<Benchmark>]
  member __.Fsharp_Seq_map_fslist() = xs |> Seq.map ((*) 2) |> Linq.toArray

  [<Benchmark>]
  member __.Funtom_select_fslist() = xs |> Linq.select ((*) 2) |> Linq.toArray

  [<Benchmark>]
  member __.Linq_Select_fslist() = xs.Select((*) 2).ToArray()
  
  [<Benchmark>]
  member __.Fsharp_Seq_map_array() = ys |> Seq.map ((*) 2) |> Linq.toArray

  [<Benchmark>]
  member __.Funtom_select_array() = ys |> Linq.select ((*) 2) |> Linq.toArray

  [<Benchmark>]
  member __.Linq_Select_array() = ys.Select((*) 2).ToArray()
  
  [<Benchmark>]
  member __.Fsharp_Seq_map_resizearry() = zs |> Seq.map ((*) 2) |> Linq.toArray

  [<Benchmark>]
  member __.Funtom_select_resizearry() = zs |> Linq.select ((*) 2) |> Linq.toArray

  [<Benchmark>]
  member __.Linq_Select_resizearry() = zs.Select((*) 2).ToArray()

  [<Benchmark>]
  member __.Fsharp_Seq_map_seq() = ss |> Seq.map ((*) 2) |> Linq.toArray

  [<Benchmark>]
  member __.Funtom_select_seq() = ss |> Linq.select ((*) 2) |> Linq.toArray

  [<Benchmark>]
  member __.Linq_Select_seq() = ss.Select((*) 2).ToArray()
```  

Result:  
![image](https://user-images.githubusercontent.com/78302178/147803803-ac3e241b-c186-46e4-bedd-a1e9c0407fd0.png)
