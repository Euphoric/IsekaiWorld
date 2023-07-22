using System;

namespace IsekaiWorld.Utils;

public static class UtilExtensions
{
    public static TResult Let<TSource, TResult>(this TSource source, Func<TSource, TResult> func)
    {
        return func(source);
    }
}