using UnfathomableParking.Models;

namespace UnfathomableParking.Services;

/// <summary>
/// A service that manages parking beaches.
/// </summary>
public class ParkingBeachManager
{
    /// <summary>
    /// The list of parking beaches.
    /// </summary>
    public List<ParkingBeach> ParkingBeaches => new(_parkingBeaches);

    private readonly List<ParkingBeach> _parkingBeaches = [];

    /// <summary>
    /// Create a new parking beach with the given width and height.
    /// </summary>
    /// <param name="width">The width of the new parking beach.</param>
    /// <param name="height">The height of the new parking beach.</param>
    void AddParkingBeach(uint width, uint height,decimal costPerHour)
    {
        var parkingBeach = new ParkingBeach(width, height,costPerHour);
        _parkingBeaches.Add(parkingBeach);
    }

    /// <summary>
    /// Resize the parking beach at the given index to the given width and height.
    /// </summary>
    /// <param name="index">The index of the parking beach.</param>
    /// <param name="width">The new width of the parking beach.</param>
    /// <param name="height">The new height of the parking beach.</param>
    void ResizeParkingBeach(int index, uint width, uint height)
    {
        _parkingBeaches[index].Resize(width, height);
    }

    /// <summary>
    /// Remove the parking beach at the given index.
    /// </summary>
    /// <param name="index">The index of the parking beach to remove.</param>
    void RemoveParkingBeach(int index)
    {
        _parkingBeaches.RemoveAt(index);
    }
}
