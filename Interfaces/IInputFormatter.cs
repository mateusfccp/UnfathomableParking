namespace UnfathomableParking.Interfaces;

/// <summary>
/// A formatter for textual user input.
/// </summary>
public interface IInputFormatter
{
    /// <summary>
    /// Formats the input based on the current and next input.
     /// <br />
     /// This is called every time the user inputs a character, and the result is used as the new input.
    /// </summary>
    /// <param name="current">The input before the text is updated.</param>
    /// <param name="next">The input after the text is updated.</param>
    /// <returns>The formatted input.</returns>
    string Format(string current, string next);
}
