using System.Drawing;
using UnfathomableParking.Enums;
using UnfathomableParking.Interfaces;
using UnfathomableParking.Models;
using UnfathomableParking.Services;

namespace UnfathomableParking.Scenes;

internal class MainMenuScene : IScene
{
    private int _sliderPos = 3;
    private SortingState _sortingState = SortingState.capacity;

    public void Draw(Engine.Canvas canvas)
    {
        canvas.Clear();
        const uint maximumWidth = 51;
        const int height = 24;
        var width = Math.Min(maximumWidth, canvas.Width);
        var originX = (uint)(canvas.Width / 2 - width / 2);
        var originY = (uint)(canvas.Height / 2 - height / 2);

        //var defaultStyle = new Style();
        //var selectedStyle = new Style(foregroundColor: Color.DodgerBlue);


        // Title
        canvas.Draw("╷ ╷╭╮╷╭─╴╭─╮╶┬╴╷ ╷╭─╮╭┬╮╭─╮╭╮ ╷  ╭─╴   ╭─╮╭─╮╭─╮╷╭ ╷╭╮╷╭─╴", (uint)(canvas.Width/2 - 29), originY - 5);
        canvas.Draw("│ ││╰┤├╴ ├─┤ │ ├─┤│ ││││├─┤├┴╮│  ├╴    ├─╯├─┤├┬╯├┴╮││╰┤│╶╮", (uint)(canvas.Width / 2 - 29), originY - 4);
        canvas.Draw("╰─╯╵ ╵╵  ╵ ╵ ╵ ╵ ╵╰─╯╵ ╵╵ ╵╰─╯╰─╴╰─╴   ╵  ╵ ╵╵╰╴╵ ╵╵╵ ╵╰─╯", (uint)(canvas.Width / 2 - 29), originY - 3)

        // Sorting state
        canvas.Draw($"Sorting by:", originX - 1, originY - 1);
        canvas.Draw(_sortingState.ToString(), originX + 11, originY - 1, new Style(Color.Gold));

        // Main Box
        canvas.DrawBox(originX - 2, originY, (uint)width, height);

        // TODO: Actuall freaking list

        // Pseudo slider
        for (int i = 0; i < height + 3; i++)
        {
            if (_sliderPos != i) canvas.Draw("█", originX + (uint)width, originY + (uint)i, new Style(Color.DimGray));
            else canvas.Draw("█", originX + (uint)width, originY + (uint)i, new Style(Color.White, null, Enums.Decoration.Bold));
        }

        // Create Button
        canvas.DrawBox(originX - 2, originY + height, (uint)(width/4), 3);
        canvas.Draw("Create (C)", originX - 2 + 1, originY + height + 1);

        // Update Button
        canvas.DrawBox(originX - 2 + (uint)(width / 4) + 1, originY + height, (uint)(width / 4), 3);
        canvas.Draw("Update (U)", originX - 2 + (uint)(width / 4) + 2, originY + height + 1);

        // Delete Button
        canvas.DrawBox(originX - 2 + 2 * ((uint)(width / 4)) + 2, originY + height, (uint)(width / 4), 3);
        canvas.Draw("Delete (D)", originX - 2 + 2 * ((uint)(width / 4)) + 3, originY + height + 1);

        // Sort Button
        canvas.DrawBox(originX - 2 + 3 * ((uint)(width / 4)) + 3, originY + height, (uint)(width / 4), 3);
        canvas.Draw("Sort (S)", originX - 2 + 3 * ((uint)(width / 4)) + 5, originY + height + 1);

        // Description -> we'll see if I do it or not

    }

    public void OnKeyPressed(ConsoleKeyInfo keyInfo)
    {
        if (keyInfo.Key == ConsoleKey.Enter)
        {
            Engine.Instance?.UpdateScene(new ParkingBeachScene(new Models.ParkingBeach(10, 10), 1, 1));
        }
    }
}
