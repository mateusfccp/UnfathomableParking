namespace UnfathomableParking.Enums;

/// <summary>
/// Decorations that can be applied to a console character.
/// </summary>
[Flags]
public enum Decoration
{
    /// <summary>
    /// No decoration.
    /// </summary>
    None = 0,

    /// <summary>
    /// Makes the character bold.
    /// </summary>
    Bold = 1 << 0,

    /// <summary>
    /// Makes the character faint (dimmed).
    /// </summary>
    Faint = 1 << 1,

    /// <summary>
    /// Makes the character italic.
    /// </summary>
    Italic = 1 << 2,

    /// <summary>
    /// Underlines the character.
    /// </summary>
    Underline = 1 << 3,

    /// <summary>
    /// Makes the character blink.
    /// </summary>
    Blink = 1 << 4,

    /// <summary>
    /// Inverts the colors of the character.
    /// <br />
    /// The foreground color becomes the background color and vice versa.
    /// </summary>
    Invert = 1 << 5,

    /// <summary>
    /// Hides the character.
    /// </summary>
    Conceal = 1 << 6,

    /// <summary>
    /// Strikes through the character.
    /// </summary>
    Strikethrough = 1 << 7,
}
