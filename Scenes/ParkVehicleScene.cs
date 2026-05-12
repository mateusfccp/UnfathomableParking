using System.Drawing;
using UnfathomableParking.Components;
using UnfathomableParking.Enums;
using UnfathomableParking.Interfaces;
using UnfathomableParking.Models;
using static UnfathomableParking.Services.Engine;

namespace UnfathomableParking.Scenes;

/// <summary>
/// A scene that allows the user to park a vehicle.
/// </summary>
public class ParkVehicleScene : IScene
{
    private const int FocusableCount = 4;
    private int _selectedFieldIndex;
    private readonly TextField _modelTextField;
    private readonly TextField _licensePlateTextField;
    private string LicensePlateNormalized => _licensePlateTextField.Text.Replace("-", "");
    private readonly Button _parkButton;

    private bool _isFormValid;
    private readonly ParkingBeach _parkingBeach;
    private readonly uint _x;
    private readonly uint _y;
    private readonly VehicleBrand? _selectedBrand;

    /// <summary>
    /// Creates a new instance of the <see cref="ParkVehicleScene"/> class.
    /// </summary>
    /// <param name="parkingBeach">The parking beach where the vehicle will be parked.</param>
    /// <param name="x">The x-coordinate of the parking spot.</param>
    /// <param name="y">The y-coordinate of the parking spot.</param>
    /// <param name="selectedBrand">The brand of the vehicle to park.</param>
    /// <param name="model">The model of the vehicle to park.</param>
    /// <param name="licensePlate">The license plate of the vehicle to park.</param>
    public ParkVehicleScene(ParkingBeach parkingBeach,
        uint x,
        uint y,
        VehicleBrand? selectedBrand = null,
        string? model = null,
        string? licensePlate = null)
    {
        _parkingBeach = parkingBeach;
        _x = x;
        _y = y;
        _selectedBrand = selectedBrand;
        _modelTextField = new TextField(model ?? "");
        _licensePlateTextField = new TextField(licensePlate ?? "", "Format: AAA-000", new LicensePlateFormatter());
        _parkButton = new Button("Park vehicle", SubmitForm);
    }

    public void Draw(Canvas canvas)
    {
        canvas.Clear();

        const uint maximumWidth = 40;
        const int height = 18 + 4;

        var width = Math.Min(maximumWidth, canvas.Width);
        var originX = (uint)(canvas.Width / 2 - width / 2);
        var originY = (uint)(canvas.Height / 2 - height / 2);

        var defaultStyle = new Style();
        var selectedStyle = new Style(foregroundColor: Color.DodgerBlue);

        canvas.DrawBox(originX, originY, (uint)width, height);

        canvas.Draw("Brand: ", originX + 2, originY + 2);
        canvas.DrawBox(originX + 1, originY + 3, (uint)width - 2, 3,
            _selectedFieldIndex == 0 ? selectedStyle : defaultStyle);
        var brandStyle = _selectedBrand == null ? new Style(decoration: Decoration.Faint) : new Style();
        canvas.Draw(_selectedBrand?.ToString() ?? "Select brand", originX + 2, originY + 4, brandStyle);
        canvas.Draw("▼", (uint)(originX + 2 + width - 5), originY + 4);

        canvas.Draw("Model: ", originX + 2, originY + 7);
        _modelTextField.Draw(canvas, originX + 1, originY + 8, (uint)width - 2,
            _selectedFieldIndex == 1 ? selectedStyle : defaultStyle);

        canvas.Draw("License plate: ", originX + 2, originY + 12);
        _licensePlateTextField.Draw(canvas, originX + 1, originY + 13, (uint)width - 2,
            _selectedFieldIndex == 2 ? selectedStyle : defaultStyle);

        var buttonWidth = (uint)width / 2;
        _parkButton.Draw(canvas, originX + (uint)width / 2 - buttonWidth / 2, originY + height - 4, buttonWidth,
            _selectedFieldIndex == 3 ? selectedStyle : defaultStyle);
    }

    public void OnKeyPressed(ConsoleKeyInfo keyInfo)
    {
        _selectedFieldIndex = keyInfo.Key switch
        {
            ConsoleKey.UpArrow => Math.Max(0, _selectedFieldIndex - 1),
            ConsoleKey.DownArrow => Math.Min(_selectedFieldIndex + 1, FocusableCount - 1),
            _ => _selectedFieldIndex
        };

        switch (_selectedFieldIndex)
        {
            case 0 when keyInfo.Key == ConsoleKey.Enter:
                var initialIndex = _selectedBrand == null ? 0 : (uint)_selectedBrand;

                Instance?.UpdateScene(new EnumSelectionScene<VehicleBrand>(initialIndex, onSelect: OnSelect));
                break;
            case 1:
                _modelTextField.ProcessKey(keyInfo);
                break;
            case 2:
                _licensePlateTextField.ProcessKey(keyInfo);
                break;
            case 3:
                _parkButton.ProcessKey(keyInfo);
                break;
        }

        ValidateForm();
        return;

        void OnSelect(VehicleBrand brand)
        {
            Instance?.UpdateScene(
                new ParkVehicleScene(_parkingBeach, _x, _y, brand, _modelTextField.Text, _licensePlateTextField.Text)
            );
        }
    }

    private void ValidateForm()
    {
        _isFormValid = _selectedBrand != null &&
                       !string.IsNullOrWhiteSpace(_modelTextField.Text) &&
                       Vehicle.IsLicensePlateValid(LicensePlateNormalized);
    }

    private void SubmitForm()
    {
        if (!_isFormValid) return;
        _parkingBeach.ParkVehicle(new Vehicle(_selectedBrand!.Value, _modelTextField.Text, LicensePlateNormalized),
            _x, _y);
        Instance?.UpdateScene(new ParkingBeachScene(_parkingBeach, _x, _y));
    }

    private class LicensePlateFormatter : IInputFormatter
    {
        public string Format(string current, string next)
        {
            if (next.Length <= current.Length) return next;

            var addedChar = char.ToUpperInvariant(next.Last());

            return current.Length switch
            {
                < 3 when char.IsLetter(addedChar) => current + addedChar,
                3 when addedChar == '-' => current + addedChar,
                3 when char.IsDigit(addedChar) => current + "-" + addedChar,
                3 => current,
                >= 4 and < 7 when char.IsDigit(addedChar) => current + addedChar,
                _ => current
            };
        }
    }
}
