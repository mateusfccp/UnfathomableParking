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
    const int AmountOfFields = 3;
    int selectedField;
    string errorMessage = "";
    private readonly Button _deleteButton;
    private readonly Button _cancelButton;
    ParkingBeachManager manager;
    IScene lastScene;
    ParkingBeach? currentBeach;
    public DeleteBeachScene(ParkingBeachManager manager, IScene last, ParkingBeach? currentBeach = null)
    {
        this.manager = manager;
        this.lastScene = last;
        this.currentBeach = currentBeach;


        _deleteButton = new Button("Confirm Delete", DeleteBeach);
        _cancelButton = new Button("Cancel Delete", CancelDelete);
    }

    public void Draw(Engine.Canvas canvas)
    {
        canvas.Clear();
        const uint CanvasMaxWidth = 40;
        const uint CanvasHeight = 19;
        uint canvasWidth = (uint)Math.Min(CanvasMaxWidth, canvas.Width);
        var originX = (uint)(canvas.Width / 2 - canvasWidth / 2);
        var originY = (uint)(canvas.Height / 2 - CanvasHeight / 2);
        var selectedStyle = new Style(foregroundColor: Color.Red);
        var defaultStyle = new Style(foregroundColor: Color.Firebrick);
        var _cancelButtonStyle = new Style(foregroundColor: Color.LimeGreen);

        canvas.DrawBox(originX, originY, canvasWidth, CanvasHeight);

        string nameText = "Delete a parking beach";
        canvas.Draw(nameText, originX + canvasWidth / 2, originY + 1, selectedStyle, Alignment.Center);
        // Beach Selection box
        canvas.Draw("Select beach", originX + 3, originY + 4, defaultStyle);
        canvas.DrawBox(originX + 2, originY + 5, canvasWidth - 4, 3, selectedField == 0 ? selectedStyle : defaultStyle);
        canvas.Draw(currentBeach?.Name ?? "Select beach", originX + 3, originY + 6, currentBeach == null ? new Style(decoration: Decoration.Faint) : new Style());
        canvas.Draw("▼", originX + canvasWidth - 4, originY + 6);
        // Delete button
        _deleteButton.Draw(canvas, originX + canvasWidth / 2 - canvasWidth / 4, originY + CanvasHeight - 8, canvasWidth / 2, selectedField == 1 ? selectedStyle : _cancelButtonStyle);
        // Cancel button
        _cancelButton.Draw(canvas, originX + canvasWidth / 2 - canvasWidth / 4, originY + CanvasHeight - 5, canvasWidth / 2, selectedField == 2 ? selectedStyle : _cancelButtonStyle);
        //Error message
        if (!string.IsNullOrEmpty(errorMessage))
        {
            canvas.Draw("ERROR:", originX + canvasWidth / 2, originY + CanvasHeight - 10, defaultStyle, Alignment.Center);
            canvas.Draw(errorMessage, originX + canvasWidth / 2, originY + CanvasHeight - 9, defaultStyle, Alignment.Center);
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
                var initialIndex = currentBeach == null ? 0 : (uint)manager.ParkingBeaches.IndexOf(currentBeach);
                Instance?.UpdateScene(new ListSelectionScene<ParkingBeach>(manager.ParkingBeaches, initialIndex, onSelect: OnSelect, formatter: beach => beach.Name));
                break;
            case 1:
                _deleteButton.ProcessKey(keyInfo);
                break;
            case 2:
                _cancelButton.ProcessKey(keyInfo);
                break;

        }
    }
    private void OnSelect(ParkingBeach beach)
    {
        Instance?.UpdateScene(
            new DeleteBeachScene(manager, lastScene, beach)
        );
    }

    private void DeleteBeach()
    {
        if (currentBeach != null)
        {
            manager.RemoveParkingBeach(currentBeach.Name);
            Engine.Instance?.UpdateScene(lastScene);
        }
        else
        {
            errorMessage = "There's no beach selected.";
        }


    }
    private void CancelDelete()
    {
        Engine.Instance?.UpdateScene(lastScene);
    }
}
