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
    int amountOfFields = 4;
    int selectedField;
    string errorMessage = "";
    TextField beachName;
    TextField beachWidth;
    TextField beachHeight;
    Button createBeach;
    ParkingBeachManager manager;
    IScene lastScene;
    NumberFormatter validateInputs;
    public CreateBeachScene(ParkingBeachManager manager, IScene last)
    {
        this.manager = manager;
        this.lastScene = last;


        validateInputs = new NumberFormatter();
        beachName = new TextField("", "Ex: Parking beach 01");
        beachWidth = new TextField("", "0", validateInputs);
        beachHeight = new TextField("", "0", validateInputs);
        createBeach = new Button("Create", SaveNewBeach);
    }

    public void Draw(Engine.Canvas canvas)
    {
        canvas.Clear();
        uint canvasMaxWidth = 40;
        uint canvasHeight = 28;
        uint canvasWidth = (uint)Math.Min(canvasMaxWidth, canvas.Width);
        var originX = (uint)(canvas.Width / 2 - canvasWidth / 2);
        var originY = (uint)(canvas.Height / 2 - canvasHeight / 2);
        var selectedStyle = new Style(foregroundColor: Color.DodgerBlue);
        var defaultStyle = new Style();
        var createStyle = new Style(foregroundColor: Color.DarkTurquoise);
        var textStyle = new Style(foregroundColor: Color.CornflowerBlue);
        var errorStyle = new Style(foregroundColor: Color.Red);

        canvas.DrawBox(originX, originY, canvasWidth, canvasHeight);

        string nameText = "Create a parking beach";
        canvas.Draw(nameText, originX + canvasWidth / 2, originY + 1, createStyle, Alignment.Center);
        // Name box
        canvas.Draw("Name", originX + 3, originY + 4, textStyle);
        beachName.Draw(canvas, originX + 2, originY + 5, canvasWidth - 4, selectedField == 0 ? selectedStyle : defaultStyle);
        // Width box
        canvas.Draw("Width", originX + 3, originY + 9, textStyle);
        beachWidth.Draw(canvas, originX + 2, originY + 10, canvasWidth - 4, selectedField == 1 ? selectedStyle : defaultStyle);
        // Height box
        canvas.Draw("Height", originX + 3, originY + 14, textStyle);
        beachHeight.Draw(canvas, originX + 2, originY + 15, canvasWidth - 4, selectedField == 2 ? selectedStyle : defaultStyle);
        // Create button
        createBeach.Draw(canvas, originX + canvasWidth / 2 - canvasWidth / 4, originY + canvasHeight - 5, canvasWidth / 2, selectedField == 3 ? selectedStyle : defaultStyle);

        // Error message
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
            case 0:
                beachName.ProcessKey(keyInfo);
                break;
            case 1:
                beachWidth.ProcessKey(keyInfo);
                break;
            case 2:
                beachHeight.ProcessKey(keyInfo);
                break;
            case 3:
                createBeach.ProcessKey(keyInfo);
                break;

        }
    }

    public void SaveNewBeach()
    {
        if (!manager.ParkingBeaches.Exists(p => p.Name == beachName.Text))
        {
            if (!string.IsNullOrWhiteSpace(beachName.Text) && beachName.Text.Length < 31)
            {
                if (uint.TryParse(beachWidth.Text, out uint w) && uint.TryParse(beachHeight.Text, out uint h) && w > 0 && h > 0)
                {
                    manager.AddParkingBeach(beachName.Text, w, h);
                    Engine.Instance?.UpdateScene(lastScene);
                }
                else
                {
                    errorMessage = "Width and Height must not be zero";
                }
            }
            else
            {
                errorMessage = "Name is not valid";
            }
        }
        else
        {
            errorMessage = "Name already exist!";
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