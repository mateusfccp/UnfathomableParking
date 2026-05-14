using UnfathomableParking.Enums;
using UnfathomableParking.Interfaces;
using UnfathomableParking.Models;
using static UnfathomableParking.Services.Engine;

namespace UnfathomableParking.Components;

/// <summary>
/// A text field that can be used to input text.
/// </summary>
/// <param name="initialText">The initial text in the text field.</param>
/// <param name="formatter">An optional formatter to use for formatting the text.</param>
public class TextField(string initialText = "", string hintText = "", IInputFormatter? formatter = null)
{
    /// <summary>
    /// The text inputted in the text field.
    /// </summary>
    public string Text { get; private set; } = initialText;

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
        var effectiveStyle = style ?? new Style();
        var textStyle = Text == "" ? new Style(decoration: Decoration.Faint) : new Style();

        canvas.DrawBox(x, y, width, 3, style: effectiveStyle);
        canvas.Draw(GetInputText(width - 2), x + 1, y + 1, textStyle);
    }

    /// <summary>
    /// Process a key press.
    /// <br />
    /// This should be called when the text field is focused.
    /// </summary>
    /// <param name="keyInfo">The </param>
    public void ProcessKey(ConsoleKeyInfo keyInfo)
    {
        string candidateText;
        if (keyInfo.Key == ConsoleKey.Backspace)
        {
            candidateText = Text.Length > 0 ? Text.Substring(0, Text.Length - 1) : "";
        }
        else if (!char.IsControl(keyInfo.KeyChar))
        {
            candidateText = Text + keyInfo.KeyChar;
        }
        else
        {
            candidateText = Text;
        }

        Text = formatter == null ? candidateText : formatter.Format(Text, candidateText);
    }

    private string GetInputText(uint width)
    {
        var text = Text == "" ? hintText : Text;
        var prefix = text.Length > width ? "◀" : "";
        var startIndex = Math.Max(0, text.Length - width + prefix.Length);

        return $"{prefix}{text[(int)startIndex..]}";
    }
    class numberFormatter()
    {

    }
}
