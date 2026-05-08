namespace UnfathomableParking.Models;

/// <summary>
/// A parking slot.
/// <br />
/// A parking slot can be occupied by a vehicle and registers the time it was parked and unparked.
/// </summary>
public struct ParkingSlot
{
    /// <summary>
    /// The vehicle that is parked in the slot.
    /// </summary>
    public Vehicle Vehicle { get; }

    /// <summary>
    /// The time the slot was parked.
    /// </summary>
    public DateTime ParkedAt { get; }

    /// <summary>
    /// Creates a new parking slot.
    /// </summary>
    /// <param name="vehicle">The vehicle that is going to park.</param>
    /// <param name="parkedAt">The time the slot was parked.</param>
    public ParkingSlot(Vehicle vehicle, DateTime parkedAt)
    {
        Vehicle = vehicle;
        ParkedAt = parkedAt;
    }
}
