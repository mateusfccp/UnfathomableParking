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

class DeleteBeachScene : IScene
{
    int amountOfFields = 3;
    int selectedField;
    string errorMessage = "";
    Button deleteBeach;
    Button cancelDelete;
    ParkingBeachManager manager;
    IScene lastScene;
    ParkingBeach? currentBeach;
    public DeleteBeachScene(ParkingBeachManager manager, IScene last, ParkingBeach? currentBeach = null)
    {
        this.manager = manager;
        this.lastScene = last;
        this.currentBeach = currentBeach;


        deleteBeach = new Button("Confirm Delete", DeleteBeach);
        cancelDelete = new Button("Cancel Delete", CancelDelete);
    }

    public void Draw(Engine.Canvas canvas)
    {
        canvas.Clear();
        uint canvasMaxWidth = 40;
        uint canvasHeight = 19;
        uint canvasWidth = (uint)Math.Min(canvasMaxWidth, canvas.Width);
        var originX = (uint)(canvas.Width / 2 - canvasWidth / 2);
        var originY = (uint)(canvas.Height / 2 - canvasHeight / 2);
        var selectedStyle = new Style(foregroundColor: Color.Red);
        var defaultStyle = new Style(foregroundColor: Color.Firebrick);
        var cancelDeleteStyle = new Style(foregroundColor: Color.LimeGreen);

        canvas.DrawBox(originX, originY, canvasWidth, canvasHeight);

        string nameText = "Delete a parking beach";
        canvas.Draw(nameText, originX + canvasWidth / 2, originY + 1, selectedStyle, Alignment.Center);
        // Beach Selection box
        canvas.Draw("Select beach", originX + 3, originY + 4, defaultStyle);
        canvas.DrawBox(originX + 2, originY + 5, canvasWidth - 4, 3, selectedField == 0 ? selectedStyle : defaultStyle);
        canvas.Draw(currentBeach?.Name ?? "Select beach", originX + 3, originY + 6, currentBeach == null ? new Style(decoration: Decoration.Faint) : new Style());
        canvas.Draw("▼", originX + canvasWidth - 4, originY + 6);
        // Delete button
        deleteBeach.Draw(canvas, originX + canvasWidth / 2 - canvasWidth / 4, originY + canvasHeight - 8, canvasWidth / 2, selectedField == 1 ? selectedStyle : cancelDeleteStyle);
        // Cancel button
        cancelDelete.Draw(canvas, originX + canvasWidth / 2 - canvasWidth / 4, originY + canvasHeight - 5, canvasWidth / 2, selectedField == 2 ? selectedStyle : cancelDeleteStyle);
        //Error message
        if (!string.IsNullOrEmpty(errorMessage))
        {
            canvas.Draw("ERROR:", originX + canvasWidth / 2, originY + canvasHeight - 10, defaultStyle, Alignment.Center);
            canvas.Draw(errorMessage, originX + canvasWidth / 2, originY + canvasHeight - 9, defaultStyle, Alignment.Center);
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
                deleteBeach.ProcessKey(keyInfo);
                break;
            case 2:
                cancelDelete.ProcessKey(keyInfo);
                break;

        }
    }
    void OnSelect(ParkingBeach beach)
    {
        Instance?.UpdateScene(
            new DeleteBeachScene(manager, lastScene, beach)
        );
    }

    public void DeleteBeach()
    {
        if (currentBeach != null)
        {
            manager.RemoveParkingBeach(currentBeach.Name);
            Engine.Instance?.UpdateScene(lastScene);
        }
        else
        {
            errorMessage = "Beach not selected";
        }


    }
    public void CancelDelete()
    {
        Engine.Instance?.UpdateScene(lastScene);
    }
}