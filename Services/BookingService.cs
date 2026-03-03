using System.Text.Json;
using GalacticTravelAgency.Models;

namespace GalacticTravelAgency.Services;

public class BookingService
{
    private static readonly string DataDir  = Path.Combine(AppContext.BaseDirectory, "data");
    private static readonly string FilePath = Path.Combine(DataDir, "bookings.json");

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        WriteIndented = true,
        Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() },
    };

    private List<Booking> _bookings = [];

    public void Load()
    {
        if (!File.Exists(FilePath)) return;
        string json = File.ReadAllText(FilePath);
        _bookings = JsonSerializer.Deserialize<List<Booking>>(json, JsonOpts) ?? [];
    }

    public void Save()
    {
        Directory.CreateDirectory(DataDir);
        File.WriteAllText(FilePath, JsonSerializer.Serialize(_bookings, JsonOpts));
    }

    public Booking Book(Passenger passenger, Planet destination, DateTime departureDate)
    {
        var booking = Booking.Create(passenger, destination, departureDate);
        _bookings.Add(booking);
        Save();
        return booking;
    }

    public IReadOnlyList<Booking> GetAll() => _bookings.AsReadOnly();

    public IEnumerable<Booking> GetByPassenger(Guid passengerId) =>
        _bookings.Where(b => b.PassengerId == passengerId);

    public bool Cancel(Guid id)
    {
        int removed = _bookings.RemoveAll(b => b.Id == id);
        if (removed > 0) Save();
        return removed > 0;
    }
}
