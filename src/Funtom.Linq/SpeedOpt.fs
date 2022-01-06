namespace Funtom.Linq

open System.Runtime.CompilerServices

module SpeedOpt =
  [<Literal>]
  let StartingCapacity = 4
  [<Literal>]
  let ResizeLimit = 8

  /// <summary>
  /// 
  /// </summary>
  /// <see href="https://github.com/JonHanna/corefx/blob/master/src/Common/src/System/Collections/Generic/LargeArrayBuilder.SpeedOpt.cs#L14">LargeArrayBuilder<T></see>
  [<Struct;NoComparison;NoEquality;IsByRefLike>]
  type LargeArrayBuilder<'T> =
    member __.foo() = ()

