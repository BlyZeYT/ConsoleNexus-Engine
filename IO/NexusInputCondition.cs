﻿namespace ConsoleNexusEngine.IO;

using System.Linq;

/// <summary>
/// Represents an input condition
/// </summary>
public sealed class NexusInputCondition
{
    private readonly Predicate<NexusKey> _keyPressCondition;
    private readonly Predicate<NexusCoord> _mousePosCondition;

    internal readonly bool _isMousePosCondition;

    private NexusInputCondition(Predicate<NexusKey> keyPressCondition, Predicate<NexusCoord> mousePosCondition, in bool isMousePosCondition)
    {
        _keyPressCondition = keyPressCondition;
        _mousePosCondition = mousePosCondition;

        _isMousePosCondition = isMousePosCondition;
    }

    private NexusInputCondition(Predicate<NexusKey> keyPressCondition)
        : this(keyPressCondition, (coord) => false, false) { }

    private NexusInputCondition(Predicate<NexusCoord> mousePosCondition)
        : this((key) => false, mousePosCondition, true) { }

    /// <summary>
    /// Checks for a key press
    /// </summary>
    /// <param name="key">The key to check for</param>
    public NexusInputCondition(NexusKey key) : this((toCheck) => toCheck == key) { }

    /// <summary>
    /// Checks for a mouse position
    /// </summary>
    /// <param name="mousePosition">The mouse position to check for</param>
    public NexusInputCondition(NexusCoord mousePosition) : this((toCheck) => toCheck == mousePosition) { }

    /// <summary>
    /// Checks if any key is pressed
    /// </summary>
    /// <param name="keys">The keys to check for</param>
    /// <returns><see cref="NexusInputCondition"/></returns>
    public static NexusInputCondition Any(params NexusKey[] keys) => new((toCheck) => keys.Contains(toCheck));

    /// <summary>
    /// Checks if all keys are pressed
    /// </summary>
    /// <param name="keys">The keys to check for</param>
    /// <returns><see cref="NexusInputCondition"/></returns>
    public static NexusInputCondition All(params NexusKey[] keys) => new((toCheck) => keys.All(x => x == toCheck));

    /// <summary>
    /// Checks if the mouse position is in range of <paramref name="start"/> and <paramref name="end"/>
    /// </summary>
    /// <param name="start">The start coordinate</param>
    /// <param name="end">The end coordinate</param>
    /// <returns><see cref="NexusInputCondition"/></returns>
    public static NexusInputCondition IsInRange(NexusCoord start, NexusCoord end) => new((toCheck) => toCheck.IsInRange(start, end));

    /// <summary>
    /// Checks if the mouse position is in range of <paramref name="start"/> and <paramref name="range"/>
    /// </summary>
    /// <param name="start">The start coordinate</param>
    /// <param name="range">The range size</param>
    /// <returns><see cref="NexusInputCondition"/></returns>
    public static NexusInputCondition IsInRange(NexusCoord start, NexusSize range) => new((toCheck) => toCheck.IsInRange(start, range));

    internal bool Check(in NexusKey key) => _keyPressCondition(key);
    internal bool Check(in NexusCoord mousePos) => _mousePosCondition(mousePos);
}