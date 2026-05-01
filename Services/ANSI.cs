using UnfathomableParking.Enums;
using UnfathomableParking.Models;

namespace UnfathomableParking.Services;

/// <summary>
/// A collection of ANSI escape sequences.
/// </summary>
public static class Ansi
{
    /// <summary>
    /// Resets all formatting back to the terminal's default.
    /// </summary>
    public const string Reset = "\e[0m";

    /// <summary>
    /// Moves the cursor to the specified X and Y coordinates.
    /// Note: ANSI cursor positioning is 1-indexed and formatted as Row;Column.
    /// </summary>
    public static string MoveTo(int x, int y)
    {
        // Add 1 because arrays are 0-indexed but ANSI is 1-indexed.
        // Y is rows, X is columns.
        return $"\e[{y + 1};{x + 1}H";
    }

    /// <summary>
    /// Converts a Style struct into a single, chained ANSI escape sequence.
    /// </summary>
    public static string GetStyleSequence(Style style)
    {
        var codes = new List<string>(capacity: 6) { "0" };

        // Text Decorations (Bitwise flag checks)
        if (style.Decoration.HasFlag(Decoration.Bold)) codes.Add("1");
        if (style.Decoration.HasFlag(Decoration.Faint)) codes.Add("2");
        if (style.Decoration.HasFlag(Decoration.Italic)) codes.Add("3");
        if (style.Decoration.HasFlag(Decoration.Underline)) codes.Add("4");
        if (style.Decoration.HasFlag(Decoration.Blink)) codes.Add("5");
        if (style.Decoration.HasFlag(Decoration.Invert)) codes.Add("7");
        if (style.Decoration.HasFlag(Decoration.Conceal)) codes.Add("8");
        if (style.Decoration.HasFlag(Decoration.Strikethrough)) codes.Add("9");

        if (style.ForegroundColor.HasValue)
        {
            var foregroundColor = style.ForegroundColor.Value;
            codes.Add($"38;2;{foregroundColor.R};{foregroundColor.G};{foregroundColor.B}");
        }

        if (style.BackgroundColor.HasValue)
        {
            var backgroundColor = style.BackgroundColor.Value;
            codes.Add($"48;2;{backgroundColor.R};{backgroundColor.G};{backgroundColor.B}");
        }

        return $"\e[{string.Join(";", codes)}m";
    }
}
