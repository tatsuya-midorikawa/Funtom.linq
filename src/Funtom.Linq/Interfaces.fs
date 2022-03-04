namespace Funtom.Linq

open System.Collections.Generic

module Interfaces =
  /// <summary>
  /// 
  /// </summary>
  // src: https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/IIListProvider.cs
  type IListProvider<'T> =
    inherit IEnumerable<'T>
    abstract member ToArray : unit -> array<'T>
    abstract member ToList : unit -> ResizeArray<'T>
    abstract member GetCount : bool -> int

  /// <summary>
  /// 
  /// </summary>
  // src: https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/IPartition.cs#L11
  type IPartition<'T> =
    inherit IListProvider<'T>
    abstract member Skip : int -> IPartition<'T>
    abstract member Take : int -> IPartition<'T>
    abstract member TryGetElementAt : int * outref<bool> -> 'T
    abstract member TryGetFirst : outref<bool> -> 'T
    abstract member TryGetLast : outref<bool> -> 'T