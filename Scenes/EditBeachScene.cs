using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Reflection;
using System.Runtime.Serialization;
using Microsoft.VisualBasic;
using UnfathomableParking.Components;
using UnfathomableParking.Enums;
using UnfathomableParking.Interfaces;
using UnfathomableParking.Models;
using UnfathomableParking.Scenes;
using UnfathomableParking.Services;
using static UnfathomableParking.Services.Engine;

class EditBeachScene : IScene
{
    const int AmountOfFields = 6;
    int selectedField;
    string errorMessage = "";
    private readonly TextField _nameField;
    private readonly TextField _revenueField;
    private readonly TextField _widthField;
    private readonly TextField _heightField;
    private readonly Button _createButton;
    ParkingBeachManager manager;
    IScene lastScene;
    NumberFormatter validateInputs;
    RevenueFormatter validateRevenue;
    ParkingBeach? currentBeach;
    public EditBeachScene(ParkingBeachManager manager, IScene last, ParkingBeach? currentBeach = null)
    {
        this.manager = manager;
        this.lastScene = last;
        this.currentBeach = currentBeach;
        validateInputs = new NumberFormatter();
        validateRevenue = new RevenueFormatter();
        _nameField = new TextField(currentBeach?.Name ?? "", "Ex: Parking beach 01");
        _revenueField = new TextField(currentBeach?.RevenuePerHour + "", "Ex: 15,50", validateRevenue);
        _widthField = new TextField(currentBeach?.Width + "" ?? "", "0", validateInputs);
        _heightField = new TextField(currentBeach?.Height + "" ?? "", "0", validateInputs);
        _createButton = new Button("Confirm Edit", EditBeach);
    }

    public void Draw(Engine.Canvas canvas)
    {
        canvas.Clear();
        const uint CanvasMaxWidth = 40;
        const uint CanvasHeight = 36;
        uint canvasWidth = (uint)Math.Min(CanvasMaxWidth, canvas.Width);
        var originX = (uint)(canvas.Width / 2 - canvasWidth / 2);
        var originY = (uint)(canvas.Height / 2 - CanvasHeight / 2);
        var selectedStyle = new Style(foregroundColor: Color.DodgerBlue);
        var defaultStyle = new Style();
        var createStyle = new Style(foregroundColor: Color.DarkTurquoise);
        var textStyle = new Style(foregroundColor: Color.CornflowerBlue);
        var errorStyle = new Style(foregroundColor: Color.Red);

        canvas.DrawBox(originX, originY, canvasWidth, CanvasHeight);

        string nameText = "Edit a parking beach";
        canvas.Draw(nameText, originX + canvasWidth / 2, originY + 1, createStyle, Alignment.Center);
        // Beach Selection box
        canvas.Draw("Select beach", originX + 3, originY + 4, textStyle);
        canvas.DrawBox(originX + 2, originY + 5, canvasWidth - 4, 3, selectedField == 0 ? selectedStyle : defaultStyle);
        canvas.Draw(currentBeach?.Name ?? "Select beach", originX + 3, originY + 6, currentBeach == null ? new Style(decoration: Decoration.Faint) : new Style());
        canvas.Draw("▼", originX + canvasWidth - 4, originY + 6);
        // Name box
        canvas.Draw("New Name", originX + 3, originY + 9, textStyle);
        _nameField.Draw(canvas, originX + 2, originY + 10, canvasWidth - 4, selectedField == 1 ? selectedStyle : defaultStyle);
        // Revenue box
        canvas.Draw("New Revenue", originX + 3, originY + 14, textStyle);
        _revenueField.Draw(canvas, originX + 2, originY + 15, canvasWidth - 4, selectedField == 2 ? selectedStyle : defaultStyle);
        // Width box
        canvas.Draw("New Width", originX + 3, originY + 19, textStyle);
        _widthField.Draw(canvas, originX + 2, originY + 20, canvasWidth - 4, selectedField == 3 ? selectedStyle : defaultStyle);
        // Height box
        canvas.Draw("New Height", originX + 3, originY + 24, textStyle);
        _heightField.Draw(canvas, originX + 2, originY + 25, canvasWidth - 4, selectedField == 4 ? selectedStyle : defaultStyle);
        // Edit button
        _createButton.Draw(canvas, originX + canvasWidth / 2 - canvasWidth / 4, originY + CanvasHeight - 5, canvasWidth / 2, selectedField == 5 ? selectedStyle : defaultStyle);
        //Error message
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
            case 0 when keyInfo.Key == ConsoleKey.Enter:
                if (manager.ParkingBeaches.Count == 0)
                {
                    errorMessage = "No beaches available.";
                    break;
                }
                var initialIndex = currentBeach == null ? 0 : (uint)manager.ParkingBeaches.IndexOf(currentBeach);
                Instance?.UpdateScene(new ListSelectionScene<ParkingBeach>(manager.ParkingBeaches, initialIndex, onSelect: OnSelect, formatter: beach => beach.Name));
                break;
            case 1:
                _nameField.ProcessKey(keyInfo);
                break;
            case 2:
                _revenueField.ProcessKey(keyInfo);
                break;
            case 3:
                _widthField.ProcessKey(keyInfo);
                break;
            case 4:
                _heightField.ProcessKey(keyInfo);
                break;
            case 5:
                _createButton.ProcessKey(keyInfo);
                break;


        }
    }
    void OnSelect(ParkingBeach beach)
    {
        Instance?.UpdateScene(
            new EditBeachScene(manager, lastScene, beach)
        );
    }

    private void EditBeach()
    {
        if (!string.IsNullOrWhiteSpace(_nameField.Text) && _nameField.Text.Length < 31)
        {
            if (!manager.ParkingBeaches.Exists(p => p.Name == _nameField.Text) || _nameField.Text.Equals(currentBeach?.Name))
            {
                if (currentBeach != null)
                {
                    if (!string.IsNullOrWhiteSpace(_revenueField.Text))
                    {
                        if (uint.TryParse(_widthField.Text, out uint w) && uint.TryParse(_heightField.Text, out uint h) && w > 0 && h > 0 && w <= 100 && h <= 100)
                        {
                            decimal revenue = decimal.TryParse(_revenueField.Text, out var result) ? result : 0;
                            manager.EditParkingBeach(currentBeach.Name, _nameField.Text, w, h, revenue);
                            Engine.Instance?.UpdateScene(lastScene);
                        }
                        else
                        {
                            errorMessage = "Size must be 1-100";
                        }
                    }
                    else
                    {
                        errorMessage = "Missing revenue;";
                    }

                }
                else
                {
                    errorMessage = "Beach not selected";
                }
            }
            else
            {
                errorMessage = "Name already exists";
            }
        }
        else
        {
            errorMessage = "Name is not valid";
        }
    }
    class NumberFormatter : IInputFormatter
    {
        public string Format(string current, string next)
        {
            if (string.IsNullOrEmpty(next)) return next;
            if (next.Length < current.Length) return next;
            if (next.Length > 3) return current;
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
            if (next.Length > 10) return current;
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