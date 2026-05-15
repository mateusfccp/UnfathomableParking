using System.Drawing;
using UnfathomableParking.Enums;
using UnfathomableParking.Interfaces;
using UnfathomableParking.Models;
using UnfathomableParking.Services;

namespace UnfathomableParking.Scenes;

/// <summary>
/// A class that contains all of the Main Menu UI and logic
/// </summary>
public class MainMenuScene : IScene
{
    private int _headIndex;
    private int _selectedFieldIndex;
    private ParkingBeachManager _beachManager;

    private SortingState _sortingState = SortingState.capacity;
    private List<ParkingBeach> _sortedBeaches; // default sort = capacity
    private List<ParkingBeach> _parkingBeaches => _sortedBeaches;
    private List<ParkingBeach>? _visualParkingBeaches;

    /// <summary>
    /// Gets the parking lot selected from the parking beach list;
    /// </summary>
    public ParkingBeach? SelectedBeach => _visualParkingBeaches?[_selectedFieldIndex];

    public MainMenuScene(ParkingBeachManager beachManager, int headIndex, int selectedFieldIndex, SortingState sortingState = SortingState.capacity)
    {
        _headIndex = headIndex;
        _selectedFieldIndex = selectedFieldIndex;
        _sortedBeaches = mergeSort(new List<ParkingBeach>(beachManager.ParkingBeaches), _sortingState);
        _beachManager = beachManager;
        _sortingState = sortingState;
    }

    /// <summary>
    /// A scene that displays the Main Menu
    /// </summary>
    /// <param name="canvas"></param>
    public void Draw(Engine.Canvas canvas)
    {
        canvas.Clear();
        const int width = 51;
        const int height = 22;
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

        // Parking lot list visual
        if (_parkingBeaches.Count >= 4)
        {
            _visualParkingBeaches = _parkingBeaches[_headIndex..(_headIndex + 4)];
            for (int i = 0; i < 4; i++) // Hacer lista de los que entran (son 4)
            {
                canvas.DrawBox(originX - 1, (uint)(originY + 1 + i * 5), (uint)width - 2, 5, _selectedFieldIndex == i ? selectedStyle : defaultStyle);
                canvas.Draw($"{_visualParkingBeaches[i].Name + ":", -25} {_visualParkingBeaches[i].FreeSlots + " slots", -4} {"$" + _visualParkingBeaches[i].TotalRevenue,-10}", originX, (uint)(originY + 1 + i * 5) + 2);
            }
        }
        else // La lista tiene menos que 4 elementos
        {
            _visualParkingBeaches = _parkingBeaches;
            for (int i = 0; i < _visualParkingBeaches.Count; i++) // Hacer lista completa
            {
                canvas.DrawBox(originX - 1, (uint)(originY + 1 + i * 5), (uint)width - 2, 5, _selectedFieldIndex == i ? selectedStyle : defaultStyle);
                canvas.Draw($"{"Parking lot name" + ":",-25} {_visualParkingBeaches[i].FreeSlots + " slots", -4} {"$" + _visualParkingBeaches[i].TotalRevenue,-10}", originX, (uint)(originY + 1 + i * 5) + 2); // Temporary format will change name and maybe add colour
            }
        }

        // Pseudo slider (gracias gemini)
        int barHeight = height + 3;
        int totalItems = _parkingBeaches.Count;
        int visibleItems = 4; // cantidad de elementos que entran en la caja
        int sliderSize;
        if (totalItems <= visibleItems)
        {
            sliderSize = barHeight;
        }
        else
        {
            var percentage = (double)visibleItems / totalItems;
            sliderSize = Math.Max(1, (int)Math.Round(percentage * barHeight));
        }

        int sliderHeadIndex = 0;
        if (totalItems > visibleItems)
        {
            // Ts is dark magic <- nvm i understand it now
            int maxHeadIndex = totalItems - visibleItems;
            int scrollableSpace = barHeight - sliderSize;
            double progress = (double)_headIndex / maxHeadIndex;
            sliderHeadIndex = (int)Math.Round(progress * scrollableSpace);
        }
        // Draw grey part
        for (int i = 0; i < barHeight; i++)
        {
            canvas.Draw("█", originX + (uint)width, originY + (uint)i, new Style(Color.DimGray));
        }
        // Draw slider on top
        for (int i = 0; i < sliderSize; i++)
        {
            canvas.Draw("█", originX + (uint)width, originY + (uint)(i + sliderHeadIndex), new Style(Color.White, decoration: Decoration.Bold));
        }

        // Create Button
        canvas.DrawBox(originX - 2, originY + height, (uint)(width / 4), 3);
        canvas.Draw("Create (C)", originX - 1, originY + height + 1);

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

    /// <summary>
    /// Preforms various actions and updates depending on when and what key is pressed
    /// </summary>
    /// <param name="keyInfo"></param>
    public void OnKeyPressed(ConsoleKeyInfo keyInfo)
    {
        // Ignore control and alt
        if (keyInfo.Modifiers.HasFlag(ConsoleModifiers.Control) ||
        keyInfo.Modifiers.HasFlag(ConsoleModifiers.Alt))
        {
            return;
        }

        // Head Index clamp
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
                if (_parkingBeaches != null && _parkingBeaches.Count != 0 && SelectedBeach != null) Engine.Instance?.UpdateScene(new ParkingBeachScene(SelectedBeach, _beachManager, _headIndex, _selectedFieldIndex, _sortingState));
                break;
            case ConsoleKey.S:
                _headIndex = 0;
                _selectedFieldIndex = 0;
                _sortingState = (SortingState)(((int)_sortingState + 1) % Enum.GetValues(typeof(SortingState)).Length);
                switch (_sortingState)
                {
                    case SortingState.capacity:
                        _sortedBeaches = mergeSort(_parkingBeaches, _sortingState);
                        break;
                    case SortingState.revenue:
                        _sortedBeaches = mergeSort(_parkingBeaches, _sortingState);
                        break;
                }
                break;

            // ACA TENES QUE HACER TU PARTE JOAQUIN!!!!!!!!!!!!
            case ConsoleKey.C:
                break;
            case ConsoleKey.E:
                break;
            case ConsoleKey.D:
                break;
             // Yo recomiendo que hagas una variable Scene? newScene = TuScene y despues haces el update(newScene)

        }

        // Selected Field Clamp
        if (_parkingBeaches != null && _parkingBeaches.Count >= 4) _selectedFieldIndex = Math.Clamp(_selectedFieldIndex, 0, 3);
        else if (_parkingBeaches != null && _parkingBeaches.Count != 0) _selectedFieldIndex = Math.Clamp(_selectedFieldIndex, 0, _parkingBeaches.Count - 1);
    }

