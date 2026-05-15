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
    public List<ParkingBeach> ParkingBeaches => [.._parkingBeaches];

    private readonly List<ParkingBeach> _parkingBeaches = [];

    /// <summary>
    /// Create a new parking beach with the given width and height.
    /// </summary>
    /// <param name="width">The width of the new parking beach.</param>
    /// <param name="height">The height of the new parking beach.</param>
    public void AddParkingBeach(string name, uint width, uint height, decimal costPerHour)
    {
        var parkingBeach = new ParkingBeach(name, width, height, costPerHour);
        _parkingBeaches.Add(parkingBeach);
    }

    /// <summary>
    /// Resize the parking beach at the given index to the given width and height.
    /// </summary>
    /// <param name="index">The index of the parking beach.</param>
    /// <param name="width">The new width of the parking beach.</param>
    /// <param name="height">The new height of the parking beach.</param>
    public void EditParkingBeach(string oldName, string newName, uint width, uint height)
    {
        var editedBeach = _parkingBeaches.FirstOrDefault(a => a.Name == oldName);
        if (editedBeach != null)
        {
            editedBeach.Rename(newName);
            editedBeach.Resize(width, height);
        }

    }

    /// <summary>
    /// Remove the parking beach at the given index.
    /// </summary>
    /// <param name="index">The index of the parking beach to remove.</param>
    public void RemoveParkingBeach(string name)
    {
        var removedBeach = _parkingBeaches.FirstOrDefault(a => a.Name == name);
        if (removedBeach != null)
        {
            _parkingBeaches.Remove(removedBeach);
        }
    }


    public List<ParkingBeach> GetParkingBeaches()
    {
        return ParkingBeaches;
    }
}
