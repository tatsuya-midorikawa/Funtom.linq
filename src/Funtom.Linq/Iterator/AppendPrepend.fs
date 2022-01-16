namespace Funtom.Linq.Iterator

open System
open System.Collections
open System.Collections.Generic
open Basis

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
      let getSourceEnumerator () =
        if __.node <> Unchecked.defaultof<SingleLinkedNode<'T>> then
          __.current <- __.node.Item
          __.node <- __.node.Linked
          true
        else
          __.GetSourceEnumerator()
          __.state <- 3
          false
      
      let loadFromEnumerator () =
        if __.LoadFromEnumerator() then
          true
        else
          if appended = Unchecked.defaultof<SingleLinkedNode<'T>> then
            false
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
      | 3 ->
        loadFromEnumerator()
      | 4 ->
        __.LoadFromEnumerator()
      | _ ->
        __.Dispose()
        false

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
      ()

  [<Sealed>]
  type AppendPrepend1Iterator<'T> (source: seq<'T>, item: 'T, appending: bool) =
    inherit AppendPrependIterator<'T>(source)
    override __.Clone() = new AppendPrepend1Iterator<'T>(source, item, appending)
    override __.MoveNext() =
      let getSouceEnumerator() =
        __.GetSourceEnumerator()
        __.state <- 3
      let loadFromEnumerator() =
        if __.LoadFromEnumerator() then
          true
        else
          if appending then
            __.current <- item
            true
          else
            __.Dispose()
            false
            
      match __.state with
      | 1 ->
        __.state <- 2
        if appending then
          __.current <- item
          true
        else
          getSouceEnumerator()
          loadFromEnumerator()
      | 2 ->
        getSouceEnumerator()
        loadFromEnumerator()
      | 3 ->
        loadFromEnumerator()
      | _ ->
        __.Dispose()
        false

