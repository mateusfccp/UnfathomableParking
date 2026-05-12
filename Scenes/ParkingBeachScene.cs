using System.Drawing;
using UnfathomableParking.Enums;
using UnfathomableParking.Interfaces;
using UnfathomableParking.Models;
using UnfathomableParking.Services;

namespace UnfathomableParking.Scenes;

/// <summary>
/// A scene that displays a parking beach.
/// <br />
/// The user can navigate the parking beach using the arrow keys and select a vehicle to 
/// </summary>
/// <param name="parkingBeach"></param>
public class ParkingBeachScene(ParkingBeach parkingBeach, uint cursorX = 0, uint cursorY = 0) : IScene
{
    private Point CursorPosition { get; set; } = new((int)cursorX, (int)cursorY);

    private ParkingSlot? SelectedSlot => parkingBeach[(uint)CursorPosition.X, (uint)CursorPosition.Y];

    public void Draw(Engine.Canvas canvas)
    {
        canvas.Clear();

        // Calculations
        const uint chunkSize = 8;
        var irregularBlocks = parkingBeach.Width % chunkSize;
        var leftBlock = irregularBlocks / 2;

        var verticalRoadsCount = 1 + parkingBeach.Width / chunkSize;
        var width = 1 + parkingBeach.Width * 2 + verticalRoadsCount * 3;

        var horizontalRoadsCount = 1 + (parkingBeach.Height - 1) / 2;
        var internalHeight = 2 + (parkingBeach.Height - 1) + (parkingBeach.Height - 1) / 2;
        var height = internalHeight * 2 + (internalHeight - 1) / 3 + 2;

        var position = new Point((int)(canvas.Width / 2 - width / 2), (int)(canvas.Height / 2 - height / 2));

        // Parking beach
        canvas.DrawBox((uint)position.X, (uint)position.Y, width, height);

        var currentRow = 0;
        var maximumRows = 1 + horizontalRoadsCount + parkingBeach.Height + horizontalRoadsCount;
        var endsWithRoad = maximumRows % 2 == 0;

        var y = 0;

        var parkingSlotX = (uint)0;
        var parkingSlotY = (uint)0;

        while (currentRow < maximumRows)
        {
            var horizontalArea = y == height - 1 ? 0 : int.Abs((currentRow + 2) % 4 - 2);
            // canvas.Draw($"{horizontalArea}", position.X - 2, position.Y + y);

            var currentColumn = 0;
            var x = 0;
            var maximumColumns = 1 + verticalRoadsCount + parkingBeach.Width;

            while (currentColumn < maximumColumns)
            {
                var verticalArea = int.Abs((int)(currentColumn - leftBlock)) % (chunkSize + 1);

                var isLeftBlockArea = currentColumn < leftBlock;
                var isRegularArea = verticalArea > 1;

                var isSlot = isLeftBlockArea || isRegularArea;
                var isAfterRoad = verticalArea == 1;
                var isBeforeRoad = x > 0 && verticalArea == 0;
                var isCorner = x == 0 && y == 0 || x == 0 && y == height - 1 || x == width - 1 && y == 0 ||
                               x == width - 1 && y == height - 1;

                // canvas.Draw($"{verticalArea}", position.X + x, (int)(position.Y + height + 1));

                switch (horizontalArea)
                {
                    // Horizontal line (0)
                    case 0 when !isCorner:
                    {
                        if (y == 0)
                        {
                            canvas.Draw("┬", (uint)(position.X + x), (uint)(position.Y + y));
                        }
                        else if (y == height - 1)
                        {
                            var character = endsWithRoad ? "─" : "┴";

                            canvas.Draw(character, (uint)(position.X + x), (uint)(position.Y + y));
                        }
                        else if (isAfterRoad && x != width - 1)
                        {
                            canvas.Draw("├─", (uint)(position.X + x), (uint)(position.Y + y));
                        }
                        else if (isSlot && x != width - 1)
                        {
                            var character = x == 0 ? "├─" : "┼─";

                            canvas.Draw(character, (uint)(position.X + x), (uint)(position.Y + y));
                        }
                        else if (isBeforeRoad || (x == width - 1 && verticalArea != 1))
                        {
                            canvas.Draw("┤", (uint)(position.X + x), (uint)(position.Y + y));
                        }

                        break;
                    }
                    // Slots (1)
                    case 1 when x < width - 1:
                        canvas.Draw("│\n│", (uint)(position.X + x), (uint)(position.Y + y));
                        if (verticalArea > 0)
                        {
                            if (CursorPosition.X == parkingSlotX && CursorPosition.Y == parkingSlotY)
                            {
                                canvas.DefaultStyle = new Style(backgroundColor: Color.White);
                            }

                            if (x != width - 1 && parkingBeach[parkingSlotX, parkingSlotY] is { } currentSlot)
                            {
                                canvas.Draw(
                                    "█\n█",
                                    (uint)(position.X + x + 1),
                                    (uint)(position.Y + y),
                                    canvas.DefaultStyle with { ForegroundColor = GetVehicleColor(currentSlot.Vehicle) }
                                );
                            }
                            else
                            {
                                canvas.Draw(
                                    " \n ",
                                    (uint)(position.X + x + 1),
                                    (uint)(position.Y + y)
                                );
                            }

                            if (CursorPosition.X == parkingSlotX && CursorPosition.Y == parkingSlotY)
                            {
                                canvas.DefaultStyle = new();
                            }

                            parkingSlotX = parkingSlotX + 1;
                        }

                        break;
                }

                if (verticalArea == 0) x = x + 1;
                x = x + 2;
                currentColumn = currentColumn + 1;
            }

            if (horizontalArea == 0) y = y - 1;
            y = y + 2;
            currentRow = currentRow + 1;

            if (horizontalArea == 1)
            {
                parkingSlotX = 0;
                parkingSlotY = parkingSlotY + 1;
            }
        }

        // Infos
        if (SelectedSlot is { } slot)
        {
            canvas.Draw($"Brand: {slot.Vehicle.Brand}", (uint)position.X + 1, (uint)position.Y - 3);
            canvas.Draw($"Model: {slot.Vehicle.Model}", (uint)position.X + 1, (uint)position.Y - 2);
            canvas.Draw($"License Plate: {slot.Vehicle.LicensePlate}", (uint)position.X + 1,
                (uint)position.Y - 1);
        }
        else
        {
            canvas.Draw("No vehicle selected.", (uint)position.X + 1, (uint)position.Y - 3,
                new Style(decoration: Decoration.Faint));
        }
    }

