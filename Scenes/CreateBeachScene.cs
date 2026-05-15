using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using Microsoft.VisualBasic;
using UnfathomableParking.Components;
using UnfathomableParking.Interfaces;
using UnfathomableParking.Models;
using UnfathomableParking.Services;

class CreateBeachScene : IScene
{
    const int AmountOfFields = 5;
    int selectedField;
    string errorMessage = "";
    private readonly TextField _nameField;
    private readonly TextField _revenueField;
    private readonly TextField _widthField;
    private readonly TextField _heightField;
    private readonly Button _createButton;
    ParkingBeachManager manager;
    IScene lastScene;
    public CreateBeachScene(ParkingBeachManager manager, IScene last)
    {
        this.manager = manager;
        this.lastScene = last;


        NumberFormatter validateInputs = new NumberFormatter();
        RevenueFormatter validateRevenue = new RevenueFormatter();
        _nameField = new TextField("", "Ex: Parking beach 01");
        _revenueField = new TextField("", "Ex: 15.50", validateRevenue);
        _widthField = new TextField("", "0", validateInputs);
        _heightField = new TextField("", "0", validateInputs);
        _createButton = new Button("Create", SaveNewBeach);
    }

    public void Draw(Engine.Canvas canvas)
    {
        canvas.Clear();
        const uint CanvasMaxWidth = 40;
        const uint CanvasHeight = 32;
        uint canvasWidth = (uint)Math.Min(CanvasMaxWidth, canvas.Width);
        var originX = (uint)(canvas.Width / 2 - canvasWidth / 2);
        var originY = (uint)(canvas.Height / 2 - CanvasHeight / 2);
        var selectedStyle = new Style(foregroundColor: Color.DodgerBlue);
        var defaultStyle = new Style();
        var createStyle = new Style(foregroundColor: Color.DarkTurquoise);
        var textStyle = new Style(foregroundColor: Color.CornflowerBlue);
        var errorStyle = new Style(foregroundColor: Color.Red);

        canvas.DrawBox(originX, originY, canvasWidth, CanvasHeight);

        string nameText = "Create a parking beach";
        canvas.Draw(nameText, originX + canvasWidth / 2, originY + 1, createStyle, Alignment.Center);
        // Name box
        canvas.Draw("Name", originX + 3, originY + 4, textStyle);
        _nameField.Draw(canvas, originX + 2, originY + 5, canvasWidth - 4, selectedField == 0 ? selectedStyle : defaultStyle);
        // Revenue box
        canvas.Draw("Revenue per hour", originX + 3, originY + 9, textStyle);
        _revenueField.Draw(canvas, originX + 2, originY + 10, canvasWidth - 4, selectedField == 1 ? selectedStyle : defaultStyle);
        // Width box
        canvas.Draw("Width", originX + 3, originY + 14, textStyle);
        _widthField.Draw(canvas, originX + 2, originY + 15, canvasWidth - 4, selectedField == 2 ? selectedStyle : defaultStyle);
        // Height box
        canvas.Draw("Height", originX + 3, originY + 19, textStyle);
        _heightField.Draw(canvas, originX + 2, originY + 20, canvasWidth - 4, selectedField == 3 ? selectedStyle : defaultStyle);
        // Create button
        _createButton.Draw(canvas, originX + canvasWidth / 2 - canvasWidth / 4, originY + CanvasHeight - 5, canvasWidth / 2, selectedField == 4 ? selectedStyle : defaultStyle);

        // Error message
        if (!string.IsNullOrEmpty(errorMessage))
        {
            canvas.Draw("ERROR:", originX + canvasWidth / 2, originY + CanvasHeight - 8, errorStyle, Alignment.Center);
            canvas.Draw(errorMessage, originX + canvasWidth / 2, originY + CanvasHeight - 7, errorStyle, Alignment.Center);
        }
    }

    public void OnKeyPressed(ConsoleKeyInfo keyInfo)
    {
        if (selectedField < AmountOfFields - 1 && keyInfo.Key == ConsoleKey.DownArrow)
        {
            selectedField++;
            return;
        }
        if (selectedField > 0 && keyInfo.Key == ConsoleKey.UpArrow)
        {
            selectedField--;
            return;
        }
        if (keyInfo.Key == ConsoleKey.Escape)
        {
            Engine.Instance?.UpdateScene(lastScene);
            return;
        }
        switch (selectedField)
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

    private void SaveNewBeach()
    {
        if (!manager.ParkingBeaches.Exists(p => p.Name == _nameField.Text))
        {
            if (!string.IsNullOrWhiteSpace(_nameField.Text))
            {
                if (_nameField.Text.Length < 31)
                {
                    if (!string.IsNullOrWhiteSpace(_revenueField.Text))
                    {
                        if (uint.TryParse(_widthField.Text, out uint w) && uint.TryParse(_heightField.Text, out uint h) && w > 0 && h > 0)
                        {
                            decimal revenue = decimal.TryParse(_revenueField.Text, out var result) ? result : 0;
                            manager.AddParkingBeach(_nameField.Text, w, h, revenue);
                            Engine.Instance?.UpdateScene(lastScene);
                        }
                        else
                        {
                            errorMessage = "Width and height must not be zero.";
                        }
                    }
                    else
                    {
                        errorMessage = "Missing revenue.";
                    }

                }
                else
                {
                    errorMessage = "Name can be at most 30 characters.";
                }
            }
            else
            {
                errorMessage = "Name field must not be empty.";
            }
        }
        else
        {
            errorMessage = "There's already a parking beach with this name!";
        }

    }
    class NumberFormatter : IInputFormatter
    {
        public string Format(string current, string next)
        {
            if (string.IsNullOrEmpty(next)) return next;
            if (next.Length < current.Length) return next;
            if (char.IsDigit(next.Last())) return next;
            return current;
        }
    }
    class RevenueFormatter : IInputFormatter
    {
        public string Format(string current, string next)
        {
            if (string.IsNullOrEmpty(next)) return next;
            if (next.Length < current.Length) return next;

            char lastChar = next.Last();
            if (char.IsDigit(lastChar)) return next;

            if ((lastChar == '.' || lastChar == ',') && !current.Contains('.') && !current.Contains(','))
            {
                return next;
            }

            return current;
        }
    }
}
