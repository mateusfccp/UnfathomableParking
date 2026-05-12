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

    private const string LicensePlatePattern = "^[A-Z]{3}[0-9]{3}$";

    public Vehicle(VehicleBrand brand, string model, string licensePlate)
    {
        Brand = brand;
        Model = model;
        LicensePlate = licensePlate;

        if (!IsLicensePlateValid(licensePlate))
        {
            throw new ArgumentException(
                "Invalid license plate format. License plate should follow the pattern AAA000");
        }
    }

    /// <summary>
    /// Validate whether the given license plate is valid.
    /// </summary>
    /// <param name="licensePlate">The license plate to be validated.</param>
    /// <returns>Whether the given license plate is valid.</returns>
    public static bool IsLicensePlateValid(string licensePlate)
    {
        return Regex.IsMatch(licensePlate, LicensePlatePattern);
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
