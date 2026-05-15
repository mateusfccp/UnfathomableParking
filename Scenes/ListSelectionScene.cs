using System.Drawing;
using UnfathomableParking.Interfaces;
using UnfathomableParking.Models;
using UnfathomableParking.Services;

namespace UnfathomableParking.Scenes;

/// <summary>
/// A scene that displays a list of enum values.
/// </summary>
/// <typeparam name="T">The type of enum to display.</typeparam>
public class ListSelectionScene<T> : IScene
{
    private readonly List<T>? _options;
    private readonly uint _maximumLength;
    private uint _selectedIndex;
    private readonly Func<T, string> _formatter = option => option.ToString();
    private readonly Action<T> _onSelect = _ => { };

    /// <summary>
    /// Creates a new ListSelectionScene.
    /// </summary>
    /// <param name="initialSelectedIndex">The initial index of the selected option.</param>
    /// <param name="formatter">A custom formatter for enum _options. Defaults to ToString() if null.</param>
    /// <param name="onSelect">A callback that is called when an option is selected. Defaults to a no-op if null.</param>
    public ListSelectionScene(List<T> _options, uint initialSelectedIndex = 0, Func<T, string>? formatter = null,
        Action<T>? onSelect = null)
    {
        this._options = _options;
        _maximumLength = (uint)_options.Select(option => formatter(option).Length).Max();
        _selectedIndex = initialSelectedIndex;
        _formatter = formatter ?? _formatter;
        _onSelect = onSelect ?? _onSelect;
    }

    public void Draw(Engine.Canvas canvas)
    {
        canvas.Clear();

        var width = (uint)Math.Min(_maximumLength + 2, canvas.Width) + 2;
        var height = (uint)Math.Min(_options.Count + 2, canvas.Height);
        var originX = (uint)(canvas.Width / 2 - width / 2);
        var originY = (uint)(canvas.Height / 2 - height / 2);

        canvas.DrawBox(originX, originY, width, height);

        for (uint i = 0; i < _options.Count; i++)
        {
            var option = _options[(int)i];
            var isSelected = i == _selectedIndex;
            canvas.Draw(_formatter(option), originX + 2, originY + i + 1,
                isSelected ? new Style(foregroundColor: Color.DodgerBlue) : new Style());
        }
    }

    public void OnKeyPressed(ConsoleKeyInfo keyInfo)
    {
        _selectedIndex = keyInfo.Key switch
        {
            ConsoleKey.UpArrow => (uint)((_selectedIndex - 1 + _options.Count) % _options.Count),
            ConsoleKey.DownArrow => (uint)((_selectedIndex + 1) % _options.Count),
            _ => _selectedIndex
        };

        if (keyInfo.Key == ConsoleKey.Enter)
        {
            _onSelect(_options[(int)_selectedIndex]);
        }
    }
}
