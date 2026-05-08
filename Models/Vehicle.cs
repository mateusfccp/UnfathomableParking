using System.ComponentModel;
using System.Text.RegularExpressions;
using UnfathomableParking.Enums;

namespace UnfathomableParking.Models;

public sealed class Vehicle
{
    /// <summary>
    ///  The brand of the vehicle.
    /// </summary>
    public VehicleBrand Brand { get; }

    /// <summary>
    /// The model of the vehicle.
    /// </summary>
    public string Model { get; }

    /// <summary>
    /// The vehicle license plate.
    /// </summary>
    public string LicensePlate { get; }

    private const string LicensePlatePattern = @"^[A-Z]{3}[0-9]{3}$";

    public Vehicle(VehicleBrand brand, string model, string licensePlate)
    {
        Brand = brand;
        Model = model;
        LicensePlate = licensePlate;

        if (!Regex.IsMatch(licensePlate, LicensePlatePattern))
        {
            throw new ArgumentException(
                "Invalid license plate format. Lincense plate should follow the pattern AAA000");
        }
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Vehicle other)
        {
            return false;
        }

        return LicensePlate == other.LicensePlate;
    }

    public override int GetHashCode()
    {
        return LicensePlate.GetHashCode();
    }
}
