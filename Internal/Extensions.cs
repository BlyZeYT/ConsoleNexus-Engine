﻿namespace ConsoleNexusEngine.Internal;

using ConsoleNexusEngine.Common;
using System;
using System.Collections;
using System.Collections.Generic;

internal static class Extensions
{
    public static int GetKey(this IReadOnlyDictionary<ConsoleColor, NexusColor> dictionary, NexusColor color)
    {
        foreach (var pair in dictionary)
        {
            if (pair.Value == color) return (int)pair.Key;
        }

        return -1;
    }

    public static bool IsInRange<T>(this T[,] array, Coord coord)
        => coord.X >= 0 && coord.X < array.GetLength(0) && coord.Y >= 0 && coord.Y < array.GetLength(1);

    public static IReadOnlyCollection<T> AsReadOnly<T>(this ICollection<T> source)
        => source as IReadOnlyCollection<T> ?? new ReadOnlyCollectionAdapter<T>(source);

    private sealed class ReadOnlyCollectionAdapter<T> : IReadOnlyCollection<T>
    {
        private readonly ICollection<T> _source;

        public ReadOnlyCollectionAdapter(ICollection<T> source) => _source = source;

        public int Count => _source.Count;

        public IEnumerator<T> GetEnumerator() => _source.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}