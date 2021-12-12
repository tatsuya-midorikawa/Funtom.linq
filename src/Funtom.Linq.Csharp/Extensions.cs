using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;

namespace Funtom.Linq.Csharp
{
  internal static class Extensions
  {

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<T> WhereA<T>(this IEnumerable<T> src, Func<T, bool> predicate)
    {
      foreach (var x in src)
      {
        if (predicate(x))
          yield return x;
      }
    }
    public static IEnumerable<T> WhereB<T>(this IEnumerable<T> src, Func<T, bool> predicate)
    {
      foreach (var x in src)
      {
        if (predicate(x))
          yield return x;
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<T> FsWhereA<T>(this IEnumerable<T> src, [InlineIfLambda]FSharpFunc<T, bool> predicate)
    {
      foreach (var x in src)
      {
        if (predicate.Invoke(x))
          yield return x;
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<T> FsWhereB<T>(this IEnumerable<T> src, FSharpFunc<T, bool> predicate)
    {
      foreach (var x in src)
      {
        if (predicate.Invoke(x))
          yield return x;
      }
    }
  }
}