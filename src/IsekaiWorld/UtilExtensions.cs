using System;

namespace IsekaiWorld;

public static class UtilExtensions
{
    public static TResult Let<TSource, TResult>(this TSource source, Func<TSource, TResult> func)
    {
        return func(source);
    }
}