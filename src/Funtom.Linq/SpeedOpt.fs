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
      assert (0 <= index && index < __.count')
      __.array'[index]

    member __.Add (item: 'T) =
      if __.count' = __.Capacity then
        __.EnsureCapacity(__.count' + 1)
      __.UncheckedAdd item

    member __.First () = __.array'[0]

    member __.Last () = __.array'[__.count' - 1]

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
    val mutable buffers': ArrayBuilder<array<'T>>
    val mutable current': array<'T>
    val mutable index': int
    val mutable count': int
    new (maxCapacity: int) = { 
      maxCapacity' = maxCapacity
      first' = Array.empty<'T>
      buffers' = ArrayBuilder(StartingCapacity)
      current' = Array.empty<'T>
      index' = 0
      count' = 0 }

    member __.Count = __.count'

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member __.Add (item: 'T) =
      assert (__.count' < __.maxCapacity')
      let mutable index = __.index'
      let current = __.current'
      if uint current.Length <= uint index then
        __.AddWithBufferAllocation item
      else
       current[index] <- item
       __.index' <- index + 1
      __.count' <- __.count' + 1

    // src: https://github.com/JonHanna/corefx/blob/master/src/Common/src/System/Collections/Generic/LargeArrayBuilder.SpeedOpt.cs#L105
    member __.AddRange (items: seq<'T>) =
      use enumerator = items.GetEnumerator()
      let mutable destination = __.current'
      let mutable index = __.index'

      while enumerator.MoveNext() do
        let item = enumerator.Current
        if uint destination.Length <= uint index then
          __.AddWithBufferAllocation(item, &destination, &index)
        else
          destination[index] <- item
        index <- index + 1 

      __.count' <- __.count' + (index - __.index')
      __.index' <- index

    // TODO
    // src: https://github.com/JonHanna/corefx/blob/master/src/Common/src/System/Collections/Generic/LargeArrayBuilder.SpeedOpt.cs#L157
    member __.CopyTo (array: array<'T>, arrayIndex: int, count: int) =
      ()
        
    [<MethodImpl(MethodImplOptions.NoInlining)>]
    member private __.AddWithBufferAllocation (item: 'T) =
      __.AllocateBuffer ()
      __.current'[__.index'] <- item
      __.index' <- __.index' + 1
      
    // src: https://github.com/JonHanna/corefx/blob/master/src/Common/src/System/Collections/Generic/LargeArrayBuilder.SpeedOpt.cs#L141
    [<MethodImpl(MethodImplOptions.NoInlining)>]
    member private __.AddWithBufferAllocation (item: 'T, destination: byref<array<'T>>, index: byref<int>) =
      __.count' <- __.count' + (index - __.index')
      __.index' <- index
      __.AllocateBuffer()
      destination <- __.current'
      index <- __.index'
      __.current'[index] <- item

    member private __.AllocateBuffer () =      
      if uint __.count' < uint ResizeLimit then
        let nextCapacity = min (if __.count' = 0 then StartingCapacity else 2 * __.count') __.maxCapacity'
        __.current' <- create<'T> nextCapacity
        System.Array.Copy(__.first', 0, __.current', 0, __.count')
        __.first' <- __.current'
      else
        let nextCapacity =
          if __.count' = ResizeLimit then ResizeLimit
          else __.buffers'.Add __.current'; min __.count' (__.maxCapacity' - __.count')
        __.current' <- create<'T> nextCapacity
        __.index' <- 0