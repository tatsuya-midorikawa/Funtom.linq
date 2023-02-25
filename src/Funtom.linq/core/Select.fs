namespace rec Funtom.linq.core

open System
open System.Collections
open System.Collections.Generic
open System.Runtime.CompilerServices
open Funtom.linq
open Funtom.linq.Interfaces
open Funtom.linq.iterator.Empty

[<Sealed>]
type SelectArrayIterator<'T, 'U> (source: 'T[], [<InlineIfLambda>] selector: 'T -> 'U) =
  let tid = Environment.CurrentManagedThreadId
  let mutable current = defaultof<'U> 
  let mutable state = 0

  member private __.State with get() = state and set v = state <- v
  member __.Current with get() = current
  member __.Reset () = NotSupportedException() |> raise
  [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
  member __.MoveNext() =
    let i = state
    if i < 1 || i = source.Length + 1
      then __.Dispose(); false
      else
        state <- i + 1
        current <- selector(source[i - 1])
        true
  
  member __.Clone () = new SelectArrayIterator<'T, 'U>(source, selector)
  member __.GetEnumerator () =
    let mutable enumerator =
      if state = 0 && tid = Environment.CurrentManagedThreadId
        then __
        else __.Clone()
    enumerator.State <- 1
    enumerator
  member __.Dispose () = 
    current <- defaultof<'U>
    state <- 0

  member __.Select<'U2> (selector': 'U -> 'U2) =
    new SelectArrayIterator<'T, 'U2>(source, combine_selectors selector selector')
  
  [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
  member __.ToArray () =
    let length = source.Length
    let results = Array.zeroCreate<'U>(length)
    let rec copy(i: int) =
      if i < length
        then
          results[i] <- selector(source[i])
          copy(i + 1)
    copy(0)
    results
  
  [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
  member __.ToList () =
    let length = source.Length
    let results = ResizeArray<'U>(length)
    let rec copy(i: int) =
      if i < length
        then
          results.Add(selector(source[i]))
          copy(i + 1)
    copy(0)
    results

  [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
  member __.GetCount (onlyIfCheap: bool) =
    if not onlyIfCheap
      then for v in source do selector v |> ignore
    source.Length
    
  [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
  member __.Skip (count: int) :  IPartition<'U> =
    if count >= source.Length
      then EmptyPartition<'U>.Instance
      else new SelectListPartitionIterator<'T, 'U>(source, selector, count, Int32.MaxValue)
    
  [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
  member __.Take (count: int) : IPartition<'U> =
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

  interface IListProvider<'U> with
    member __.ToArray() = __.ToArray()
    member __.ToList() = __.ToList()
    member __.GetCount(onlyIfCheap: bool) = __.GetCount(onlyIfCheap)

  interface IPartition<'U> with
    member __.Skip(count: int) = __.Skip(count)
    member __.Take(count: int) = __.Take(count)
    member __.TryGetElementAt(index: int, found: outref<bool>) = __.TryGetElementAt(index, &found)
    member __.TryGetFirst(found: outref<bool>) = __.TryGetFirst(&found)
    member __.TryGetLast(found: outref<bool>) = __.TryGetLast(&found)

  interface IDisposable with member __.Dispose () = __.Dispose()
  interface IEnumerator with
    member __.MoveNext() = __.MoveNext()
    member __.Current with get() = current
    member __.Reset () = __.Reset ()
  interface IEnumerator<'U> with member __.Current with get() = current
  interface IEnumerable with member __.GetEnumerator () = __.GetEnumerator ()
  interface IEnumerable<'U> with member __.GetEnumerator () = __.GetEnumerator ()

[<AbstractClass>]
type Iterator<'T> () =
  let tid = Environment.CurrentManagedThreadId
  let mutable current' = defaultof<'T> 
  [<DefaultValue>] val mutable internal state : int
  member val internal current = current' with get, set

  abstract member Dispose : unit -> unit
  default __.Dispose () = 
    current' <- defaultof<'T>
    __.state <- 0
  
  abstract member MoveNext : unit -> bool
  abstract member Current : 'T with get
  default __.Current with get() = current'
  abstract member Reset : unit -> unit
  default __.Reset () = NotSupportedException() |> raise

  abstract member GetEnumerator : unit -> IEnumerator<'T>
  default __.GetEnumerator () =
    let mutable enumerator =
      if __.state = 0 && tid = Environment.CurrentManagedThreadId
        then __
        else __.Clone()
    enumerator.state <- 1
    enumerator

  abstract member Select<'U> : ('T -> 'U) -> seq<'U>
  default __.Select<'U> (selector: 'T -> 'U) = new SelectEnumerableIterator<'T, 'U>(__, selector)

  //// TODO: implement default
  //abstract member Where: ('T -> bool) -> seq<'T>

  abstract member Clone : unit -> Iterator<'T>

  interface IDisposable with member __.Dispose () = __.Dispose()
  interface IEnumerator with
    member __.MoveNext() = __.MoveNext()
    member __.Current with get() = current'
    member __.Reset () = __.Reset ()
  interface IEnumerator<'T> with member __.Current with get() = current'
  interface IEnumerable with member __.GetEnumerator () = __.GetEnumerator ()
  interface IEnumerable<'T> with member __.GetEnumerator () = __.GetEnumerator ()

// src: https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/Select.SpeedOpt.cs#L675
[<Sealed>]
type SelectListPartitionIterator<'T, 'U>(source: IList<'T>, [<InlineIfLambda>]selector: 'T -> 'U, minIndexInclusive: int, maxIndexInclusive: int) =
  inherit Iterator<'U>()
  override __.Clone() = new SelectListPartitionIterator<'T, 'U>(source, selector, minIndexInclusive, maxIndexInclusive)
  
  [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
  override __.MoveNext() =
    let index = __.state - 1
    if uint index <= uint (maxIndexInclusive - minIndexInclusive) && index < (source.Count - minIndexInclusive)
      then
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
    if not onlyIfCheap
      then
        let endIndex = minIndexInclusive + count - 1
        for i = minIndexInclusive to endIndex do
          selector source[i] |> ignore
    count

  interface IListProvider<'U> with
    member __.ToArray() = __.ToArray()
    member __.ToList() = __.ToList()
    member __.GetCount(onlyIfCheap: bool) = __.GetCount(onlyIfCheap)

  interface Interfaces.IPartition<'U> with
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
    if enumerator <> defaultof<IEnumerator<'T>>
      then
        enumerator.Dispose()
        enumerator <- defaultof<IEnumerator<'T>>
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
    if onlyIfCheap
      then -1
      else
        let mutable count = 0
        for item in source do
          count <- Checked.(+) count 1
        count

  interface IListProvider<'U> with
    member __.ToArray() = __.ToArray()
    member __.ToList() = __.ToList()
    member __.GetCount(onlyIfCheap: bool) = __.GetCount(onlyIfCheap)

// // src: https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/Select.cs#L159
// // src: https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/Select.SpeedOpt.cs#L72
// [<Sealed>]
// [<Obsolete("Use SelectArrayIterator<'T, 'U>")>]
// type SelectArrayIteratorObsolete<'T, 'U> (source: 'T[], [<InlineIfLambda>]selector: 'T -> 'U) =
//   inherit Iterator<'U>()
//   override __.Clone() = new SelectArrayIteratorObsolete<'T, 'U>(source, selector)
//
//   [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
//   override __.MoveNext() =
//     let i = __.state
//     if i < 1 || i = source.Length + 1
//       then
//         __.Dispose()
//         false
//       else
//         __.state <- i + 1
//         __.current <- selector(source[i - 1])
//         true
//
//   override __.Select<'U2> (selector': 'U -> 'U2) =
//     new SelectArrayIteratorObsolete<'T, 'U2>(source, combine_selectors selector selector')
//
//   [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
//   member __.ToArray () =
//     let length = source.Length
//     let results = Array.zeroCreate<'U>(length)
//     let rec copy(i: int) =
//       if i < length
//         then
//           results[i] <- selector(source[i])
//           copy(i + 1)
//     copy(0)
//     results
//
//   [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
//   member __.ToList () =
//     let length = source.Length
//     let results = ResizeArray<'U>(length)
//     let rec copy(i: int) =
//       if i < length
//         then
//           results.Add(selector(source[i]))
//           copy(i + 1)
//     copy(0)
//     results
//
//   [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
//   member __.GetCount (onlyIfCheap: bool) =
//     if not onlyIfCheap
//       then for v in source do selector v |> ignore
//     source.Length
//
//   [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
//   member __.Skip (count: int) :  IPartition<'U> =
//     if count >= source.Length
//       then EmptyPartition<'U>.Instance
//       else new SelectListPartitionIterator<'T, 'U>(source, selector, count, Int32.MaxValue)
//
//   [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
//   member __.Take (count: int) : IPartition<'U> =
//     if count >= source.Length 
//       then __
//       else new SelectListPartitionIterator<'T, 'U>(source, selector, 0, count - 1)
//   
//   [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
//   member __.TryGetElementAt(index: int, found: outref<bool>) =
//     if uint index < uint source.Length
//       then found <- true; selector(source[index])
//       else found <- false; Unchecked.defaultof<'U>
//
//   [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
//   member __.TryGetFirst(found: outref<bool>) =
//     found <- true
//     selector(source[0])
//
//   [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
//   member __.TryGetLast(found: outref<bool>) =
//     found <- true
//     selector(source[source.Length - 1])
//
//   interface IListProvider<'U> with
//     member __.ToArray() = __.ToArray()
//     member __.ToList() = __.ToList()
//     member __.GetCount(onlyIfCheap: bool) = __.GetCount(onlyIfCheap)
//
//   interface IPartition<'U> with
//     member __.Skip(count: int) = __.Skip(count)
//     member __.Take(count: int) = __.Take(count)
//     member __.TryGetElementAt(index: int, found: outref<bool>) = __.TryGetElementAt(index, &found)
//     member __.TryGetFirst(found: outref<bool>) = __.TryGetFirst(&found)
//     member __.TryGetLast(found: outref<bool>) = __.TryGetLast(&found)
//

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
    if count = 0
      then Array.Empty<'U>()
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
    if not onlyIfCheap
      then
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

  interface IListProvider<'U> with
    member __.ToArray() = __.ToArray()
    member __.ToList() = __.ToList()
    member __.GetCount(onlyIfCheap: bool) = __.GetCount(onlyIfCheap)

  interface IPartition<'U> with
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
    if enumerator <> Unchecked.defaultof<IEnumerator<'T>>
      then
        enumerator.Dispose()
        enumerator <- Unchecked.defaultof<IEnumerator<'T>>
    base.Dispose()

  [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
  override __.Select<'U2> (selector': 'U -> 'U2) =
    new SelectIListIterator<'T, 'U2>(source, combine_selectors selector selector')
  
  [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
  member __.ToArray () =
    let count = source.Count
    if count = 0
      then Array.Empty<'U>()
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
    if not onlyIfCheap
      then
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

  interface IListProvider<'U> with
    member __.ToArray() = __.ToArray()
    member __.ToList() = __.ToList()
    member __.GetCount(onlyIfCheap: bool) = __.GetCount(onlyIfCheap)

  interface IPartition<'U> with
    member __.Skip(count: int) = __.Skip(count)
    member __.Take(count: int) = __.Take(count)
    member __.TryGetElementAt(index: int, found: outref<bool>) = __.TryGetElementAt(index, &found)
    member __.TryGetFirst(found: outref<bool>) = __.TryGetFirst(&found)
    member __.TryGetLast(found: outref<bool>) = __.TryGetLast(&found)

// src: https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/Select.SpeedOpt.cs#L494
[<Sealed>]
type SelectIPartitionIterator<'T, 'U>(source: IPartition<'T>, [<InlineIfLambda>]selector: 'T -> 'U) =
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
    if enumerator <> Unchecked.defaultof<IEnumerator<'T>>
      then
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
    if not onlyIfCheap
      then
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

  interface IListProvider<'U> with
    member __.ToArray() = __.ToArray()
    member __.ToList() = __.ToList()
    member __.GetCount(onlyIfCheap: bool) = __.GetCount(onlyIfCheap)

  interface IPartition<'U> with
    member __.Skip(count: int) = __.Skip(count)
    member __.Take(count: int) = __.Take(count)
    member __.TryGetElementAt(index: int, found: outref<bool>) = __.TryGetElementAt(index, &found)
    member __.TryGetFirst(found: outref<bool>) = __.TryGetFirst(&found)
    member __.TryGetLast(found: outref<bool>) = __.TryGetLast(&found)

// /// <summary>
// /// 
// /// </summary>
// [<NoComparison;NoEquality;Sealed;>]
// type SelectEnumerator<'T, 'U> (iter: IEnumerator<'T>, [<InlineIfLambda>] selector: 'T -> 'U) =
//   let mutable current : 'U = defaultof<'U>
//   [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
//   let rec move_next () =
//     if iter.MoveNext()
//       then current <- selector iter.Current; true
//       else false

//   [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
//   member __.Dispose() = ()
//   [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
//   member __.MoveNext() = move_next ()
//   member __.Current with get(): 'U = current
//   [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
//   member __.Reset() = ()
  
//   interface IDisposable with
//     [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
//     member __.Dispose () = __.Dispose ()

//   interface IEnumerator with
//     [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
//     member __.MoveNext () = move_next ()
//     member __.Current with get() = __.Current
//     [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
//     member __.Reset () = __.Reset()
    
//   interface IEnumerator<'U> with
//     member __.Current with get() = __.Current

// /// <summary>
// /// 
// /// </summary>
// [<NoComparison;NoEquality;Sealed;>]
// type SelectIterator<'T, 'U> (src: seq<'T>, [<InlineIfLambda>] selector: 'T -> 'U) =
//   let get_enumerator () = new SelectEnumerator<'T, 'U> (src.GetEnumerator(), selector)
//   [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
//   member __.select<'V> (selector': 'U -> 'V) = SelectIterator<'T, 'V>(src, (combine_selectors selector selector'))

//   interface IEnumerable with
//     [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
//     member __.GetEnumerator () = get_enumerator ()
    
//   interface IEnumerable<'U> with
//     [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
//     member __.GetEnumerator () = get_enumerator ()

// /// <summary>
// /// 
// /// </summary>
// [<NoComparison;NoEquality;Sealed;>]
// type SelectArrayEnumerator<'T, 'U> (src: array<'T>, [<InlineIfLambda>] selector: 'T -> 'U) =
//   let mutable current : 'U = defaultof<'U>
//   let mutable i : int = 0
//   let dispose () = ()
//   let move_next () =
//     if i < src.Length
//       then current <- selector src[i]; i <- i + 1; true
//       else dispose (); false
//   let current (): 'U = current
//   let reset () = ()

//   [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
//   member __.Dispose() = dispose ()
//   [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
//   member __.MoveNext() = move_next ()
//   member __.Current with get() : 'U = current ()
//   [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
//   member __.Reset() = reset ()
  
//   interface IDisposable with
//     [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
//     member __.Dispose () = dispose ()

//   interface IEnumerator with
//     [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
//     member __.MoveNext () = move_next ()
//     member __.Current with get() = current ()
//     [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
//     member __.Reset () = reset ()

//   interface IEnumerator<'U> with
//     member __.Current with get() = current ()

// /// <summary>
// /// 
// /// </summary>
// [<NoComparison;NoEquality;Sealed;>]
// type SelectArrayIterator<'T, 'U> (src: array<'T>, [<InlineIfLambda>] selector: 'T -> 'U) =
//   let get_enumerator () = new SelectArrayEnumerator<'T, 'U> (src, selector)
//   member __.select<'V> (selector': 'U -> 'V) = SelectArrayIterator<'T, 'V>(src, (combine_selectors selector selector'))

//   interface IEnumerable with
//     [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
//     member __.GetEnumerator () = get_enumerator ()

//   interface IEnumerable<'U> with
//     [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
//     member __.GetEnumerator () = get_enumerator ()
    

// /// <summary>
// /// 
// /// </summary>
// module SelectListIterator =
//   let inline create ([<InlineIfLambda>]selector: 'T -> 'R) (src: ResizeArray<'T>) = 
//     { 
//       SelectListIterator.src = src
//       selector = selector
//       tid = get_tid()
//       current = defaultof<'R>
//       idx = 0
//     }
//   let inline get_enumerator (iter: SelectListIterator<'source, 'result>) = 
//     if iter.tid = get_tid()
//       then iter
//       else { iter with tid = get_tid(); current = defaultof<'result> }
//   let inline move_next (iter: SelectListIterator<'source, 'result>) =
//     if uint iter.idx < uint iter.src.Count
//       then
//         iter.current <- iter.selector iter.src[iter.idx]
//         iter.idx <- iter.idx + 1
//         true
//       else
//         false

// /// <summary>
// /// 
// /// </summary>
// [<NoComparison;NoEquality>]
// type SelectListIterator<'src, 'res> =
//   {
//     src: ResizeArray<'src>
//     selector: 'src -> 'res
//     tid : int
//     mutable current : 'res
//     mutable idx : int
//   }
//   interface IDisposable with
//     [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
//     member __.Dispose () = ()

//   interface IEnumerator with
//     [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
//     member __.MoveNext () : bool = SelectListIterator.move_next __
//     member __.Current with get() = __.current :> obj
//     [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
//     member __.Reset () = raise(NotSupportedException "not supported")

//   interface IEnumerator<'res> with
//     member __.Current with get() = __.current

//   interface IEnumerable with
//     [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
//     member __.GetEnumerator () = SelectListIterator.get_enumerator __

//   interface IEnumerable<'res> with
//     [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
//     member __.GetEnumerator () = SelectListIterator.get_enumerator __

// /// <summary>
// /// 
// /// </summary>
// module SelectFsListIterator =
//   let inline create ([<InlineIfLambda>]selector: 'T -> 'R) (source: list<'T>) = 
//     { 
//       SelectFsListIterator.selector = selector
//       thread_id = Environment.CurrentManagedThreadId
//       current = Unchecked.defaultof<'R>
//       cache = source
//     }
//   let inline get_enumerator (iter: SelectFsListIterator<'source, 'result>) = 
//     if iter.thread_id = Environment.CurrentManagedThreadId then iter
//     else { iter with thread_id = Environment.CurrentManagedThreadId; current = Unchecked.defaultof<'result> }
//   let inline move_next (iter: SelectFsListIterator<'source, 'result>) =
//     match iter.cache with
//     | h::tail ->
//       iter.current <- iter.selector h
//       iter.cache <- tail
//       true
//     | _ ->
//       false

// /// <summary>
// /// 
// /// </summary>
// [<NoComparison;NoEquality>]
// type SelectFsListIterator<'T, 'R> =
//   {
//     selector: 'T -> 'R
//     thread_id : int
//     mutable current : 'R
//     mutable cache : list<'T>
//   }
//   interface IDisposable with
//     [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
//     member __.Dispose () = ()

//   interface IEnumerator with
//     [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
//     member __.MoveNext () : bool = SelectFsListIterator.move_next __
//     member __.Current with get() = __.current :> obj
//     [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
//     member __.Reset () = raise(NotSupportedException "not supported")

//   interface IEnumerator<'R> with
//     member __.Current with get() = __.current

//   interface IEnumerable with
//     [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
//     member __.GetEnumerator () = SelectFsListIterator.get_enumerator __

//   interface IEnumerable<'R> with
//     [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
//     member __.GetEnumerator () = SelectFsListIterator.get_enumerator __