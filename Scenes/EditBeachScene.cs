using System.Drawing;
using UnfathomableParking.Components;
using UnfathomableParking.Interfaces;
using UnfathomableParking.Models;
using UnfathomableParking.Services;
using static UnfathomableParking.Services.Engine;

namespace UnfathomableParking.Scenes;

/// <summary>
/// A scene that allows the user to create or edit a parking beach.
/// </summary>
class EditBeachScene : IScene
{
    const int AmountOfFields = 5;
    private int _selectedField;
    private string _errorMessage = "";
    private readonly TextField _nameField;
    private readonly TextField _revenueField;
    private readonly TextField _widthField;
    private readonly TextField _heightField;
    private readonly Button _createButton;
    private readonly ParkingBeachManager _manager;
    private readonly IScene _lastSceneScene;
    private readonly ParkingBeach? _currentBeach;

    private bool IsEditing => _currentBeach != null;

    public EditBeachScene(ParkingBeachManager manager, IScene lastScene, ParkingBeach? currentBeach = null)
    {
        _manager = manager;
        _lastSceneScene = lastScene;
        _currentBeach = currentBeach;
        var numberFormatter = new NumberFormatter();
        var revenueFormatter = new RevenueFormatter();
        _nameField = new TextField(currentBeach?.Name ?? "", "Ex: Parking beach 01");
        _revenueField = new TextField(currentBeach?.RevenuePerHour + "", "Ex: 15.50", revenueFormatter);
        _widthField = new TextField((currentBeach?.Width ?? 0).ToString(), "0", numberFormatter);
        _heightField = new TextField((currentBeach?.Height ?? 0).ToString(), "0", numberFormatter);
        _createButton = new Button(IsEditing ? "Confirm Edit" : "Create", Submit);
    }

    public void Draw(Canvas canvas)
    {
        canvas.Clear();
        const uint maxWidth = 40;
        const uint maxHeight = 32;
        var canvasWidth = (uint)Math.Min(maxWidth, canvas.Width);
        var originX = (uint)(canvas.Width / 2 - canvasWidth / 2);
        var originY = (uint)(canvas.Height / 2 - maxHeight / 2);
        var selectedStyle = new Style(foregroundColor: Color.DodgerBlue);
        var defaultStyle = new Style();
        var createStyle = new Style(foregroundColor: Color.DarkTurquoise);
        var textStyle = new Style(foregroundColor: Color.CornflowerBlue);
        var errorStyle = new Style(foregroundColor: Color.Red);

        canvas.DrawBox(originX, originY, canvasWidth, maxHeight);

        string nameText = "Create a parking beach";
        canvas.Draw(nameText, originX + canvasWidth / 2, originY + 1, createStyle, Alignment.Center);
        // Name box
        canvas.Draw("Name", originX + 3, originY + 4, textStyle);
        _nameField.Draw(canvas, originX + 2, originY + 5, canvasWidth - 4,
            _selectedField == 0 ? selectedStyle : defaultStyle);
        // Revenue box
        canvas.Draw("Revenue per hour", originX + 3, originY + 9, textStyle);
        _revenueField.Draw(canvas, originX + 2, originY + 10, canvasWidth - 4,
            _selectedField == 1 ? selectedStyle : defaultStyle);
        // Width box
        canvas.Draw("Width", originX + 3, originY + 14, textStyle);
        _widthField.Draw(canvas, originX + 2, originY + 15, canvasWidth - 4,
            _selectedField == 2 ? selectedStyle : defaultStyle);
        // Height box
        canvas.Draw("Height", originX + 3, originY + 19, textStyle);
        _heightField.Draw(canvas, originX + 2, originY + 20, canvasWidth - 4,
            _selectedField == 3 ? selectedStyle : defaultStyle);
        // Create button
        _createButton.Draw(canvas, originX + canvasWidth / 2 - canvasWidth / 4, originY + maxHeight - 5,
            canvasWidth / 2, _selectedField == 4 ? selectedStyle : defaultStyle);

        // Error message
        if (!string.IsNullOrEmpty(_errorMessage))
        {
            canvas.Draw("ERROR:", originX + canvasWidth / 2, originY + maxHeight - 8, errorStyle, Alignment.Center);
            canvas.Draw(_errorMessage, originX + canvasWidth / 2, originY + maxHeight - 7, errorStyle,
                Alignment.Center);
        }
    }

    public void OnKeyPressed(ConsoleKeyInfo keyInfo)
    {
        if (_selectedField < AmountOfFields - 1 && keyInfo.Key == ConsoleKey.DownArrow)
        {
            _selectedField++;
            return;
        }

        if (_selectedField > 0 && keyInfo.Key == ConsoleKey.UpArrow)
        {
            _selectedField--;
            return;
        }

        if (keyInfo.Key == ConsoleKey.Escape)
        {
            Engine.Instance?.UpdateScene(_lastSceneScene);
            return;
        }

        switch (_selectedField)
        {
            case 0:
                _nameField.ProcessKey(keyInfo);
                break;
            case 1:
                _revenueField.ProcessKey(keyInfo);
                break;
            case 2:
                _widthField.ProcessKey(keyInfo);
                break;
            case 3:
                _heightField.ProcessKey(keyInfo);
                break;
            case 4:
                _createButton.ProcessKey(keyInfo);
                break;
        }
    }

    private void Submit()
    {
        if (string.IsNullOrWhiteSpace(_nameField.Text))
        {
            _errorMessage = "The name field can't be empty!";
            return;
        }

        if (_nameField.Text.Length > 30)
        {
            _errorMessage = "The name must be at least 31 characters long!";
            return;
        }

        if (!_nameField.Text.Equals(_currentBeach?.Name) &&
            _manager.ParkingBeaches.Exists(p => p.Name == _nameField.Text))
        {
            _errorMessage = "There's already a beach with this name!";
            return;
        }

        if (string.IsNullOrWhiteSpace(_revenueField.Text))
        {
            _errorMessage = "The revenue field can't be empty!";
            return;
        }

        if (uint.TryParse(_widthField.Text, out var width) &&
            uint.TryParse(_heightField.Text, out var height) && width > 0 && height > 0)
        {
            var revenue = decimal.TryParse(_revenueField.Text, out var result) ? result : 0;

            if (_currentBeach == null)
            {
                _manager.AddParkingBeach(_nameField.Text, width, height, revenue);
            }
            else
            {
                _manager.EditParkingBeach(_currentBeach.Name, _nameField.Text, width, height, revenue);
            }

            Instance?.UpdateScene(_lastSceneScene);
        }
        else
        {
            _errorMessage = "Width and height must not be greater than zero!";
        }
    }

    class NumberFormatter : IInputFormatter
    {
        public string Format(string current, string next)
        {
            if (string.IsNullOrEmpty(next) || next.Length < current.Length || char.IsDigit(next.Last())) return next;
            return current;
        }
    }

    private class RevenueFormatter : IInputFormatter
    {
        public string Format(string current, string next)
        {
            if (string.IsNullOrEmpty(next) || next.Length < current.Length) return next;

            var lastChar = next.Last();
            if (char.IsDigit(lastChar) ||
                lastChar is '.' or ',' &&
                !current.Contains('.') &&
                !current.Contains(',')) return next;

            return current;
        }
    }
}