    public void OnKeyPressed(ConsoleKeyInfo keyInfo)
    {
        switch (keyInfo.Key)
        {
            case ConsoleKey.N:
                parkingBeach.Resize(parkingBeach.Width, parkingBeach.Height + 1);
                break;
            case ConsoleKey.P:
                parkingBeach.Resize(parkingBeach.Width, parkingBeach.Height - 1);
                break;
            case ConsoleKey.F:
                parkingBeach.Resize(parkingBeach.Width + 1, parkingBeach.Height);
                break;
            case ConsoleKey.B:
                parkingBeach.Resize(parkingBeach.Width - 1, parkingBeach.Height);
                break;
            case ConsoleKey.UpArrow:
                var y = (CursorPosition.Y - 1 + (int)parkingBeach.Height) % (int)parkingBeach.Height;
                CursorPosition = CursorPosition with { Y = y };
                break;
            case ConsoleKey.DownArrow:
                var y2 = (CursorPosition.Y + 1) % (int)parkingBeach.Height;
                CursorPosition = CursorPosition with { Y = y2 };
                break;
            case ConsoleKey.LeftArrow:
                var x = (CursorPosition.X - 1 + (int)parkingBeach.Width) % (int)parkingBeach.Width;
                CursorPosition = CursorPosition with { X = x };
                break;
            case ConsoleKey.RightArrow:
                var x2 = (CursorPosition.X + 1) % (int)parkingBeach.Width;
                CursorPosition = CursorPosition with { X = x2 };
                break;
            case ConsoleKey.Enter:
                SelectSlot();
                break;
        }
    }

    private void SelectSlot()
    {
        if (SelectedSlot is { } slot)
        {
            parkingBeach.UnparkVehicle((uint)CursorPosition.X, (uint)CursorPosition.Y);
        }
        else
        {
            Engine.Instance?.UpdateScene(
                new ParkVehicleScene(parkingBeach, (uint)CursorPosition.X, (uint)CursorPosition.Y)
            );
        }
    }

    private static Color GetVehicleColor(Vehicle vehicle)
    {
        List<Color> colors =
        [
            Color.Firebrick,
            Color.DodgerBlue,
            Color.White,
            Color.DarkGray,
            Color.MediumSpringGreen,
        ];

        if (string.IsNullOrEmpty(vehicle.LicensePlate)) return colors[0];

        // FNV-1a algorithm to generate unique deterministic hash based on the license plate
        var hash = 2166136261;
        foreach (var c in vehicle.LicensePlate)
        {
            hash = (hash ^ c) * 16777619;
        }

        var index = (int)(hash % (uint)colors.Count);

        return colors[index];
    }
}
