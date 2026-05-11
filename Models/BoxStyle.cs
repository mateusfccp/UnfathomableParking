namespace UnfathomableParking.Models;

/// <summary>
/// A style for a box.
/// </summary>
/// <param name="TopLeftCorner">The top-left corner of the box.</param>
/// <param name="TopRightCorner">The top-right corner of the box.</param>
/// <param name="BottomLeftCorner">The bottom-left corner of the box.</param>
/// <param name="BottomRightCorner">The bottom-right corner of the box.</param>
/// <param name="HorizontalLine">The horizontal line of the box.</param>
/// <param name="VerticalLine">The vertical line of the box.</param>
public record struct BoxStyle(
    char TopLeftCorner,
    char TopRightCorner,
    char BottomLeftCorner,
    char BottomRightCorner,
    char HorizontalLine,
    char VerticalLine)
{
    /// <summary>
    /// The default box style.
    /// <br />
    /// It uses the `─│┌┐└┘` characters to draw the box.
    /// </summary>
    public static readonly BoxStyle Default = new(
        TopLeftCorner: '┌',
        TopRightCorner: '┐',
        BottomLeftCorner: '└',
        BottomRightCorner: '┘',
        HorizontalLine: '─',
        VerticalLine: '│'
    );
}
