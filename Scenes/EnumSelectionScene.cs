namespace UnfathomableParking.Scenes;

/// <summary>
/// A scene that displays a list of enum values.
/// </summary>
/// <typeparam name="T">The type of enum to display.</typeparam>
public class EnumSelectionScene<T>(
    uint initialSelectedIndex = 0,
    Func<T, string>? formatter = null,
    Action<T>? onSelect = null,
    string? title = null)
    : ListSelectionScene<T>(Enum.GetValues<T>().ToList(), initialSelectedIndex, formatter, onSelect, title)
    where T : struct, Enum;
