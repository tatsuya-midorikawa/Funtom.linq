namespace Funtom.Linq.Common

open System.Collections.Generic

module Interfaces =
  /// <summary>
  /// 
  /// </summary>
  // src: abstract member ToArray : unit -> array<'T>
  type IListProvider<'T> =
    inherit IEnumerable<'T>
    abstract member ToArray : unit -> array<'T>
    abstract member ToList : unit -> ResizeArray<'T>
    abstract member GetCount : bool -> int

