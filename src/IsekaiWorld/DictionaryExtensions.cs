using System;
using System.Collections.Generic;

namespace IsekaiWorld;

/// <summary>
/// Extensions for dealing with <see cref="Dictionary{TKey,TValue}"/>
/// </summary>
public static class DictionaryExtensions
{
    public static TValue GetOrAddNew<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue defaultValue) 
        where TValue : new() 
        => dict.GetOrAdd(key, (_, _) => EqualityComparer<TValue>.Default.Equals(default, defaultValue) ? new TValue() : defaultValue);

    public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue defaultValue)
        => dict.GetOrAdd(key, (_, _) => defaultValue);

    public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, Func<TValue> valueProvider)
        => dict.GetOrAdd(key, (_, _) => valueProvider());

    public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, Func<TKey, TValue> valueProvider)
        => dict.GetOrAdd(key, (_, _) => valueProvider(key));

    public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, Func<IDictionary<TKey, TValue>, TKey, TValue> valueProvider)
    {
        if (dict == null) throw new ArgumentNullException(nameof(dict));
        if (key == null) throw new ArgumentNullException(nameof(key));
        if (valueProvider == null) throw new ArgumentNullException(nameof(valueProvider));

        if (dict.TryGetValue(key, out var foundValue))
            return foundValue;

        dict[key] = valueProvider(dict, key);
        return dict[key];
    }
}