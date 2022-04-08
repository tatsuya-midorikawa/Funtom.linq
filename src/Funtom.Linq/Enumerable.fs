namespace Funtom.Linq

open System
open System.Collections
open System.Collections.Generic
open Funtom.Linq.Interfaces

module Enumerable =

  [<Literal>]
  let private maxArrayLength = 0x7FEFFFFF
  [<Literal>]
  let private defaultCapacity = 4

  // TODO: 高速化対象. IL 見比べて, 原因調査するところから.
  /// <summary>
  /// 
  /// </summary>
  /// <see href="https://github.com/JonHanna/corefx/blob/f0d3761d8e875f6fa17e43ab715b746bbcb68716/src/Common/src/System/Collections/Generic/EnumerableHelpers.cs">EnumerableHelpers.ToArray()</see>
  let inline toArray<'T> (source: seq<'T>) =
    match source with
    | :? IListProvider<'T> as provider ->
      provider.ToArray()
    | :? ICollection<'T> as collection ->
      let count = collection.Count
      if count <> 0 then
        let acc = Array.zeroCreate count
        collection.CopyTo(acc, 0)
        acc
      else
        Array.empty<'T>
    | _ ->
      use iter = source.GetEnumerator()
      if iter.MoveNext() then
        let mutable acc = Array.zeroCreate defaultCapacity
        acc[0] <- iter.Current
        let mutable count = 1
        while iter.MoveNext() do
          if count = acc.Length then
            let mutable newLength = count <<< 1
            if uint newLength > uint maxArrayLength then
              newLength <- if maxArrayLength <= count then count + 1 else maxArrayLength
            Array.Resize(&acc, newLength)
          acc[count] <- iter.Current
          count <- count + 1
        Array.Resize(&acc, count)
        acc
      else
        Array.empty<'T>

  /// <summary>
  /// 
  /// </summary>
  let inline toList<'T> (source: seq<'T>) =
    match source with
    | :? IListProvider< ^T> as provider -> provider.ToList()
    | _ -> ResizeArray(source)

  /// <summary>
  /// 
  /// </summary>
  // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/ToCollection.cs#L76
  let inline toDictionaryFromArray (src: 'Src[], [<InlineIfLambda>]selector: 'Src -> 'Key, comparer: IEqualityComparer<'Key>) =
    let acc = Dictionary<'Key, 'Src>(src.Length, comparer)
    for i = 0 to src.Length - 1 do
      acc.Add(selector src[i], src[i])
    acc

  /// <summary>
  /// 
  /// </summary>
  // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/ToCollection.cs#L87
  let inline toDictionaryFromResizeArray (src: ResizeArray<'Src>, [<InlineIfLambda>]selector: 'Src -> 'Key, comparer: IEqualityComparer<'Key>) =
    let acc = Dictionary<'Key, 'Src>(src.Count, comparer)
    for elem in src do
      acc.Add(selector elem, elem)
    acc

  /// <summary>
  /// 
  /// </summary>
  let inline toDictionaryFromList (src: list<'Src>, [<InlineIfLambda>]selector: 'Src -> 'Key, comparer: IEqualityComparer<'Key>) =
    let acc = Dictionary<'Key, 'Src>(src.Length, comparer)
    let rec loop (xs: list<'Src>) =
      match xs with
      | h::tail ->
        acc.Add(selector h, h)
        loop(tail)
      | _ -> ()
    loop src
    acc

  // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/ToCollection.cs#L35
  let inline toDictionary' (src: seq<'Src>, [<InlineIfLambda>]selector: 'Src -> 'Key, comparer: IEqualityComparer<'Key>) =
    match src with
    | :? IReadOnlyCollection<'Src> as collection ->
      if collection.Count = 0
      then
        Dictionary<'Key, 'Src>(comparer)
      else
        match collection with
        | :? list<'Src> as ls -> toDictionaryFromList(ls, selector, comparer)
        | _ ->
          let acc = Dictionary<'Key, 'Src>(collection.Count, comparer)
          for elem in src do
            acc.Add(selector elem, elem)
          acc
    | :? ICollection<'Src> as collection-> 
      if collection.Count = 0
      then
        Dictionary<'Key, 'Src>(comparer)
      else
        match collection with
        | :? array<'Src> as ary -> toDictionaryFromArray(ary, selector, comparer)
        | :? ResizeArray<'Src> as ary -> toDictionaryFromResizeArray(ary, selector, comparer)
        | _ ->
          let acc = Dictionary<'Key, 'Src>(collection.Count, comparer)
          for elem in src do
            acc.Add(selector elem, elem)
          acc
    | _ ->
      let acc = Dictionary<'Key, 'Src>(4, comparer)
      for elem in src do
        acc.Add(selector elem, elem)
      acc

  // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/ToCollection.cs#L32
  let inline toDictionary (src: seq<'Src>, [<InlineIfLambda>]selector: 'Src -> 'Key) =
    toDictionary'(src, selector, null)

  // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/ToCollection.cs#L147
  let inline toDictionary2FromArray (src: array<'Src>, [<InlineIfLambda>]keySelector: 'Src -> 'Key, [<InlineIfLambda>]elementSelector: 'Src -> 'Element, comparer: IEqualityComparer<'Key>) =
    let dict = Dictionary<'Key, 'Element>(src.Length, comparer)
    for i = 0 to src.Length - 1 do
      dict.Add(keySelector src[i], elementSelector src[i])
    dict

  // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/ToCollection.cs#L158
  let inline toDictionary2FromResizeArray (src: ResizeArray<'Src>, [<InlineIfLambda>]keySelector: 'Src -> 'Key, [<InlineIfLambda>]elementSelector: 'Src -> 'Element, comparer: IEqualityComparer<'Key>) =
    let dict = Dictionary<'Key, 'Element>(src.Count, comparer)
    for v in src do
      dict.Add(keySelector v, elementSelector v)
    dict

  // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/ToCollection.cs#L101
  let inline toDictionary2' (src: seq<'Src>, [<InlineIfLambda>]keySelector: 'Src -> 'Key, [<InlineIfLambda>]elementSelector: 'Src -> 'Element, comparer: IEqualityComparer<'Key>) =
    let inline to_dict (capacity: int) =
      let acc = Dictionary<'Key, 'Element>(capacity, comparer)
      for e in src do acc.Add(keySelector e, elementSelector e)
      acc
    
    match src with
    | :? ICollection as collection ->
      if collection.Count = 0
      then Dictionary<'Key, 'Element>(comparer)
      else
        match collection with
        | :? array<'Src> as array -> toDictionary2FromArray(array, keySelector, elementSelector, comparer)
        | :? ResizeArray<'Src> as array -> toDictionary2FromResizeArray(array, keySelector, elementSelector, comparer)
        | _ -> to_dict(collection.Count)
    | _ -> to_dict(4)

  // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/ToCollection.cs#L98
  let inline toDictionary2 (src: seq<'Src>, [<InlineIfLambda>]keySelector: 'Src -> 'Key, [<InlineIfLambda>]elementSelector: 'Src -> 'Element) =
    toDictionary2'(src, keySelector, elementSelector, EqualityComparer<'Key>.Default)
  
  /// <summary>
  /// 
  /// </summary>
  /// <see href="https://github.com/JonHanna/corefx/blob/master/src/Common/src/System/Collections/Generic/EnumerableHelpers.Linq.cs#L22">bool TryGetCount<T>(IEnumerable<T> source, out int count)</see>
  let inline tryGetCount<'T> (source: seq<'T>, count: outref<int>) =
    match source with
    | :? ICollection<'T> as collection ->
      count <- collection.Count
      true
    | :? IListProvider<'T> as provider ->
      count <- provider.GetCount(true)
      0 <= count
    | _ ->
      count <- -1
      false
      
  /// <summary>
  /// 
  /// </summary>
  /// <see href="https://github.com/JonHanna/corefx/blob/master/src/Common/src/System/Collections/Generic/EnumerableHelpers.Linq.cs#L74">void IterativeCopy<T>(IEnumerable<T> source, T[] array, int arrayIndex, int count)</see>
  let inline iterativeCopy<'T> (source: seq<'T>, array: array<'T>, arrayIndex: int, count: int) =
    let mutable index = arrayIndex
    let endIndex = arrayIndex + count
    for v in source do
      array[index] <- v
      index <- index + 1
    assert(arrayIndex = endIndex)
      
  /// <summary>
  /// 
  /// </summary>
  /// <see href="https://github.com/JonHanna/corefx/blob/master/src/Common/src/System/Collections/Generic/EnumerableHelpers.Linq.cs#L49">void Copy<T>(IEnumerable<T> source, T[] array, int arrayIndex, int count)</see>
  let inline copy<'T> (source: seq<'T>, array: 'T[], arrayIndex: int, count: int) =
    match source with
    | :? ICollection<'T> as collection -> collection.CopyTo(array, arrayIndex)
    | _ -> iterativeCopy(source, array, arrayIndex, count)

  // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/Count.cs#L11
  let inline count<'T> (src: seq<'T>) =
    match src with
    | :? IReadOnlyCollection<'T> as collection -> collection.Count
    | :? ICollection<'T> as collection -> collection.Count
    | :? IListProvider<'T> as prov -> prov.GetCount(false)
    | :? ICollection as collection -> collection.Count
    | _ ->
      let mutable count = 0
      use e = src.GetEnumerator()
      while e.MoveNext() do
        count <- Checked.(+) count 1
      count

  // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/Count.cs#L48
  let inline count'<'T> ([<InlineIfLambda>]predicate: 'T -> bool) (src: seq<'T>) =
    let mutable count = 0
    for v in src do
      if predicate v then count <- Checked.(+) count 1
    count

  // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/Contains.cs#L14
  let inline contains'<'T> (value: 'T, comparer: IEqualityComparer<'T>) (src: seq<'T>) =
    match src with
    | :? list<'T> as xs ->
      let rec loop (xs: list<'T>) =
        match xs with
        | head::tail ->
          if comparer.Equals(head, value)
          then true
          else loop(tail)
        | _ -> false
      loop xs
    | _ -> 
      let enumerator = src.GetEnumerator()
      let mutable isbreak = false
      let mutable contains = false
      while not isbreak && enumerator.MoveNext() do
        if comparer.Equals(enumerator.Current, value)
        then 
          contains <- true
          isbreak <- true
      contains

  // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/Contains.cs#L10
  let inline contains<'T> (value: 'T) (src: seq<'T>) =
    match src with
    | :? ICollection<'T> as collection -> collection.Contains(value)
    | _ -> src |> contains' (value, EqualityComparer<'T>.Default)

  // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/Count.cs#L128
  let inline longCount<'T> (src: seq<'T>) =
    let mutable count = 0L
    use e = src.GetEnumerator()
    while e.MoveNext() do
      count <- Checked.(+) count 1L
    count

  // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/Count.cs#L150
  let inline longCount'<'T> ([<InlineIfLambda>]predicate: 'T -> bool) (src: seq<'T>) =
    let mutable count = 0L
    for v in src do
      if predicate v then count <- Checked.(+) count 1L
    count

  // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/Count.cs#L95
  let inline tryGetNonEnumeratedCount<'T> (src: seq<'T>) : (bool * int) =
    match src with
    | :? IReadOnlyCollection<'T> as collection -> (true, collection.Count)
    | :? ICollection<'T> as collection -> (true, collection.Count)
    | :? IListProvider<'T> as prov ->
      let c = prov.GetCount(false)
      (0 <= c, c)
    | :? ICollection as collection -> (true, collection.Count)
    | _ -> (false, 0)

  // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/ToCollection.cs#L186
  let inline hashseToArray<'T> (set: HashSet<'T>) =
    let result = Array.zeroCreate<'T> set.Count
    set.CopyTo result
    result

  // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/ToCollection.cs#L193
  let inline hashseToList<'T> (set: HashSet<'T>) =
    let result = ResizeArray<'T> set.Count
    for item in set do
      result.Add item
    result

  // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/First.cs#L52
  let inline tryGetFirst<'T> (src: seq<'T>) =
    match src with
    | :? IPartition<'T> as partition -> partition.TryGetFirst()
    | :? list<'T> as list -> match list with h::tail -> (h, true) | _ -> (defaultof<'T>, false)
    | :? IList<'T> as list -> if 0 < list.Count then (list[0], true) else (defaultof<'T>, false)
    | :? IReadOnlyList<'T> as list -> if 0 < list.Count then (list[0], true) else (defaultof<'T>, false)
    | _ ->
      use e = src.GetEnumerator()
      if e.MoveNext() then (e.Current, true) else (defaultof<'T>, false)

  // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/First.cs#L52
  let inline tryGetFirst'<'T> (src: seq<'T>, [<InlineIfLambda>]predicate: 'T -> bool) =
    use e = src.GetEnumerator()
    let rec loop() =
      if e.MoveNext()
      then
        let element = e.Current
        if predicate e.Current then (element, true) else loop()
      else (defaultof<'T>, false)
    loop()

  // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/Last.cs#L52
  let inline tryGetLast<'T> (src: seq<'T>) =
    match src with
    | :? IPartition<'T> as partition -> partition.TryGetLast()
    | :? IList<'T> as xs -> if 0 < xs.Count then (xs[xs.Count - 1], true) else (defaultof<'T>, false)
    | :? IReadOnlyList<'T> as xs -> if 0 < xs.Count then (xs[xs.Count - 1], true) else (defaultof<'T>, false)
    | _ ->
      use e = src.GetEnumerator()
      if e.MoveNext() 
      then 
        let rec loop (v) =
          if e.MoveNext() 
          then loop(e.Current)
          else v
        let last = loop (e.Current)
        (last, true)
      else (defaultof<'T>, false)

  // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/Last.cs#L96
  let inline tryGetLast'<'T> (src: seq<'T>, [<InlineIfLambda>]predicate: 'T -> bool) =
    match src with
    | :? IList<'T> as xs -> 
      if 0 < xs.Count 
      then 
        let rec loop i =
          if i < 0
          then (defaultof<'T>, false)
          else if predicate xs[i] then (xs[i], true) else loop (i - 1)
        loop (xs.Count)
      else 
        (defaultof<'T>, false)
    | :? IReadOnlyList<'T> as xs -> 
      if 0 < xs.Count 
      then 
        let rec loop i =
          if i < 0
          then (defaultof<'T>, false)
          else if predicate xs[i] then (xs[i], true) else loop (i - 1)
        loop (xs.Count)
      else 
        (defaultof<'T>, false)
    | _ ->
      use e = src.GetEnumerator()
      if e.MoveNext() 
      then 
        let rec loop (v, found) =
          if e.MoveNext() 
          then 
            if predicate e.Current
            then loop(e.Current, true)
            else loop(v, found)
          else (v, found)
        if predicate e.Current then loop (e.Current, true) else loop (e.Current, false)
      else (defaultof<'T>, false)

  let inline tryGetSingle<'T> (src: seq<'T>) =
    match src with
    | :? IReadOnlyList<'T> as xs ->
      match xs.Count with
      | 0 -> (false, defaultof<'T>)
      | 1 -> (true, xs[0])
      | _ -> raise (invalidArg "src" "more than one element")
    | :? IList<'T> as xs ->
      match xs.Count with
      | 0 -> (false, defaultof<'T>)
      | 1 -> (true, xs[0])
      | _ -> raise (invalidArg "src" "more than one element")
    | _ -> 
      use e = src.GetEnumerator()
      if e.MoveNext() |> not
      then (false, defaultof<'T>)
      else
        let result = e.Current
        if e.MoveNext() |> not
        then (true, result)
        else raise (invalidArg "src" "more than one element")

  let inline tryGetSingle'<'T> (src: seq<'T>, [<InlineIfLambda>]predicate: 'T -> bool) =
    use e = src.GetEnumerator()
    let rec loop () =
      if e.MoveNext() 
      then
        let result = e.Current
        if predicate result
        then
          while e.MoveNext() do
            if predicate e.Current then raise (invalidArg "src" "more than one element")
          (true, result)
        else
          loop()
      else
        (false, defaultof<'T>)
    loop()

type Buffer<'T> = { items: 'T[]; count: int }
module public Buffer =
  let bind (items: seq<'T>) =
    match items with
    | :? IListProvider<'T> as xs -> 
      let array = xs.ToArray()
      { items = array; count = array.Length }
    | _ ->
      let array = Enumerable.toArray items
      { items = array; count = array.Length }
