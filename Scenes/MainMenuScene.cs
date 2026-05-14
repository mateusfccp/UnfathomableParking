using System.Drawing;
using UnfathomableParking.Enums;
using UnfathomableParking.Interfaces;
using UnfathomableParking.Models;
using UnfathomableParking.Services;

namespace UnfathomableParking.Scenes;

public class MainMenuScene(ParkingBeachManager beachManager, int headIndex, int selectedFieldIndex) : IScene
{
    private int _headIndex = headIndex;
    private int _selectedFieldIndex = selectedFieldIndex;
    private SortingState _sortingState = SortingState.capacity;
    private List<ParkingBeach> _parkingBeaches => beachManager.ParkingBeaches;
    private List<ParkingBeach>? _visualParkingBeaches;
    public ParkingBeach? SelectedBeach => _visualParkingBeaches?[_selectedFieldIndex];

    public void Draw(Engine.Canvas canvas)
    {
        canvas.Clear();
        const uint maximumWidth = 51;
        const int height = 22;
        var width = Math.Min(maximumWidth, canvas.Width);
        var originX = (uint)(canvas.Width / 2 - width / 2);
        var originY = (uint)(canvas.Height / 2 - height / 2);

        var defaultStyle = new Style();
        var selectedStyle = new Style(foregroundColor: Color.DodgerBlue);


        // Title
        canvas.Draw("╷ ╷╭╮╷╭─╴╭─╮╶┬╴╷ ╷╭─╮╭┬╮╭─╮╭╮ ╷  ╭─╴   ╭─╮╭─╮╭─╮╷╭ ╷╭╮╷╭─╴", (uint)(canvas.Width / 2 - 29), originY - 5);
        canvas.Draw("│ ││╰┤├╴ ├─┤ │ ├─┤│ ││││├─┤├┴╮│  ├╴    ├─╯├─┤├┬╯├┴╮││╰┤│╶╮", (uint)(canvas.Width / 2 - 29), originY - 4);
        canvas.Draw("╰─╯╵ ╵╵  ╵ ╵ ╵ ╵ ╵╰─╯╵ ╵╵ ╵╰─╯╰─╴╰─╴   ╵  ╵ ╵╵╰╴╵ ╵╵╵ ╵╰─╯", (uint)(canvas.Width / 2 - 29), originY - 3);

        // Sorting state
        canvas.Draw($"Sorting by:", originX - 1, originY - 1);
        canvas.Draw(_sortingState.ToString(), originX + 11, originY - 1, new Style(Color.Gold));

        // Main Box
        canvas.DrawBox(originX - 2, originY, (uint)width, height);

        // Parking lot list visual - TODO: Display name, revenue and free spots
        if (_parkingBeaches.Count >= 4)
        {
            _visualParkingBeaches = _parkingBeaches[_headIndex..(_headIndex + 4)];
            for (int i = 0; i < 4; i++) // Hacer lista de los que entran (son 4)
            {
                canvas.DrawBox(originX - 1, (uint)(originY + 1 + i * 5), (uint)width - 2, 5, _selectedFieldIndex == i ? selectedStyle : defaultStyle);
                canvas.Draw($"{_visualParkingBeaches[i].Width.ToString()}×{_visualParkingBeaches[i].Height.ToString()} parking slots", originX, (uint)(originY + 1 + i * 5) + 2);
            }
        }
        else // La lista tiene menos que 4 elementos
        {
            _visualParkingBeaches = _parkingBeaches;
            for (int i = 0; i < _visualParkingBeaches.Count; i++) // Hacer lista completa
            {
                canvas.DrawBox(originX - 1, (uint)(originY + 1 + i * 5), (uint)width - 2, 5, _selectedFieldIndex == i ? selectedStyle : defaultStyle);
                canvas.Draw($"{_visualParkingBeaches[i].Width.ToString()}×{_visualParkingBeaches[i].Height.ToString()} parking slots", originX, (uint)(originY + 1 + i * 5) + 2);
            }
        }

        // Pseudo slider -> TODO: slider visual logic
        for (int i = 0; i < height + 3; i++)
        {
            if (_headIndex != i) canvas.Draw("█", originX + (uint)width, originY + (uint)i, new Style(Color.DimGray));
            else canvas.Draw("█", originX + (uint)width, originY + (uint)i, new Style(Color.White, decoration: Decoration.Bold));
        }

        // TODO: Ordenar hardcodeo -> poner un error cuando la pantalla sea muy chica

        // Create Button
        canvas.DrawBox(originX - 2, originY + height, (uint)(width / 4), 3);
        canvas.Draw("Create (C)", originX - 3, originY + height + 1);

        // Update Button
        canvas.DrawBox(originX - 2 + (uint)(width / 4) + 1, originY + height, (uint)(width / 4), 3);
        canvas.Draw("Edit (E)", originX - 2 + (uint)(width / 4) + 3, originY + height + 1);

        // Delete Button
        canvas.DrawBox(originX - 2 + 2 * ((uint)(width / 4)) + 2, originY + height, (uint)(width / 4), 3);
        canvas.Draw("Delete (D)", originX - 2 + 2 * ((uint)(width / 4)) + 3, originY + height + 1);

        // Sort Button
        canvas.DrawBox(originX - 2 + 3 * ((uint)(width / 4)) + 3, originY + height, (uint)(width / 4), 3);
        canvas.Draw("Sort (S)", originX - 2 + 3 * ((uint)(width / 4)) + 5, originY + height + 1);
    }

    public void OnKeyPressed(ConsoleKeyInfo keyInfo)
    {
        // Clamps
        if (_parkingBeaches.Count >= 4) _headIndex = Math.Clamp(_headIndex, 0, _parkingBeaches.Count - 4);
        else _headIndex = 0;

        // Basic switch case format
        switch (keyInfo.Key)
        {
            case ConsoleKey.DownArrow:
                _selectedFieldIndex++;
                if (_selectedFieldIndex == 4 && !(_headIndex + 4 == _parkingBeaches.Count)) _headIndex++;
                break;
            case ConsoleKey.UpArrow:
                _selectedFieldIndex--;
                if (_selectedFieldIndex == -1 && !(_headIndex == 0)) _headIndex--;
                break;
            case ConsoleKey.Enter:
                if (_parkingBeaches != null && _parkingBeaches.Count != 0 && SelectedBeach != null) Engine.Instance?.UpdateScene(new ParkingBeachScene(SelectedBeach, beachManager, _headIndex, _selectedFieldIndex));
                break;
            case ConsoleKey.S:
                // TODO: Logica de sorteo
                break;

            // ACA TENES QUE HACER TU PARTE JOAQUIN
            case ConsoleKey.C:
                break;
            case ConsoleKey.E:
                break;
            case ConsoleKey.D:
                break;
            // Yo recomiendo que hagas una variable Scene? newScene = TuScene y despues haces el update(newScene)

        }

        if (_parkingBeaches != null &&  _parkingBeaches.Count >= 4) _selectedFieldIndex = Math.Clamp(_selectedFieldIndex, 0, 3);
        else if (_parkingBeaches != null && _parkingBeaches.Count != 0) _selectedFieldIndex = Math.Clamp(_selectedFieldIndex, 0, _parkingBeaches.Count - 1);
    }
}