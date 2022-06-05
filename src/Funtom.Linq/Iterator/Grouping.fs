namespace rec Funtom.Linq.Iterator

open System.Collections
open System.Collections.Generic
open Funtom.Linq
open Funtom.Linq.Interfaces

// https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/Grouping.cs#L50
type Grouping<'Key, 'Element> (key: 'Key, hashCode: int) =
  let mutable elements = Array.zeroCreate<'Element>(1)
  let mutable hashNext = defaultof<IGrouping<'Key, 'Element>>
  let mutable next = defaultof<IGrouping<'Key, 'Element>>
  let mutable count = 0

  member internal __.HashNext with get () = hashNext and set v = hashNext <- v
  member internal __.HashCode with get () = hashCode
  member internal __.Next with get () = next and set v = next <- v

  member __.Add (element: 'Element) =
    if elements.Length = count then
      System.Array.Resize(&elements, check (*) count 2)
    elements[count] <- element
    count <- count + 1

  member __.Trim () =
    if elements.Length <> count then
      System.Array.Resize(&elements, count)

  member __.GetEnumerator() : IEnumerator<'Element> =
    seq { for i in 0..(count - 1) -> elements[i] }
    |> (fun xs -> xs.GetEnumerator())
    
  abstract member Key : 'Key with get
  default __.Key with get() = key
  member __.Count with get() = count
  member __.IsReadOnly with get() = true

  interface IEnumerable with member __.GetEnumerator () = __.GetEnumerator ()
  interface IEnumerable<'Element> with member __.GetEnumerator () = __.GetEnumerator ()
  interface IGrouping<'Key, 'Element> with 
    member __.Key with get() = __.Key
    member __.HashNext with get () = hashNext and set v = hashNext <- v
    member __.HashCode with get () = hashCode
    member __.Next with get () = next and set v = next <- v
    member __.Elements with get () = elements
    member __.Add (e) = __.Add (e)
    member __.Trim () = __.Trim()
  interface ICollection<'Element> with 
    member __.Count with get() = __.Count
    member __.IsReadOnly with get() = __.IsReadOnly
    member __.Add(item) = raise (System.NotSupportedException "")
    member __.Clear() = raise (System.NotSupportedException "")
    member __.Contains(item) = 0 <= System.Array.IndexOf(elements, item, 0, count)
    member __.CopyTo(array, index) = System.Array.Copy(elements, 0, array, index, count)
    member __.Remove(item) = raise (System.NotSupportedException "")
  interface IList<'Element> with
    member __.IndexOf(item) = System.Array.IndexOf(elements, item, 0, count)
    member __.Insert(index, item) = raise (System.NotSupportedException "")
    member __.RemoveAt(index) = raise (System.NotSupportedException "")
    member __.Item 
      with get index = if index < 0 || count <= index then raise (System.ArgumentOutOfRangeException "") else elements[index]
      and set index value = raise (System.NotSupportedException "")
    
// https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/Lookup.cs#L65
type Lookup<'Key, 'Element> private (comparer: IEqualityComparer<'Key>) =
  let mutable groupings = Array.zeroCreate<IGrouping<'Key, 'Element>>(7)
  let mutable lastGrouping = defaultof<IGrouping<'Key, 'Element>>
  let mutable count = 0

  // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/Lookup.cs#L196
  member __.GetGrouping (key: 'Key, create: bool) =
    let inline getHashCode key = comparer.GetHashCode(key) &&& 0x7FFFFFFF;
    let hashcode = getHashCode key
    let inline resize () =
      let newsize = Checked.(+) (count * 2) 1
      let newgroupings = Array.zeroCreate<IGrouping<'Key, 'Element>>(newsize)
      let mutable g = lastGrouping :> IGrouping<'Key, 'Element>
      let rec loop () =
        g <- g.Next
        let index = hashcode % groupings.Length
        g.HashNext <- newgroupings[index]
        newgroupings[index] <- g
        if g <> lastGrouping
        then loop()
        else ()
      loop()
      groupings <- newgroupings

    let def = defaultof<IGrouping<'Key, 'Element>>
    let rec loop (g: IGrouping<'Key, 'Element>) =
      if g <> def
      then
        if g.HashCode = hashcode && comparer.Equals(g.Key, key)
        then g
        else loop(g.HashNext)
      else def
    let g = loop (groupings[hashcode % groupings.Length])

    if g <> def 
    then g
    else 
      if create
      then
        if count = groupings.Length then resize()
        let index = hashcode % groupings.Length
        g.HashNext <- groupings[index]
        groupings[index] <- g
        if lastGrouping = def
        then g.Next <- g
        else g.Next <- lastGrouping.Next; lastGrouping.Next <- g
        lastGrouping <- g
        count <- count + 1
        g
      else def

  // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/Lookup.cs#L124
  member __.Item with get (key: 'Key) = 
    let grouping = __.GetGrouping(key, create = false)
    if grouping = defaultof<IGrouping<'Key, 'Element>>
    then Array.empty<'Element> :> IEnumerable<'Element> 
    else grouping

  // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/Lookup.cs#L133
  member __.Contains(key: 'Key) = 
    __.GetGrouping(key, create =  false) <> defaultof<IGrouping<'Key, 'Element>>

  // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/Lookup.cs#L135
  member __.GetEnumerator() =
    let mutable g = lastGrouping :> IGrouping<'Key, 'Element>
    let x = 
      if g <> defaultof<_> then
        seq {
          g <- g.Next
          yield g
          while g <> defaultof<_> do
            g <- g.Next
            yield g
        }
      else
        Array.empty<_>
    x.GetEnumerator()

  // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/Lookup.cs#L151
  member internal __.ToList<'Result>(selector: 'Key -> seq<'Element> -> 'Result) =
    let acc = ResizeArray<'Result>(count)
    let mutable g = lastGrouping
    if g <> defaultof<_> then
      g <- g.Next
      g.Trim()
      acc.Add(selector g.Key g.Elements)
      while g <> defaultof<_> do
        g <- g.Next
        g.Trim()
        acc.Add(selector g.Key g.Elements)
    acc

  // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/Lookup.cs#L171
  member __.ApplyResultSelector<'Result>(selector: 'Key -> seq<'Element> -> 'Result) = 
    let mutable g = lastGrouping
    if g <> defaultof<_> then
      seq {
        g <- g.Next
        g.Trim()
        yield (selector g.Key g.Elements)

        while g <> defaultof<_> do
          g <- g.Next
          g.Trim()
          yield (selector g.Key g.Elements)
      }
    else
      Array.empty<_>

  member __.Count with get() = count

  // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/Lookup.SpeedOpt.cs#L11
  member __.ToArray() =
    let acc = Array.zeroCreate<IGrouping<'Key, 'Element>>(count)
    let mutable i = 0
    let mutable g = lastGrouping
    if g <> defaultof<_> then
      g <- g.Next
      acc[i] <- g
      i <- i + 1
      while g <> defaultof<_> do
        g <- g.Next
        acc[i] <- g
        i <- i + 1
    acc

  // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/Lookup.SpeedOpt.cs#L32
  member __.ToArray(selector) =
    let acc = Array.zeroCreate<IGrouping<'Key, 'Element>>(count)
    let mutable i = 0
    let mutable g = lastGrouping
    if g <> defaultof<_> then
      g <- g.Next
      acc[i] <- selector g.Key g.Elements
      i <- i + 1
      while g <> defaultof<_> do
        g <- g.Next
        acc[i] <- g
        i <- i + 1
    acc
    
  // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/Lookup.SpeedOpt.cs#L54
  member __.ToList() =
    let acc = ResizeArray<'Result>(count)
    let mutable g = lastGrouping
    if g <> defaultof<_> then
      g <- g.Next
      acc.Add(g)
      while g <> defaultof<_> do
        g <- g.Next
        acc.Add(g)
    acc

  interface IEnumerable with
    member __.GetEnumerator () = __.GetEnumerator ()
  interface IEnumerable<IGrouping<'Key, 'Element>> with
    member __.GetEnumerator () = __.GetEnumerator ()
  interface ILookup<'Key, 'Element> with
    member __.Count with get () = __.Count
    member __.Item with get (key) = __.Item(key)
    member __.Contains (key) = __.Contains(key)
  interface IListProvider<IGrouping<'Key, 'Element>> with
    member __.ToArray() = __.ToArray()
    member __.ToList() = __.ToList()
    member __.GetCount (_: bool) = count

  // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/Lookup.cs#L72
  static member Create<'Source>(src: seq<'Source>, keyselector: 'Source -> 'Key, elemselector: 'Source -> 'Element, comparer: IEqualityComparer<'Key>) =
    let lookup = Lookup<'Key, 'Element>(comparer)
    for item in src do
      lookup.GetGrouping(keyselector item, create= true).Add(elemselector item)
    lookup

  // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/Lookup.cs#L87
  static member Create<'Source>(src: seq<'Source>, keyselector: 'Source -> 'Key, comparer: IEqualityComparer<'Key>) =
    let lookup = Lookup<'Key, 'Source>(comparer)
    for item in src do
      lookup.GetGrouping(keyselector item, create= true).Add(item)
    lookup


// https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/Grouping.cs#L223
// https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/Grouping.SpeedOpt.cs#L32
type GroupedEnumerable<'Source, 'Key, 'Element> (source: seq<'Source>, keyselector: 'Source -> 'Key, elemselector: 'Source -> 'Element, comaprer: IEqualityComparer<'Key>) =
  member __.GetEnumerator() =
    Lookup<'Key, 'Element>.Create(source, keyselector, elemselector, comaprer).GetEnumerator()
  
  member __.ToArray() =
    let lookup : IListProvider<_> = Lookup<'Key, 'Element>.Create(source, keyselector, elemselector, comaprer)
    lookup.ToArray()
  member __.ToList() =
    let lookup : IListProvider<_> = Lookup<'Key, 'Element>.Create(source, keyselector, elemselector, comaprer)
    lookup.ToList()
  member __.GetCount(onlyIfCheap: bool) =
    if onlyIfCheap then -1 else Lookup<'Key, 'Element>.Create(source, keyselector, elemselector, comaprer).Count

  interface IEnumerable with
    member __.GetEnumerator () = __.GetEnumerator ()
  interface IEnumerable<IGrouping<'Key, 'Element>> with
    member __.GetEnumerator () = __.GetEnumerator ()


// https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/Grouping.cs#L257
// https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/Grouping.SpeedOpt.cs#L50
type GroupedEnumerable<'Source, 'Key> (source: seq<'Source>, selector: 'Source -> 'Key, comaprer: IEqualityComparer<'Key>) =
  member __.GetEnumerator() =
    Lookup<'Key, 'Source>.Create(source, selector, comaprer).GetEnumerator()
  
  member __.ToArray() =
    let lookup : IListProvider<_> = Lookup<'Key, 'Source>.Create(source, selector, comaprer)
    lookup.ToArray()
  member __.ToList() =
    let lookup : IListProvider<_> = Lookup<'Key, 'Source>.Create(source, selector, comaprer)
    lookup.ToList()
  member __.GetCount(onlyIfCheap: bool) =
    if onlyIfCheap then -1 else Lookup<'Key, 'Source>.Create(source, selector, comaprer).Count

  interface IEnumerable with
    member __.GetEnumerator () = __.GetEnumerator ()
  interface IEnumerable<IGrouping<'Key, 'Source>> with
    member __.GetEnumerator () = __.GetEnumerator ()