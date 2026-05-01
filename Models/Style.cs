using System.Drawing;
using UnfathomableParking.Enums;

namespace UnfathomableParking.Models;

public readonly struct Style : IEquatable<Style>
{
    /// <summary>
    /// The foreground color of the style.
    /// </summary>
    public Color? ForegroundColor { get; }

    /// <summary>
    /// The background color of the style.
    /// </summary>
    public Color? BackgroundColor { get; }

    /// <summary>
    /// The decoration of the style.
    /// </summary>
    public Decoration Decoration { get; }

    /// <summary>
    /// Creates a new style.
    /// </summary>
    /// <param name="foregroundColor">The foreground color of the style.</param>
    /// <param name="backgroundColor">The background color of the style.</param>
    /// <param name="decoration">The decoration of the style.</param>
    public Style(Color? foregroundColor = null, Color? backgroundColor = null, Decoration decoration = Decoration.None)
    {
        ForegroundColor = foregroundColor;
        BackgroundColor = backgroundColor;
        Decoration = decoration;
    }

    public bool Equals(Style other)
    {
        return Nullable.Equals(ForegroundColor, other.ForegroundColor) &&
               Nullable.Equals(BackgroundColor, other.BackgroundColor) && Decoration == other.Decoration;
    }

    public override bool Equals(object? obj)
    {
        return obj is Style other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ForegroundColor, BackgroundColor, (int)Decoration);
    }

    public static bool operator ==(Style left, Style right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Style left, Style right)
    {
        return !(left == right);
    }
}