    /// <summary>
    /// A method to sort the parking beach list with a mergeSort algorithm with an efficiency of O(n log n)
    /// </summary>
    /// <param name="list"></param>
    /// <param name="state"></param>
    /// <returns></returns>
    private List<ParkingBeach> mergeSort(List<ParkingBeach> list, SortingState state)
    {
        if (list != null && list.Count > 1)
        {
            int middle = list.Count / 2;
            List<ParkingBeach> L = list[0..middle]; // mitad izquierda
            List<ParkingBeach> R = list[middle..list.Count]; // mitad derecha

            //Ordenar las 2 mitades
            mergeSort(L, state);
            mergeSort(R, state);

            int i = 0, j = 0, k = 0;

            switch (state)
            {
                case SortingState.revenue: // revenue
                    while (i < L.Count && j < R.Count)
                    {
                        if (L[i].TotalRevenue >= R[j].TotalRevenue)
                        {
                            list[k] = L[i];
                            i += 1;
                        }
                        else
                        {
                            list[k] = R[j];
                            j += 1;
                        }
                        k += 1;
                    }
                    while (i < L.Count)
                    {
                        list[k] = L[i];
                        i += 1;
                        k += 1;
                    }
                    while (j < R.Count)
                    {
                        list[k] = R[j];
                        j += 1;
                        k += 1;
                    }
                    break;

                case SortingState.capacity: // capacity
                    while (i < L.Count && j < R.Count)
                    {
                        if (L[i].FreeSlots <= R[j].FreeSlots)
                        {
                            list[k] = L[i];
                            i += 1;
                        }
                        else
                        {
                            list[k] = R[j];
                            j += 1;
                        }
                        k += 1;
                    }

                    while (i < L.Count)
                    {
                        list[k] = L[i];
                        i += 1;
                        k += 1;
                    }
                    while (j < R.Count)
                    {
                        list[k] = R[j];
                        j += 1;
                        k += 1;
                    }
                    break;
            }

        }
        return list!;
    }
}
