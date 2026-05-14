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
    int amountOfFields = 5;
    int selectedField;
    string errorMessage = "";
    TextField beachName;
    TextField beachWidth;
    TextField beachHeight;
    Button createBeach;
    ParkingBeachManager manager;
    IScene lastScene;
    NumberFormatter validateInputs;
    ParkingBeach? currentBeach;
    public EditBeachScene(ParkingBeachManager manager, IScene last, ParkingBeach? currentBeach = null)
    {
        this.manager = manager;
        this.lastScene = last;
        this.currentBeach = currentBeach;
        validateInputs = new NumberFormatter();

        beachName = new TextField(currentBeach?.Name ?? "", "Ex: Parking beach 01");
        beachWidth = new TextField(currentBeach?.Width + "" ?? "", "0", validateInputs);
        beachHeight = new TextField(currentBeach?.Height + "" ?? "", "0", validateInputs);
        createBeach = new Button("Confirm Edit", EditBeach);
    }

    public void Draw(Engine.Canvas canvas)
    {
        canvas.Clear();
        uint canvasMaxWidth = 40;
        uint canvasHeight = 32;
        uint canvasWidth = (uint)Math.Min(canvasMaxWidth, canvas.Width);
        var originX = (uint)(canvas.Width / 2 - canvasWidth / 2);
        var originY = (uint)(canvas.Height / 2 - canvasHeight / 2);
        var selectedStyle = new Style(foregroundColor: Color.DodgerBlue);
        var defaultStyle = new Style();
        var createStyle = new Style(foregroundColor: Color.DarkTurquoise);
        var textStyle = new Style(foregroundColor: Color.CornflowerBlue);
        var errorStyle = new Style(foregroundColor: Color.Red);

        canvas.DrawBox(originX, originY, canvasWidth, canvasHeight);

        string nameText = "Edit a parking beach";
        canvas.Draw(nameText, originX + canvasWidth / 2, originY + 1, createStyle, Alignment.Center);
        // Beach Selection box
        canvas.Draw("Select beach", originX + 3, originY + 4, textStyle);
        canvas.DrawBox(originX + 2, originY + 5, canvasWidth - 4, 3, selectedField == 0 ? selectedStyle : defaultStyle);
        canvas.Draw(currentBeach?.Name ?? "Select beach", originX + 3, originY + 6, currentBeach == null ? new Style(decoration: Decoration.Faint) : new Style());
        canvas.Draw("▼", originX + canvasWidth - 4, originY + 6);
        // Name box
        canvas.Draw("New Name", originX + 3, originY + 9, textStyle);
        beachName.Draw(canvas, originX + 2, originY + 10, canvasWidth - 4, selectedField == 1 ? selectedStyle : defaultStyle);
        // Width box
        canvas.Draw("New Width", originX + 3, originY + 14, textStyle);
        beachWidth.Draw(canvas, originX + 2, originY + 15, canvasWidth - 4, selectedField == 2 ? selectedStyle : defaultStyle);
        // Height box
        canvas.Draw("New Height", originX + 3, originY + 19, textStyle);
        beachHeight.Draw(canvas, originX + 2, originY + 20, canvasWidth - 4, selectedField == 3 ? selectedStyle : defaultStyle);
        // Edit button
        createBeach.Draw(canvas, originX + canvasWidth / 2 - canvasWidth / 4, originY + canvasHeight - 5, canvasWidth / 2, selectedField == 4 ? selectedStyle : defaultStyle);
        //Error message
        if (!string.IsNullOrEmpty(errorMessage))
        {
            canvas.Draw("ERROR:", originX + canvasWidth / 2, originY + canvasHeight - 8, errorStyle, Alignment.Center);
            canvas.Draw(errorMessage, originX + canvasWidth / 2, originY + canvasHeight - 7, errorStyle, Alignment.Center);
        }
    }

    public void OnKeyPressed(ConsoleKeyInfo keyInfo)
    {
        if (selectedField < amountOfFields - 1 && keyInfo.Key == ConsoleKey.DownArrow)
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
                var initialIndex = currentBeach == null ? 0 : (uint)manager.ParkingBeaches.IndexOf(currentBeach);
                Instance?.UpdateScene(new ListSelectionScene<ParkingBeach>(manager.ParkingBeaches, initialIndex, onSelect: OnSelect, formatter: beach => beach.Name));
                break;
            case 1:
                beachName.ProcessKey(keyInfo);
                break;
            case 2:
                beachWidth.ProcessKey(keyInfo);
                break;
            case 3:
                beachHeight.ProcessKey(keyInfo);
                break;
            case 4:
                createBeach.ProcessKey(keyInfo);
                break;


        }
    }
    void OnSelect(ParkingBeach beach)
    {
        Instance?.UpdateScene(
            new EditBeachScene(manager, lastScene, beach)
        );
    }

    public void EditBeach()
    {
        if (!string.IsNullOrWhiteSpace(beachName.Text) && beachName.Text.Length < 31)
        {
            if (!manager.ParkingBeaches.Exists(p => p.Name == beachName.Text) || beachName.Text.Equals(currentBeach?.Name))
            {
                if (currentBeach != null)
                {
                    if (uint.TryParse(beachWidth.Text, out uint w) && uint.TryParse(beachHeight.Text, out uint h) && w > 0 && h > 0)
                    {
                        manager.EditParkingBeach(currentBeach.Name, beachName.Text, w, h);
                        Engine.Instance?.UpdateScene(lastScene);
                    }
                    else
                    {
                        errorMessage = "Width and Height must not be zero";
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
            if (char.IsDigit(next.Last())) return next;
            return current;
        }
    }
}