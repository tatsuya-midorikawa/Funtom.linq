﻿namespace rec Funtom.linq.iterator

open System
open System.Collections
open System.Collections.Generic
open System.Runtime.CompilerServices
open System.Diagnostics
open Funtom.linq
open Empty

module Basis =
  [<AbstractClass>]
  type Iterator<'T> () =
    let threadId = Environment.CurrentManagedThreadId
    member val internal state = 0 with get, set
    member val internal current = Unchecked.defaultof<'T> with get, set

    abstract member Dispose : unit -> unit
    default __.Dispose () = 
      __.current <- Unchecked.defaultof<'T>
      __.state <- 0
    
    abstract member MoveNext : unit -> bool
    abstract member Current : 'T with get
    default __.Current with get() = __.current
    abstract member Reset : unit -> unit
    default __.Reset () = NotSupportedException() |> raise

    abstract member GetEnumerator : unit -> IEnumerator<'T>
    default __.GetEnumerator () =
      let mutable enumerator =
        if __.state = 0 && threadId = Environment.CurrentManagedThreadId then __
        else __.Clone()
      enumerator.state <- 1
      enumerator

    abstract member Select<'U> : ('T -> 'U) -> seq<'U>
    default __.Select<'U> (selector: 'T -> 'U) = new Select.SelectEnumerableIterator<'T, 'U>(__, selector)

    //// TODO: implement default
    //abstract member Where: ('T -> bool) -> seq<'T>

    abstract member Clone : unit -> Iterator<'T>

    interface IDisposable with member __.Dispose () = __.Dispose()
    interface IEnumerator with
      member __.MoveNext() = __.MoveNext()
      member __.Current with get() = __.Current
      member __.Reset () = __.Reset ()
    interface IEnumerator<'T> with member __.Current with get() = __.Current
    interface IEnumerable with member __.GetEnumerator () = __.GetEnumerator ()
    interface IEnumerable<'T> with member __.GetEnumerator () = __.GetEnumerator ()

  // src: https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/SingleLinkedNode.cs#L12
  [<Sealed>]
  type SingleLinkedNode<'T> private (linked: SingleLinkedNode<'T>, item: 'T) =
    new (item': 'T) = SingleLinkedNode(Unchecked.defaultof<SingleLinkedNode<'T>>, item')

    member __.Item with get() = item
    member __.Linked with get() = linked
    member __.Add (item': 'T) = SingleLinkedNode(__, item')
    member __.GetCount () =
      let rec counts(node: SingleLinkedNode<'T>, count: int) =
        if node = Unchecked.defaultof<SingleLinkedNode<'T>> then
          count
        else
          counts (node.Linked, count + 1)
      counts (__, 0)
    member __.GetNode (index: int) =
      let mutable node = __
      for i = index downto 0 do
        node <- node.Linked
      node
    member __.ToArray (count: int) =
      let array = Array.zeroCreate<'T> count
      let rec copy(node: SingleLinkedNode<'T>, index: int) =
        if node <> Unchecked.defaultof<SingleLinkedNode<'T>> then
          array[index] <- node.Item
          copy(node.Linked, index - 1)
      copy(__, count - 1)
      array

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
  /// <see href="https://github.com/JonHanna/corefx/blob/master/src/Common/src/System/Collections/Generic/LargeArrayBuilder.cs#L13">CopyPosition</see>
  [<Struct; IsReadOnly; DebuggerDisplay("{DebuggerDisplay, nq}")>]
  type CopyPosition = {
    row: int
    column: int
  } with
    static member Start with get() = { row = 0; column = 0; }
    member __.Normalize (endColumn: int) =
      if __.column = endColumn then { row = __.row + 1; column = 0}
      else __
    member __.DebuggerDisplay() = $"[%d{__.row}, %d{__.column}]"

  /// <summary>
  /// 
  /// </summary>
  /// <see href="https://github.com/JonHanna/corefx/blob/master/src/Common/src/System/Collections/Generic/ArrayBuilder.cs#L13">struct ArrayBuilder&lt;T&gt;</see>
  [<Struct;NoComparison;NoEquality>]
  type ArrayBuilder<'T> =
    val mutable array': array<'T>
    val mutable count': int
    new (capacity: int) = { 
      array' = if 0 < capacity then Array.zeroCreate<'T> capacity else Array.empty<'T>
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
        result <- Array.zeroCreate<'T> __.count'
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

      let next = Array.zeroCreate<'T> nextCapacity
      if 0 < __.count' then
        System.Array.Copy(__.array', 0, next, 0, __.count')
      __.array' <- next

  /// <summary>
  /// 
  /// </summary>
  /// <see href="https://github.com/JonHanna/corefx/blob/master/src/Common/src/System/Collections/Generic/LargeArrayBuilder.SpeedOpt.cs#L14">LargeArrayBuilder&lt;T&gt;</see>
  [<Struct;NoComparison;NoEquality>]
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

    // src: https://github.com/JonHanna/corefx/blob/master/src/Common/src/System/Collections/Generic/LargeArrayBuilder.SpeedOpt.cs#L157
    member __.CopyTo (array: array<'T>, arrayIndex: int, count: int) =
      let mutable count' = count
      let mutable arrayIndex' = arrayIndex
      let length = count - 1

      for i = 0 to length do
        let buffer = __.GetBuffer(i)
        let toCopy = min count' buffer.Length
        System.Array.Copy(buffer, 0, array,  arrayIndex', toCopy)
        
        count' <- count' - toCopy
        arrayIndex' <- arrayIndex' + toCopy

    // https://github.com/JonHanna/corefx/blob/master/src/Common/src/System/Collections/Generic/LargeArrayBuilder.SpeedOpt.cs#L186
    member __.CopyTo (position: CopyPosition, array: array<'T>, arrayIndex: int, count: int) =
      let mutable arrayIndex = arrayIndex
      let mutable count = count

      let inline copyToCore (sourceBuffer: array<'T>, sourceIndex: int) =
        let copyCount = min (sourceBuffer.Length - sourceIndex) count
        System.Array.Copy(sourceBuffer, sourceIndex, array, arrayIndex, copyCount)
        arrayIndex <- arrayIndex + copyCount
        count <- count - copyCount
        copyCount
        
      let mutable row = position.row
      let col = position.column
      let mutable buf = __.GetBuffer row
      let mutable copied = copyToCore(buf, col)

      if count = 0 then
        let p = { row = row; column = col + copied }
        p.Normalize(buf.Length)
      else
        buf <- __.GetBuffer row
        row <- row + 1
        copied <- copyToCore(buf, 0)
        
        while 0 < count do 
          buf <- __.GetBuffer row
          row <- row + 1
          copied <- copyToCore(buf, 0)
        let p = { row = row; column = copied }
        p.Normalize(buf.Length)

    member __.GetBuffer (index: int) : array<'T> =
      if index = 0 then
        __.first'
      else if index <= __.buffers'.Count then
        __.buffers'[index-1]
      else
        __.current'

    [<MethodImpl(MethodImplOptions.NoInlining)>]
    member __.SlowAdd(item: 'T) = __.Add(item)

    // src: https://github.com/JonHanna/corefx/blob/master/src/Common/src/System/Collections/Generic/LargeArrayBuilder.SpeedOpt.cs#L266
    member __.ToArray() =
      let mutable ary = Unchecked.defaultof<array<'T>>
      if __.TryMove &ary then
        ary
      else
        ary <- Array.zeroCreate<'T> __.count'
        __.CopyTo(ary, 0, __.count')
        ary

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member __.TryMove(array: outref<array<'T>>) =
      array <- __.first'
      __.count' = __.first'.Length

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

    // src: https://github.com/JonHanna/corefx/blob/master/src/Common/src/System/Collections/Generic/LargeArrayBuilder.SpeedOpt.cs#L290
    member private __.AllocateBuffer () =      
      if uint __.count' < uint ResizeLimit then
        let nextCapacity = min (if __.count' = 0 then StartingCapacity else 2 * __.count') __.maxCapacity'
        __.current' <- Array.zeroCreate<'T> nextCapacity
        System.Array.Copy(__.first', 0, __.current', 0, __.count')
        __.first' <- __.current'
      else
        let nextCapacity =
          if __.count' = ResizeLimit then ResizeLimit
          else __.buffers'.Add __.current'; min __.count' (__.maxCapacity' - __.count')
        __.current' <- Array.zeroCreate<'T> nextCapacity
        __.index' <- 0

  /// <summary>
  /// 
  /// </summary>
  // src: https://github.com/JonHanna/corefx/blob/master/src/Common/src/System/Collections/Generic/SparseArrayBuilder.cs#L13
  [<Struct; IsReadOnly; DebuggerDisplay("{DebuggerDisplay, nq}")>]
  type Marker = { count: int; index: int }
  with member __.DebuggerDisplay with get() = $"index: {__.index}, count: {__.count}"

  /// <summary>
  /// 
  /// </summary>
  // src: https://github.com/JonHanna/corefx/blob/master/src/Common/src/System/Collections/Generic/SparseArrayBuilder.cs#L49
  [<Struct;NoComparison;NoEquality;>]
  type SparseArrayBuilder<'T> = {
    mutable builder : LargeArrayBuilder<'T>
    mutable markers: ArrayBuilder<Marker>
    mutable reservedCount: int
  }
  with
    static member Create() = { builder = LargeArrayBuilder<'T>(System.Int32.MaxValue); markers = ArrayBuilder<Marker>(4); reservedCount = 0 }
    member __.Count with get() = __.builder.Count + __.reservedCount
    member __.Add (item: 'T) = __.builder.Add(item)
    member __.AddRange (items: seq<'T>) = __.builder.AddRange(items)
    member __.CopyTo (array: array<'T>, arrayIndex: int, count: int) =
      let mutable arrayIndex = arrayIndex
      let mutable count = count
      let mutable copied = 0
      let mutable position = CopyPosition.Start

      for i = 0 to __.markers.Count - 1 do
        let marker = __.markers[i]
        let toCopy = min (marker.index - copied ) count
        if 0 < toCopy then
          position <- __.builder.CopyTo(position, array, arrayIndex, toCopy)
          arrayIndex <- arrayIndex + toCopy
          copied <- copied + toCopy
          count <- count - toCopy

        if count = 0 then
          ()
        else
          let reservedCount = min marker.count count
          arrayIndex <- arrayIndex + reservedCount
          copied <- copied + reservedCount
          count <- count - reservedCount
      
      if 0 < count then
        __.builder.CopyTo(position, array, arrayIndex, count) |> ignore

    member __.Reserve (count: int) =
      __.markers.Add { count = count; index = __.Count }
      __.reservedCount <- Checked.(+) __.reservedCount count

    member __.ReserveOrAdd (items: seq<'T>) =
      let mutable itemCount = 0
      if Enumerable.tryGetCount<'T>(items, &itemCount) then
        if 0 < itemCount then __.Reserve(itemCount); true
        else false
      else
        __.AddRange(items)
        false

    member __.ToArray() =
      if __.markers.Count = 0 then
        __.builder.ToArray()
      else
        let array = Array.zeroCreate __.Count
        __.CopyTo(array, 0, array.Length)
        array

module Select =
  open Basis

  let inline defaultval<'T> = Unchecked.defaultof<'T>
  let inline combine_selectors ([<InlineIfLambda>] lhs: 'T -> 'U) ([<InlineIfLambda>] rhs: 'U -> 'V) = lhs >> rhs
  
  // src: https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/Select.SpeedOpt.cs#L675
  [<Sealed>]
  type SelectListPartitionIterator<'T, 'U>(source: IList<'T>, [<InlineIfLambda>]selector: 'T -> 'U, minIndexInclusive: int, maxIndexInclusive: int) =
    inherit Iterator<'U>()
    override __.Clone() = new SelectListPartitionIterator<'T, 'U>(source, selector, minIndexInclusive, maxIndexInclusive)
    
    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    override __.MoveNext() =
      let index = __.state - 1
      if uint index <= uint (maxIndexInclusive - minIndexInclusive) && index < (source.Count - minIndexInclusive) then
        __.current <- selector (source[minIndexInclusive + index])
        __.state <- __.state + 1
        true
      else
        __.Dispose()
        false
    
    override __.Select<'U2> (selector': 'U -> 'U2) =
      new SelectListPartitionIterator<'T, 'U2>(source, combine_selectors selector selector', minIndexInclusive, maxIndexInclusive)

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member __.Skip (count: int) : Funtom.linq.Interfaces.IPartition<'U> =
      let minIndex = minIndexInclusive + count
      if uint maxIndexInclusive < uint minIndex
      then EmptyPartition<'U>.Instance
      else new SelectListPartitionIterator<'T, 'U>(source, selector, minIndex, maxIndexInclusive)

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member __.Take (count: int) : Funtom.linq.Interfaces.IPartition<'U> =
      let maxIndex = minIndexInclusive + count - 1
      if uint maxIndexInclusive <= uint maxIndex
      then __
      else new SelectListPartitionIterator<'T, 'U>(source, selector, minIndexInclusive, maxIndex)

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member __.TryGetElementAt(index: int, found: outref<bool>) =
      if uint index <= uint (maxIndexInclusive - minIndexInclusive) && index < (source.Count - minIndexInclusive)
      then found <- true; selector(source[minIndexInclusive + index])
      else found <- false; Unchecked.defaultof<'U>
      
    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member __.TryGetFirst(found: outref<bool>) =
      if minIndexInclusive < source.Count
      then found <- true; selector(source[minIndexInclusive])
      else found <- false; Unchecked.defaultof<'U>
      
    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member __.TryGetLast(found: outref<bool>) =
      let lastIndex = source.Count - 1
      if minIndexInclusive <= lastIndex
      then found <- true; selector(source[min lastIndex maxIndexInclusive])
      else found <- false; Unchecked.defaultof<'U>
      
    member private __.Count with get () =
      let count = source.Count
      if count <= minIndexInclusive
      then 0
      else (min (count - 1) maxIndexInclusive) - minIndexInclusive + 1

    [<MethodImpl(MethodImplOptions.AggressiveOptimization)>]
    member __.ToArray () =
      let count = __.Count
      if count = 0
      then Array.Empty<'U>()
      else
        let array = Array.zeroCreate<'U>(count)
        for i = 0 to array.Length do 
          let j = minIndexInclusive + i
          array[i] <- selector(source[j])
        array
    
    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member __.ToList () =
      let count = __.Count
      if count = 0
      then ResizeArray<'U>()
      else
        let list = ResizeArray<'U>(count)
        let endIndex = minIndexInclusive + count - 1
        for i = minIndexInclusive to endIndex do
          list.Add(selector source[i])
        list

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member __.GetCount (onlyIfCheap: bool) =
      let count = __.Count
      if not onlyIfCheap then
        let endIndex = minIndexInclusive + count - 1
        for i = minIndexInclusive to endIndex do
          selector source[i] |> ignore
      count

    interface Funtom.linq.Interfaces.IListProvider<'U> with
      member __.ToArray() = __.ToArray()
      member __.ToList() = __.ToList()
      member __.GetCount(onlyIfCheap: bool) = __.GetCount(onlyIfCheap)

    interface Funtom.linq.Interfaces.IPartition<'U> with
      member __.Skip(count: int) = __.Skip(count)
      member __.Take(count: int) = __.Take(count)
      member __.TryGetElementAt(index: int, found: outref<bool>) = __.TryGetElementAt(index, &found)
      member __.TryGetFirst(found: outref<bool>) = __.TryGetFirst(&found)
      member __.TryGetLast(found: outref<bool>) = __.TryGetLast(&found)

  // src: https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/Select.cs#L98
  // src: https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/Select.SpeedOpt.cs#L21
  [<Sealed>]
  type SelectEnumerableIterator<'T, 'U> (source: seq<'T>, [<InlineIfLambda>]selector: 'T -> 'U) =
    inherit Iterator<'U>()
    let mutable enumerator : IEnumerator<'T> = source.GetEnumerator()
    
    override __.Clone() = new SelectEnumerableIterator<'T, 'U>(source, selector)

    override __.Dispose() =
      if enumerator <> defaultval<IEnumerator<'T>> then
        enumerator.Dispose()
        enumerator <- defaultval<IEnumerator<'T>>
      base.Dispose()
      
    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    override __.MoveNext() =
      if enumerator.MoveNext()
      then __.current <- selector(enumerator.Current); true
      else __.Dispose(); false
    
    override __.Select<'U2> (selector': 'U -> 'U2) =
      new SelectEnumerableIterator<'T, 'U2>(source, combine_selectors selector selector')
    
    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member __.ToArray () =
      let mutable builder = LargeArrayBuilder<'U>(Int32.MaxValue)
      for item in source do
        builder.Add(selector item)
      builder.ToArray()
    
    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member __.ToList () =
      let mutable list = ResizeArray()
      for item in source do
        list.Add(selector item)
      list

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member __.GetCount (onlyIfCheap: bool) =
      if onlyIfCheap then 
        -1
      else
        let mutable count = 0
        for item in source do
          count <- Checked.(+) count 1
        count

    interface Funtom.linq.Interfaces.IListProvider<'U> with
      member __.ToArray() = __.ToArray()
      member __.ToList() = __.ToList()
      member __.GetCount(onlyIfCheap: bool) = __.GetCount(onlyIfCheap)

  // src: https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/Select.cs#L159
  // src: https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/Select.SpeedOpt.cs#L72
  [<Sealed>]
  type SelectArrayIterator<'T, 'U> (source: 'T[], [<InlineIfLambda>]selector: 'T -> 'U) =
    inherit Iterator<'U>()
    override __.Clone() = new SelectArrayIterator<'T, 'U>(source, selector)
      
    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    override __.MoveNext() =
      if __.state < 1 || __.state = source.Length + 1 then
        __.Dispose()
        false
      else
        let index = __.state - 1
        __.state <- __.state + 1
        __.current <- selector(source[index])
        true
    
    override __.Select<'U2> (selector': 'U -> 'U2) =
      new SelectArrayIterator<'T, 'U2>(source, combine_selectors selector selector')
    
    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member __.ToArray () =
      let length = source.Length
      let results = Array.zeroCreate<'U>(length)
      let rec copy(i: int) =
        if i < length then
          results[i] <- selector(source[i])
          copy(i + 1)
      copy(0)
      results
    
    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member __.ToList () =
      let length = source.Length
      let results = ResizeArray<'U>(length)
      let rec copy(i: int) =
        if i < length then
          results.Add(selector(source[i]))
          copy(i + 1)
      copy(0)
      results

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member __.GetCount (onlyIfCheap: bool) =
      if not onlyIfCheap then 
        for v in source do selector v |> ignore
      source.Length
      
    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member __.Skip (count: int) : Funtom.linq.Interfaces.IPartition<'U> =
      if count >= source.Length
      then EmptyPartition<'U>.Instance
      else new SelectListPartitionIterator<'T, 'U>(source, selector, count, Int32.MaxValue)
      
    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member __.Take (count: int) : Funtom.linq.Interfaces.IPartition<'U> =
      if count >= source.Length 
      then __
      else new SelectListPartitionIterator<'T, 'U>(source, selector, 0, count - 1)
      
    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member __.TryGetElementAt(index: int, found: outref<bool>) =
      if uint index < uint source.Length
      then found <- true; selector(source[index])
      else found <- false; Unchecked.defaultof<'U>
      
    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member __.TryGetFirst(found: outref<bool>) =
      found <- true
      selector(source[0])
      
    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member __.TryGetLast(found: outref<bool>) =
      found <- true
      selector(source[source.Length - 1])

    interface Funtom.linq.Interfaces.IListProvider<'U> with
      member __.ToArray() = __.ToArray()
      member __.ToList() = __.ToList()
      member __.GetCount(onlyIfCheap: bool) = __.GetCount(onlyIfCheap)

    interface Funtom.linq.Interfaces.IPartition<'U> with
      member __.Skip(count: int) = __.Skip(count)
      member __.Take(count: int) = __.Take(count)
      member __.TryGetElementAt(index: int, found: outref<bool>) = __.TryGetElementAt(index, &found)
      member __.TryGetFirst(found: outref<bool>) = __.TryGetFirst(&found)
      member __.TryGetLast(found: outref<bool>) = __.TryGetLast(&found)
  
  // src: https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/Select.cs#L200
  // src: https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/Select.SpeedOpt.cs#L291
  [<Sealed>]
  type SelectListIterator<'T, 'U> (source: ResizeArray<'T>, [<InlineIfLambda>]selector: 'T -> 'U) =
    inherit Iterator<'U>()
    let mutable enumerator = source.GetEnumerator()
    override __.Clone() = new SelectListIterator<'T, 'U>(source, selector)
    
    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    override __.MoveNext() =
      if enumerator.MoveNext()
      then __.current <- selector(enumerator.Current); true // Unsafe.As<obj, 'T>(ref enumerator.Current)
      else __.Dispose(); false
    
    override __.Select<'U2> (selector': 'U -> 'U2) =
      new SelectListIterator<'T, 'U2>(source, combine_selectors selector selector')
    
    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member __.ToArray () =
      let count = source.Count
      if count = 0 then 
        Array.Empty<'U>()
      else
        let results = Array.zeroCreate<'U>(count)
        let length = count - 1
        for i = 0 to length do
          results[i] <- selector(source[i])
        results
        
    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member __.ToList () =
      let count = source.Count - 1
      let results = ResizeArray<'U>(count + 1)
      for i = 0 to count do
        results.Add(selector(source[i]))
      results

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member __.GetCount (onlyIfCheap: bool) =
      let count = source.Count
      if not onlyIfCheap then
        for i = 0 to count - 1 do
          selector(source[i]) |> ignore
      count
    
    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member __.Skip (count: int) =
      new SelectListPartitionIterator<'T, 'U>(source, selector, count, Int32.MaxValue);
  
    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member __.Take (count: int) =
      new SelectListPartitionIterator<'T, 'U>(source, selector, 0, count - 1)
    
    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member __.TryGetElementAt(index: int, found: outref<bool>) =
      if uint index < uint source.Count
      then found <- true; selector(source[index])
      else found <- false; Unchecked.defaultof<'U>
    
    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member __.TryGetFirst(found: outref<bool>) =
      if 0 < source.Count
      then found <- true; selector(source[0])
      else found <- false; Unchecked.defaultof<'U>
    
    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member __.TryGetLast(found: outref<bool>) =
      let count = source.Count
      if 0 < count
      then found <- true; selector(source[count - 1])
      else found <- false; Unchecked.defaultof<'U>

    interface Funtom.linq.Interfaces.IListProvider<'U> with
      member __.ToArray() = __.ToArray()
      member __.ToList() = __.ToList()
      member __.GetCount(onlyIfCheap: bool) = __.GetCount(onlyIfCheap)

    interface Funtom.linq.Interfaces.IPartition<'U> with
      member __.Skip(count: int) = __.Skip(count)
      member __.Take(count: int) = __.Take(count)
      member __.TryGetElementAt(index: int, found: outref<bool>) = __.TryGetElementAt(index, &found)
      member __.TryGetFirst(found: outref<bool>) = __.TryGetFirst(&found)
      member __.TryGetLast(found: outref<bool>) = __.TryGetLast(&found)

  // src: https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/Select.cs#L250
  // src: https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/Select.SpeedOpt.cs#L390
  [<Sealed>]
  type SelectIListIterator<'T, 'U> (source: IList<'T>, [<InlineIfLambda>]selector: 'T -> 'U) =
    inherit Iterator<'U>()
    let mutable enumerator = source.GetEnumerator()
    override __.Clone() = new SelectIListIterator<'T, 'U>(source, selector)
    
    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    override __.MoveNext() =
      if enumerator.MoveNext()
      then __.current <- selector(enumerator.Current); true
      else __.Dispose(); false

    override __.Dispose() =
      if enumerator <> Unchecked.defaultof<IEnumerator<'T>> then
        enumerator.Dispose()
        enumerator <- Unchecked.defaultof<IEnumerator<'T>>
      base.Dispose()

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    override __.Select<'U2> (selector': 'U -> 'U2) =
      new SelectIListIterator<'T, 'U2>(source, combine_selectors selector selector')
    
    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member __.ToArray () =
      let count = source.Count
      if count = 0 then 
        Array.Empty<'U>()
      else
        let results = Array.zeroCreate<'U>(count)
        let length = count - 1
        for i = 0 to length do
          results[i] <- selector(source[i])
        results
        
    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member __.ToList () =
      let count = source.Count - 1
      let results = ResizeArray<'U>(count + 1)
      for i = 0 to count do
        results.Add(selector(source[i]))
      results

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member __.GetCount (onlyIfCheap: bool) =
      let count = source.Count
      if not onlyIfCheap then
        for i = 0 to count - 1 do
          selector(source[i]) |> ignore
      count
    
    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member __.Skip (count: int) =
      new SelectListPartitionIterator<'T, 'U>(source, selector, count, Int32.MaxValue);
      
    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member __.Take (count: int) =
      new SelectListPartitionIterator<'T, 'U>(source, selector, 0, count - 1)
    
    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member __.TryGetElementAt(index: int, found: outref<bool>) =
      if uint index < uint source.Count
      then found <- true; selector(source[index])
      else found <- false; Unchecked.defaultof<'U>
    
    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member __.TryGetFirst(found: outref<bool>) =
      if 0 < source.Count
      then found <- true; selector(source[0])
      else found <- false; Unchecked.defaultof<'U>
    
    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member __.TryGetLast(found: outref<bool>) =
      let count = source.Count
      if 0 < count
      then found <- true; selector(source[count - 1])
      else found <- false; Unchecked.defaultof<'U>

    interface Funtom.linq.Interfaces.IListProvider<'U> with
      member __.ToArray() = __.ToArray()
      member __.ToList() = __.ToList()
      member __.GetCount(onlyIfCheap: bool) = __.GetCount(onlyIfCheap)

    interface Funtom.linq.Interfaces.IPartition<'U> with
      member __.Skip(count: int) = __.Skip(count)
      member __.Take(count: int) = __.Take(count)
      member __.TryGetElementAt(index: int, found: outref<bool>) = __.TryGetElementAt(index, &found)
      member __.TryGetFirst(found: outref<bool>) = __.TryGetFirst(&found)
      member __.TryGetLast(found: outref<bool>) = __.TryGetLast(&found)

  // src: https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/Select.SpeedOpt.cs#L494
  [<Sealed>]
  type SelectIPartitionIterator<'T, 'U>(source: Funtom.linq.Interfaces.IPartition<'T>, [<InlineIfLambda>]selector: 'T -> 'U) =
    inherit Iterator<'U>()
    let mutable enumerator = source.GetEnumerator()

    let lazyToArray() =
      let mutable builder = LargeArrayBuilder<'U>(Int32.MaxValue)
      for item in source do builder.Add(selector item)
      builder.ToArray()

    let preallocatingToArray(count: int) =
      let array = Array.zeroCreate<'U>(count)
      let mutable i = 0
      for item in source do
        array[i] <- (selector item)
        i <- i + 1
      array

    override __.Clone() = new SelectIPartitionIterator<'T, 'U>(source, selector)
    
    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    override __.MoveNext() =
      if enumerator.MoveNext()
      then __.current <- selector(enumerator.Current); true
      else __.Dispose(); false
      
    override __.Dispose() =
      if enumerator <> Unchecked.defaultof<IEnumerator<'T>> then
        enumerator.Dispose()
        enumerator <- Unchecked.defaultof<IEnumerator<'T>>
      base.Dispose()
      
    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    override __.Select<'U2> (selector': 'U -> 'U2) =
      new SelectIPartitionIterator<'T, 'U2>(source, combine_selectors selector selector')

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member __.ToArray () =
      let count = source.GetCount(true)
      match count with
      | -1 -> lazyToArray()
      | 0 -> Array.Empty<'U>()
      | _ -> preallocatingToArray(count)

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member __.ToList () =
      let count = source.GetCount(true)
      let list =
        match count with
        | -1 | 0 -> ResizeArray<'U>()
        | _ -> ResizeArray<'U>(count)
      for item in source do list.Add(selector item)
      list

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member __.GetCount (onlyIfCheap: bool) =
      if not onlyIfCheap then
        let mutable count = 0
        for item in source do 
          selector item |> ignore
          count <- Checked.(+) count 1
        count
      else
        source.GetCount(true)
    
    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member __.Skip (count: int) = new SelectIPartitionIterator<'T, 'U>(source.Skip(count), selector);
      
    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member __.Take (count: int) = new SelectIPartitionIterator<'T, 'U>(source.Take(count), selector)
    
    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member __.TryGetElementAt(index: int, found: outref<bool>) =
      let (item, srcFound) = source.TryGetElementAt(index)
      found <- srcFound
      if srcFound then selector item else Unchecked.defaultof<'U>
    
    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member __.TryGetFirst(found: outref<bool>) =
      let (item, srcFound) = source.TryGetFirst()
      found <- srcFound
      if srcFound then selector item else Unchecked.defaultof<'U>
    
    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member __.TryGetLast(found: outref<bool>) =
      let (item, srcFound) = source.TryGetLast()
      found <- srcFound
      if srcFound then selector item else Unchecked.defaultof<'U>

    interface Funtom.linq.Interfaces.IListProvider<'U> with
      member __.ToArray() = __.ToArray()
      member __.ToList() = __.ToList()
      member __.GetCount(onlyIfCheap: bool) = __.GetCount(onlyIfCheap)

    interface Funtom.linq.Interfaces.IPartition<'U> with
      member __.Skip(count: int) = __.Skip(count)
      member __.Take(count: int) = __.Take(count)
      member __.TryGetElementAt(index: int, found: outref<bool>) = __.TryGetElementAt(index, &found)
      member __.TryGetFirst(found: outref<bool>) = __.TryGetFirst(&found)
      member __.TryGetLast(found: outref<bool>) = __.TryGetLast(&found)