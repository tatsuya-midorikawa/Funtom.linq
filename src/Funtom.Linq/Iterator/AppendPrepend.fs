namespace Funtom.Linq.Iterator

open System
open System.Collections
open System.Collections.Generic
open Funtom.Linq.Common
open Basis
open Interfaces
open System.Runtime.CompilerServices

module AppendPrepend =
  // src: https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/AppendPrepend.cs#L39
  // src: https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/AppendPrepend.SpeedOpt.cs#L11
  [<AbstractClass>]
  type AppendPrependIterator<'T> (source: seq<'T>) =
    inherit Iterator<'T> ()
    member val internal source = source
    member val internal enumerator = Unchecked.defaultof<IEnumerator<'T>> with get, set

    abstract member Append : 'T -> AppendPrependIterator<'T>
    abstract member Prepend : 'T -> AppendPrependIterator<'T>
    abstract member ToArray : unit -> array<'T>
    abstract member ToList : unit -> ResizeArray<'T>
    abstract member GetCount : bool -> int

    member internal __.GetSourceEnumerator() = __.enumerator <- __.source.GetEnumerator()
    member internal __.LoadFromEnumerator() =
      if __.enumerator.MoveNext() then
        __.current <- __.enumerator.Current
        true
      else
        __.Dispose()
        false
    override __.Dispose() = 
      if __.enumerator <> Unchecked.defaultof<IEnumerator<'T>> then
        __.enumerator.Dispose()
        __.enumerator <- Unchecked.defaultof<IEnumerator<'T>>
      base.Dispose()

  // src: https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/AppendPrepend.cs#L168
  // src: https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/AppendPrepend.SpeedOpt.cs#L103
  [<Sealed>]
  type AppendPrependN<'T> (source: seq<'T>, prepended: SingleLinkedNode<'T>, appended: SingleLinkedNode<'T>, prependCount: int, appendCount: int) =
    inherit AppendPrependIterator<'T>(source)
    member val private node = Unchecked.defaultof<SingleLinkedNode<'T>> with get, set

    override __.Clone () =
      new AppendPrependN<'T> (source, prepended, appended, prependCount, appendCount)

    override __.MoveNext () =
      let inline getSourceEnumerator () =
        if __.node <> Unchecked.defaultof<SingleLinkedNode<'T>> then
          __.current <- __.node.Item
          __.node <- __.node.Linked
          true
        else
          __.GetSourceEnumerator()
          __.state <- 3
          false
      
      let inline loadFromEnumerator () =
        if __.LoadFromEnumerator() then true
        else
          if appended = Unchecked.defaultof<SingleLinkedNode<'T>> then false
          else
            __.enumerator <- (appended.ToArray(appendCount) :> IEnumerable<'T>).GetEnumerator()
            __.state <- 4
            __.LoadFromEnumerator()

      match __.state with
      | 1 ->
        __.node <- prepended
        __.state <- 2
        if getSourceEnumerator() then true
        else loadFromEnumerator()
      | 2 ->
        if getSourceEnumerator() then true
        else loadFromEnumerator()
      | 3 -> loadFromEnumerator()
      | 4 -> __.LoadFromEnumerator()
      | _ -> __.Dispose(); false

    override __.Append (item: 'T) =
      let appended =
        if appended <> Unchecked.defaultof<SingleLinkedNode<'T>> then appended.Add(item)
        else new SingleLinkedNode<'T>(item)
      new AppendPrependN<'T>(source, prepended, appended, prependCount, appendCount + 1)

    override __.Prepend (item: 'T) =
      let prepended =
        if prepended <> Unchecked.defaultof<SingleLinkedNode<'T>> then prepended.Add(item)
        else new SingleLinkedNode<'T>(item)
      new AppendPrependN<'T>(source, prepended, appended, prependCount + 1, appendCount)

    member private __.LazyToArray() =
      let mutable builder = SparseArrayBuilder<'T>.Create()
      if prepended <> Unchecked.defaultof<SingleLinkedNode<'T>> then
        builder.Reserve(prependCount)

      builder.AddRange(source)

      if appended <> Unchecked.defaultof<SingleLinkedNode<'T>> then
        builder.Reserve(appendCount)

      let mutable array = builder.ToArray()
      let rec fx (node: SingleLinkedNode<'T>, index: int) =
        if node <> Unchecked.defaultof<SingleLinkedNode<'T>> then
          array[index] <- node.Item
          fx(node.Linked, index + 1)
      fx(__.node, 0)
      array

    override __.GetCount (onlyIfCheap: bool) =
      match source with
      | :? IListProvider<'T> as provider ->
        let count = provider.GetCount(onlyIfCheap)
        if count = -1 then -1 else count + appendCount + prependCount
      | _ -> 
        if not onlyIfCheap || source.GetType() = typeof<ICollection<'T>> then
          let length =
            match source with
            | :? ICollection<'T> as collection -> collection.Count
            | :? IReadOnlyCollection<'T> as collection -> collection.Count
            | _ -> source |> Seq.length // TODO: Seq.length 辞めたい
          length + appendCount + prependCount
        else -1

    override __.ToArray() =
      let count = __.GetCount(true)
      if count = -1 then
        __.LazyToArray()
      else
        let mutable array = Array.zeroCreate<'T>(count)
        let rec prepend(node: SingleLinkedNode<'T>, index: int) =
          if node <> Unchecked.defaultof<SingleLinkedNode<'T>> then
            array[index] <- node.Item
            prepend(node.Linked, index + 1)
          else
            index
        let mutable length = prepend(prepended, 0)

        match __.source with
        | :? ICollection<'T> as collection -> collection.CopyTo(array, length)
        | _ ->
          for item in __.source do
            array[length] <- item
            length <- length + 1

        length <- array.Length - 1
        let rec append (node: SingleLinkedNode<'T>, index: int) =
          if node <> Unchecked.defaultof<SingleLinkedNode<'T>> then
            array[index] <- node.Item
            append(node.Linked, index - 1)
        append(appended, 0)
        array

    override __.ToList() =
      let count = __.GetCount(true)
      let list = ResizeArray<'T>(max count 4)
      let rec prepend(node: SingleLinkedNode<'T>) =
        if node <> Unchecked.defaultof<SingleLinkedNode<'T>> then
          list.Add(node.Item)
          prepend(node.Linked)        

      prepend(prepended)
      list.AddRange(__.source)
      // TODO: なんで appended だけ、ToArray(int) で AddRange してるの？
      if appended <> Unchecked.defaultof<SingleLinkedNode<'T>> then
        list.AddRange(appended.ToArray(appendCount))

      list

  // src: https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/AppendPrepend.cs#L89
  // src: https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/AppendPrepend.SpeedOpt.cs#L20
  [<Sealed>]
  type AppendPrepend1Iterator<'T> (source: seq<'T>, item: 'T, appending: bool) =
    inherit AppendPrependIterator<'T>(source)

    let lazyToArray() =
      let mutable builder = new LargeArrayBuilder<'T>(Int32.MaxValue)
      if not appending then builder.SlowAdd(item)
      builder.AddRange(source)
      if appending then builder.SlowAdd(item)
      builder.ToArray()

    override __.Clone() = new AppendPrepend1Iterator<'T>(source, item, appending)

    // TODO: 実装再確認
    // src: https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/AppendPrepend.cs#L103
    override __.MoveNext() =
      let inline getSouceEnumerator() = __.GetSourceEnumerator(); __.state <- 3
      let inline loadFromEnumerator() =
        if __.LoadFromEnumerator() then true
        else
          if appending then __.current <- item; true
          else __.Dispose(); false
      let exec = (getSouceEnumerator >> loadFromEnumerator)

      match __.state with
      | 1 ->
        __.state <- 2
        if appending then __.current <- item; true
        else exec()
      | 2 -> exec()
      | 3 -> loadFromEnumerator()
      | _ -> __.Dispose(); false

    override __.GetCount (onlyIfCheap: bool) =
      match source with
      | :? IListProvider<'T> as provider ->
        let count = provider.GetCount(onlyIfCheap)
        if count = -1 then -1 else count + 1
      | _ -> 
        if not onlyIfCheap || source.GetType() = typeof<ICollection<'T>> then
          (Seq.length __.source) + 1  // TODO: Seq.length 辞めたい
        else
          -1
    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    override __.Append (item': 'T) =
      if appending then new AppendPrependN<'T>(source, Unchecked.defaultof<SingleLinkedNode<'T>>, SingleLinkedNode<'T>(item).Add(item'), 0, 2)
      else new AppendPrependN<'T>(source, SingleLinkedNode<'T>(item), SingleLinkedNode<'T>(item'), 1, 1)
      
    override __.Prepend (item': 'T) =
      if appending then new AppendPrependN<'T>(source, SingleLinkedNode<'T>(item'), SingleLinkedNode<'T>(item), 1, 1)
      else new AppendPrependN<'T>(source, SingleLinkedNode<'T>(item).Add(item'), Unchecked.defaultof<SingleLinkedNode<'T>>, 2, 0)
      
    override __.ToArray() =
      let count = __.GetCount(true)
      if count = -1 then
        lazyToArray()
      else
        let array = Array.zeroCreate<'T>(count)
        let mutable index = 
          if appending then 0
          else array[0] <- item; 1
        Enumerable.copy (source, array, index, count - 1)
        if appending then array[array.Length - 1] <- item
        array

    override __.ToList() =
      let count = __.GetCount(true)
      let list =
        if count = -1 then ResizeArray()
        else ResizeArray(count)
      if not appending then list.Add(item)
      list.AddRange(source)
      if appending then list.Add(item)
      list