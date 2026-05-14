namespace UnfathomableParking.Models;

public class ParkingBeach
{
    /// <summary>
    /// The width of the parking beach.
    /// </summary>
    public uint Width { get; private set; }

    /// <summary>
    /// The height of the parking beach.
    /// </summary>
    public uint Height { get; private set; }

    /// <summary>
    /// The total of slots in the parking beach.
    /// </summary>
    public uint TotalSlots => Width * Height;

    /// <summary>
    /// The total of occupied slots in the parking beach.
    /// </summary>
    public uint OccupiedSlots
    {
        get
        {
            uint counter = 0;
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (_slots[x, y] != null)
                    {
                        counter = counter + 1;
                    }
                }
            }
            return counter;
        }
    }
    /// <summary>
    /// The total of free slots in the parking beach.
    /// </summary>
    public uint FreeSlots => TotalSlots - OccupiedSlots;

    /// <summary>
    /// The total current revenue of the parking beach.
    /// </summary>
    public decimal TotalRevenue { get; private set; }

    /// <summary>
    /// The revenue per hour of the parking Beach
    /// </summary>
    private decimal RevenuePerHour { get; set; }

    private ParkingSlot?[,] _slots;

    /// <summary>
    /// Gets the parking slot at the given coordinates.
    /// </summary>
    /// <param name="x">The X position in the parking beach.</param>
    /// <param name="y">The Y position in the parking beach.</param>
    public ParkingSlot? this[uint x, uint y] => _slots[x, y];

    /// <summary>
    /// Creates a new parking beach with the given width and height.
    /// </summary>
    /// <param name="width">The width of the parking beach.</param>
    /// <param name="height">The height of the parking beach.</param>
    public ParkingBeach(uint width, uint height, decimal revenuePerHour)
    {
        if (width == 0 || height == 0 || revenuePerHour <= 0)
        {
            if (revenuePerHour <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(revenuePerHour),
                "The revenue per hour cannot be 0 or a negative numbre");
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(width), nameof(height),
                    "The width and height must be greater than 0.");
            }
        }

        Width = width;
        Height = height;
        RevenuePerHour = revenuePerHour;
        _slots = new ParkingSlot?[width, height];
    }

    /// <summary>
    /// Resize the parking beach to the given width and height.
    /// </summary>
    /// <param name="width">The new width of the parking beach.</param>
    /// <param name="height">The new height of the parking beach.</param>
    public void Resize(uint width, uint height)
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
    /// Gets whether the given license plate is parked in the parking beach.
    /// </summary>
    /// <param name="licensePlate">The license plate to check for.</param>
    /// <returns>True if the license plate is parked, false otherwise.</returns>
    public bool IsParked(string licensePlate)
    {
        for (var x = 0; x < Width; x++)
        {
            for (var y = 0; y < Height; y++)
            {
                if (_slots[x, y]?.Vehicle.LicensePlate == licensePlate)
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Park the given vehicle at the given coordinates.
    /// </summary>
    /// <param name="vehicle"></param>
    /// <param name="x">The X position of the slot to unpark.</param>
    /// <param name="y">The X position of the slot to unpark.</param>
    /// <exception cref="ArgumentOutOfRangeException">The slot does not exist.</exception>
    /// <exception cref="InvalidOperationException">The slot in the position is already occupied.</exception>
    public void ParkVehicle(Vehicle vehicle, uint x, uint y)
    {
        if (x >= Width || y >= Height)
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
    public void UnparkVehicle(uint x, uint y)
    {
        if (x >= Width || y >= Height)
        {
            throw new ArgumentOutOfRangeException(nameof(x), "This slot does not exist.");
        }

        var currentSlot = _slots[x, y];
        if (currentSlot != null)
        {
            TimeSpan timeParked = DateTime.Now - currentSlot.Value.ParkedAt;
            decimal chargedHours = (decimal)Math.Ceiling(timeParked.TotalHours);
            TotalRevenue = TotalRevenue + (chargedHours * RevenuePerHour);

            _slots[x, y] = null;
        }
        else throw new InvalidOperationException("This slot is not occupied.");
    }

}
