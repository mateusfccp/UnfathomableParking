using UnfathomableParking.Models;
using static UnfathomableParking.Services.Engine;

namespace UnfathomableParking.Components;

/// <summary>
/// A text field that can be used to input text.
/// </summary>
/// <param name="label">The label of the button.</param>
public class Button(string label, Action action)
{
    /// <summary>
    /// Draws the text field to the canvas.
    /// </summary>
    /// <param name="canvas">The canvas to draw to.</param>
    /// <param name="x">The x position of the text field.</param>
    /// <param name="y">The y position of the text field.</param>
    /// <param name="width">The width of the text field.</param>
    /// <param name="style">The style of the text field. It is only applied to the border of the text field.</param>
    public void Draw(Canvas canvas, uint x, uint y, uint width, Style? style)
    {
        canvas.DrawBox(x + width / 2 - width / 2, y, width, 3, style);
        canvas.Draw(label, x + width / 2, y + 1, style, Alignment.Center);
    }

    /// <summary>
    /// Process a key press.
    /// <br />
    /// This should be called when the button is focused.
    /// </summary>
    /// <param name="keyInfo">The </param>
    public void ProcessKey(ConsoleKeyInfo keyInfo)
    {
        if (keyInfo.Key == ConsoleKey.Enter)
        {
            action();
        }
    }
}
