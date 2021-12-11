open Funtom.Linq

seq { for i = 0 to 10 do yield i }
|> Linq.selecti (fun x i -> x * i)
|> Seq.toArray
|> printfn "%A"