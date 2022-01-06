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
  let inline create<'T> length = 
    if 0 < length then Array.create length Unchecked.defaultof<'T> 
    else Array.empty<'T>

  /// <summary>
  /// 
  /// </summary>
  /// <see href="https://github.com/JonHanna/corefx/blob/master/src/Common/src/System/Collections/Generic/ArrayBuilder.cs#L13">struct ArrayBuilder&lt;T&gt;</see>
  [<Struct;NoComparison;NoEquality;IsByRefLike>]
  type ArrayBuilder<'T> =
    val mutable array': array<'T>
    val mutable count': int
    new (capacity: int) = { 
      array' = if 0 < capacity then create<'T> capacity else Array.empty<'T>
      count' = 0; }

    member __.Capacity with get() = __.array'.Length
    member __.Buffer with get() = __.array'
    member __.Count with get() = __.count'
    member __.Item with get(index: int) =
      assert(0 <= index && index < __.count')
      __.array'[index]

    member __.Add (item: 'T) =
      if __.count' = __.Capacity then
        __.EnsureCapacity(__.count' + 1)
      __.UncheckedAdd item

    member __.First () =
      assert(0 < __.count')
      __.array'[0]

    member __.Last () =
      assert(0 < __.count')
      __.array'[__.count' - 1]

    member __.ToArray () =
      let mutable result = __.array'
      if __.count' < result.Length then
        result <- create<'T> __.count'
        System.Array.Copy(__.array', 0, result, 0, __.count')
      result

    member __.UncheckedAdd (item: 'T) =
      __.array'[__.count'] <- item
      __.count' <- __.count' + 1

    member private __.EnsureCapacity (minimum: int) =
      assert(__.Capacity < minimum)
      let capacity = __.Capacity
      let mutable nextCapacity = if capacity = 0 then DefaultCapacity else 2 * capacity
      if uint MaxCoreClrArrayLength < uint nextCapacity then
        nextCapacity <- max (capacity + 1) MaxCoreClrArrayLength
      nextCapacity <- max nextCapacity minimum

      let next = create<'T> nextCapacity
      if 0 < __.count' then
        System.Array.Copy(__.array', 0, next, 0, __.count')
      __.array' <- next

  /// <summary>
  /// 
  /// </summary>
  /// <see href="https://github.com/JonHanna/corefx/blob/master/src/Common/src/System/Collections/Generic/LargeArrayBuilder.SpeedOpt.cs#L14">LargeArrayBuilder&lt;T&gt;</see>
  [<Struct;NoComparison;NoEquality;IsByRefLike>]
  type LargeArrayBuilder<'T> =
    val mutable maxCapacity': int
    val mutable first': array<'T>
    new (maxCapacity: int) = { maxCapacity' = maxCapacity; first' = Array.empty<'T>; }

