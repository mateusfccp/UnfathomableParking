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
        const int roadWidth = 3;
        var irregularBlocks = parkingBeach.Width % chunkSize;
        var leftBlock = irregularBlocks / 2;

        var verticalRoadsCount = 1 + parkingBeach.Width / chunkSize;
        var width = 1 + parkingBeach.Width * 2 + verticalRoadsCount * (1 + (uint)roadWidth);

        var horizontalRoadsCount = 1 + (parkingBeach.Height - 1) / 2;
        var boundariesCount = 1 + (parkingBeach.Height + 1) / 2;
        var height = parkingBeach.Height * 2 + horizontalRoadsCount * roadWidth + boundariesCount;

        var position = new Point((int)(canvas.Width / 2 - width / 2), (int)(canvas.Height / 2 - height / 2));

        // Parking beach
        canvas.DrawBox((uint)position.X, (uint)position.Y, width, height);

        var currentRow = 0;
        var maximumRows = 1 + horizontalRoadsCount + parkingBeach.Height + horizontalRoadsCount;
        var endsWithRoad = maximumRows % 2 == 0;
        var maximumColumns = 1 + verticalRoadsCount + parkingBeach.Width;

        var y = 0;

        var parkingSlotX = (uint)0;
        var parkingSlotY = (uint)0;

        while (currentRow < maximumRows)
        {
            var horizontalArea = y == height - 1 ? 0 : int.Abs((currentRow + 2) % 4 - 2);

            var currentColumn = 0;
            var x = 0;

            while (currentColumn < maximumColumns)
            {
                var verticalArea = int.Abs((int)(currentColumn - leftBlock)) % (chunkSize + 1);

                var isLeftBlockArea = currentColumn < leftBlock;
                var isRegularArea = verticalArea > 1;

                var isSlot = isLeftBlockArea || isRegularArea;
                var isAfterRoad = !isLeftBlockArea && verticalArea == 1;
                var isBeforeRoad = x > 0 && verticalArea == 0;
                var isCorner = x == 0 && y == 0 || x == 0 && y == height - 1 || x == width - 1 && y == 0 ||
                               x == width - 1 && y == height - 1;

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

                            parkingSlotX = parkingSlotX + 1;
                        }

                        break;
                }

                if (verticalArea == 0) x = x + (roadWidth - 1);
                x = x + 2;
                currentColumn = currentColumn + 1;
            }

            if (horizontalArea == 0) y = y - 1;
            if (horizontalArea == 2) y = y + roadWidth;
            else y = y + 2;
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
            var carDescription = $"{slot.Vehicle.Brand} {slot.Vehicle.Model}";
            var licensePlate = $"{slot.Vehicle.LicensePlate[..3]}-{slot.Vehicle.LicensePlate[3..]}";
            canvas.Draw(carDescription, (uint)position.X + 1, (uint)position.Y - 2);
            canvas.Draw(licensePlate, (uint)position.X + 1, (uint)position.Y - 1);

            var parkingDate = slot.ParkedAt.ToString("dddd, dd MMMM yyyy HH:mm");
            canvas.Draw($"Parked at: {parkingDate}", (uint)position.X, (uint)position.Y + height + 1);
        }
        else
        {
            canvas.Draw(
                "No vehicle selected.",
                (uint)position.X + 1,
                (uint)position.Y - 2,
                new Style(decoration: Decoration.Faint)
            );
        }

        // Post-processing: Highlight row and column
        var activeSlotX = -1;
        var activeSlotY = -1;
        var simulationRow = 0;
        var simulationY = 0;
        var simulationSlotY = 0;

        while (simulationRow < maximumRows)
        {
            var simulationHorizontalArea = simulationY == height - 1 ? 0 : Math.Abs((simulationRow + 2) % 4 - 2);
            if (simulationHorizontalArea == 0) simulationY = simulationY - 1;

            if (simulationHorizontalArea == 1 && simulationSlotY == CursorPosition.Y)
            {
                activeSlotY = simulationY;
            }

            if (simulationHorizontalArea == 2) simulationY = simulationY + roadWidth;
            else simulationY = simulationY + 2;
            if (simulationHorizontalArea == 1) simulationSlotY = simulationSlotY + 1;
            simulationRow = simulationRow + 1;
        }

        var simulationColumn = 0;
        var simulationX = 0;
        var simulationSlotX = 0;
        while (simulationColumn < maximumColumns)
        {
            var simulationVerticalArea = Math.Abs((int)(simulationColumn - leftBlock)) % (chunkSize + 1);
            if (simulationVerticalArea > 0)
            {
                if (simulationSlotX == CursorPosition.X)
                {
                    activeSlotX = simulationX + 1;
                }

                simulationSlotX = simulationSlotX + 1;
            }

            if (simulationVerticalArea == 0) simulationX = simulationX + (roadWidth - 1);
            simulationX = simulationX + 2;
            simulationColumn = simulationColumn + 1;
        }

        var cursorColor = Color.FromArgb(76, 76, 76);

        // Column highlight
        if (activeSlotX != -1)
        {
            canvas.SetBackground((uint)(position.X + activeSlotX), (uint)position.Y, 1, height, cursorColor);
        }

        // Row highlight
        if (activeSlotY != -1)
        {
            canvas.SetBackground((uint)position.X, (uint)(position.Y + activeSlotY), width, 2, cursorColor);
        }

        // Active cell highlight (2x1)
        if (activeSlotX != -1 && activeSlotY != -1)
        {
            canvas.SetBackground(
                (uint)(position.X + activeSlotX),
                (uint)(position.Y + activeSlotY),
                1, 2,
                Color.FromArgb(140, 140, 140)
            );
        }
    }

    public void OnKeyPressed(ConsoleKeyInfo keyInfo)
    {
        switch (keyInfo.Key)
        {
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

            case ConsoleKey.R:
                ExportReport();
                break;
        }
    }

    private void SelectSlot()
    {
        if (SelectedSlot is { } slot)
        {
            var licensePlate = $"{slot.Vehicle.LicensePlate[..3]}-{slot.Vehicle.LicensePlate[3..]}";
            Engine.Instance?.UpdateScene(
                new ConfirmationScene(
                    title: $"Unpark vehicle {licensePlate}?",
                    onConfirm: () =>
                    {
                        parkingBeach.UnparkVehicle((uint)CursorPosition.X, (uint)CursorPosition.Y);
                        Engine.Instance.UpdateScene(
                            new ParkingBeachScene(parkingBeach, (uint)CursorPosition.X, (uint)CursorPosition.Y)
                        );
                    },
                    onCancel: () =>
                    {
                        Engine.Instance.UpdateScene(
                            new ParkingBeachScene(parkingBeach, (uint)CursorPosition.X, (uint)CursorPosition.Y)
                        );
                    }
                )
            );
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
    
    private void ExportReport()
    {
        //TODO
        string report= $@"
        Ingreso total: ${parkingBeach.TotalRevenue}.
        Espacios ocupados: {parkingBeach.OccupiedSlots}/{parkingBeach.TotalSlots}
         ";
        File.WriteAllText($"Parking_Beach_report{DateTime.Now:yyyyMMdd_HHmmss}.txt",report);
    }
    
}
