using System.Drawing;
using UnfathomableParking.Enums;

namespace UnfathomableParking.Models;

public readonly record struct Style
{
    /// <summary>
    /// The foreground color of the style.
    /// </summary>
    public Color? ForegroundColor { get; init; }

    /// <summary>
    /// The background color of the style.
    /// </summary>
    public Color? BackgroundColor { get; init; }

    /// <summary>
    /// The decoration of the style.
    /// </summary>
    public Decoration Decoration { get; init; }

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
}
