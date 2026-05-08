namespace UnfathomableParking.Models;

public class ParkingBeach
{
    /// <summary>
    /// The width of the parking beach.
    /// </summary>
    public int Width { get; private set; }

    /// <summary>
    /// The height of the parking beach.
    /// </summary>
    public int Height { get; private set; }

    private ParkingSlot?[,] _slots;

    /// <summary>
    /// Gets the parking slot at the given coordinates.
    /// </summary>
    /// <param name="x">The X position in the parking beach.</param>
    /// <param name="y">The Y position in the parking beach.</param>
    public ParkingSlot? this[int x, int y] => _slots[x, y];

    /// <summary>
    /// Creates a new parking beach with the given width and height.
    /// </summary>
    /// <param name="width">The width of the parking beach.</param>
    /// <param name="height">The height of the parking beach.</param>
    public ParkingBeach(int width, int height)
    {
        Width = width;
        Height = height;
        _slots = new ParkingSlot?[width, height];
    }

    /// <summary>
    /// Resize the parking beach to the given width and height.
    /// </summary>
    /// <param name="width">The new width of the parking beach.</param>
    /// <param name="height">The new height of the parking beach.</param>
    public void Resize(int width, int height)
    {
        var newSlots = new ParkingSlot?[width, height];
        for (var x = 0; x < Math.Min(Width, width); x++)
        {
            for (var y = 0; y < Math.Min(Height, height); y++)
            {
                newSlots[x, y] = _slots[x, y];
            }
        }

        _slots = newSlots;
        Width = width;
        Height = height;
    }

    /// <summary>
    /// Park the given vehicle at the given coordinates.
    /// </summary>
    /// <param name="vehicle"></param>
    /// <param name="x">The X position of the slot to unpark.</param>
    /// <param name="y">The X position of the slot to unpark.</param>
    /// <exception cref="ArgumentOutOfRangeException">The slot does not exist.</exception>
    /// <exception cref="InvalidOperationException">The slot in the position is already occupied.</exception>
    public void ParkVehicle(Vehicle vehicle, int x, int y)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height)
        {
            throw new ArgumentOutOfRangeException(nameof(x), "This slot does not exist.");
        }

        if (_slots[x, y] != null)
        {
            throw new InvalidOperationException("This slot is already occupied.");
        }

        _slots[x, y] = new ParkingSlot(vehicle, DateTime.Now);
    }

    /// <summary>
    /// Unpark the vehicle at the given coordinates.
    /// </summary>
    /// <param name="x">The X position of the slot to unpark.</param>
    /// <param name="y">The Y position of the slot to unpark.</param>
    /// <exception cref="ArgumentOutOfRangeException">The slot does not exist.</exception>
    /// <exception cref="InvalidOperationException">The slot in the position is already unoccupied.</exception>
    public void UnparkVehicle(int x, int y)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height)
        {
            throw new ArgumentOutOfRangeException(nameof(x), "This slot does not exist.");
        }

        if (_slots[x, y] == null)
        {
            throw new InvalidOperationException("This slot is not occupied.");
        }

        // TODO: store unparked time into the history

        _slots[x, y] = null;
    }
}
