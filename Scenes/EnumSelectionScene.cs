using System.Drawing;
using UnfathomableParking.Interfaces;
using UnfathomableParking.Models;
using UnfathomableParking.Services;

namespace UnfathomableParking.Scenes;

/// <summary>
/// A scene that displays a list of enum values.
/// </summary>
/// <typeparam name="T">The type of enum to display.</typeparam>
public class EnumSelectionScene<T> : ListSelectionScene<T> where T : struct, Enum
{
    public EnumSelectionScene(uint initialSelectedIndex = 0, Func<T, string>? formatter = null, Action<T>? onSelect = null) : base(Enum.GetValues<T>().ToList(), initialSelectedIndex, formatter, onSelect)
    {
    }
}
