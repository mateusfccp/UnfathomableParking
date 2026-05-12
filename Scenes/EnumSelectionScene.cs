using System.Drawing;
using UnfathomableParking.Enums;
using UnfathomableParking.Interfaces;
using UnfathomableParking.Models;
using UnfathomableParking.Services;

namespace UnfathomableParking.Scenes;

/// <summary>
/// A scene that displays a list of enum values.
/// </summary>
/// <typeparam name="T">The type of enum to display.</typeparam>
public class EnumSelectionScene<T> : IScene where T : struct, Enum
{
    private readonly string? _title;
    private static readonly T[] Options = Enum.GetValues<T>();
    private readonly uint _maximumLength;
    private uint _selectedIndex;
    private readonly Func<T, string> _formatter = option => option.ToString();
    private readonly Action<T> _onSelect = _ => { };

    /// <summary>
    /// Creates a new EnumSelectionScene.
    /// </summary>
    /// <param name="initialSelectedIndex">The initial index of the selected option.</param>
    /// <param name="formatter">A custom formatter for enum options. Defaults to ToString() if null.</param>
    /// <param name="onSelect">A callback that is called when an option is selected. Defaults to a no-op if null.</param>
    /// <param name="title">An optional title for the selection scene.</param>
    public EnumSelectionScene(
        uint initialSelectedIndex = 0,
        Func<T, string>? formatter = null,
        Action<T>? onSelect = null,
        string? title = null
    )
    {
        _title = title;
        _maximumLength = (uint)Options.Select(option => option.ToString().Length).Max();
        _selectedIndex = initialSelectedIndex;
        _formatter = formatter ?? _formatter;
        _onSelect = onSelect ?? _onSelect;
    }

    public void Draw(Engine.Canvas canvas)
    {
        canvas.Clear();

        var width = (uint)Math.Min(_maximumLength + 2, canvas.Width) + 2;
        var height = (uint)Math.Min(Options.Length + 2, canvas.Height);
        var originX = (uint)(canvas.Width / 2 - width / 2);
        var originY = (uint)(canvas.Height / 2 - height / 2);

        canvas.DrawBox(originX, originY, width, height);

        for (uint i = 0; i < Options.Length; i++)
        {
            var option = Options[i];
            var isSelected = i == _selectedIndex;
            canvas.Draw(_formatter(option), originX + 2, originY + i + 1,
                isSelected ? new Style(foregroundColor: Color.DodgerBlue) : new Style());
        }

        if (_title is { } title)
        {
            canvas.Draw(
                title,
                originX + width / 2,
                originY - 2,
                new Style(decoration: Decoration.Bold),
                Alignment.Center
            );
        }
    }

    public void OnKeyPressed(ConsoleKeyInfo keyInfo)
    {
        _selectedIndex = keyInfo.Key switch
        {
            ConsoleKey.UpArrow => (uint)((_selectedIndex - 1 + Options.Length) % Options.Length),
            ConsoleKey.DownArrow => (uint)((_selectedIndex + 1) % Options.Length),
            _ => _selectedIndex
        };

        if (keyInfo.Key == ConsoleKey.Enter)
        {
            _onSelect(Options[_selectedIndex]);
        }
    }
}
