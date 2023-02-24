namespace rec Funtom.linq.iterator

open System.Collections.Generic
open Funtom.linq

[<AutoOpen>]
module JoinIterator =
  // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Linq/src/System/Linq/Join.cs#L70
  let inline joinIterator<'outer, 'inner, 'key, 'result> (
    outer: seq<'outer>, inner: seq<'inner>,
    outerkeyselector: 'outer -> 'key, innerkeyselector: 'inner -> 'key, resultselector: 'outer -> 'inner -> 'result,
    comparer: IEqualityComparer<'key>) =

    use e = outer.GetEnumerator()
    seq {
      if e.MoveNext() then
        let lookup = Lookup<'key, 'inner>.CreateForJoin (inner, innerkeyselector, comparer)
        if lookup.Count <> 0 then

          let item = e.Current
          let g = lookup.GetGrouping((outerkeyselector item), false)
          if g <> defaultof<_> then
            let count = g.Count
            let elems = g.Elements
            for i = 0 to count do
              yield (resultselector item elems[i])

            while e.MoveNext() do
              let item = e.Current
              let g = lookup.GetGrouping((outerkeyselector item), false)
              if g <> defaultof<_> then
                let count = g.Count
                let elems = g.Elements
                for i = 0 to count do
                  yield (resultselector item elems[i])   
      }
