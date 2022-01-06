namespace Funtom.Linq.SpeedOpt

open System.Runtime.CompilerServices

module ArrayOp =
  [<Literal>]
  let DefaultCapacity = 4
  [<Literal>]
  let StartingCapacity = 4
  [<Literal>]
  let ResizeLimit = 8
  [<Literal>]
  let MaxCoreClrArrayLength = 0x7fefffff  // For byte arrays the limit is slightly larger

  /// <summary>
  /// 
  /// </summary>
  /// <see href="https://github.com/JonHanna/corefx/blob/master/src/Common/src/System/Collections/Generic/ArrayBuilder.cs#L13">struct ArrayBuilder&lt;T&gt;</see>
  [<Struct;NoComparison;NoEquality;IsByRefLike>]
  type ArrayBuilder<'T> =
    val mutable capacity': int
    val mutable array': array<'T>
    val mutable count': int
    new (capacity: int) = { capacity' = capacity; array' = Array.empty<'T>; count' = 0; }


  /// <summary>
  /// 
  /// </summary>
  /// <see href="https://github.com/JonHanna/corefx/blob/master/src/Common/src/System/Collections/Generic/LargeArrayBuilder.SpeedOpt.cs#L14">LargeArrayBuilder&lt;T&gt;</see>
  [<Struct;NoComparison;NoEquality;IsByRefLike>]
  type LargeArrayBuilder<'T> =
    val mutable maxCapacity': int
    val mutable first': array<'T>
    new (maxCapacity: int) = { maxCapacity' = maxCapacity; first' = Array.empty<'T>; }

