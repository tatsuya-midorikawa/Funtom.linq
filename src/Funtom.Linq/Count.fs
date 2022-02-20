namespace Funtom.Linq

open System.Runtime.CompilerServices

module Count =
  open System.Collections
  open System.Collections.Generic
  open Interfaces

  [<Extension>]
  type CountExtension =
    [<Extension>]
    static member inline tryGetNonEnumerateCount<'T>(src: seq<'T>, count: outref<int>) = 
      match src with
      | :? ICollection<'T> as collection -> count <- collection.Count; true
      | :? IListProvider<'T> as prov ->
        let c = prov.GetCount(false)
        count <- c
        0 <= c
      | :? ICollection as collection -> count <- collection.Count; true
      | _ -> count <- 0; false
 