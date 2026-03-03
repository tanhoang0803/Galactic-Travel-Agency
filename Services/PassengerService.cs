using System.Text.Json;
using GalacticTravelAgency.Models;

namespace GalacticTravelAgency.Services;

public class PassengerService
{
    private static readonly string DataDir  = Path.Combine(AppContext.BaseDirectory, "data");
    private static readonly string FilePath = Path.Combine(DataDir, "passengers.json");

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        WriteIndented = true,
        Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() },
    };

    private List<Passenger> _passengers = [];

    public void Load()
    {
        if (!File.Exists(FilePath)) return;
        string json = File.ReadAllText(FilePath);
        _passengers = JsonSerializer.Deserialize<List<Passenger>>(json, JsonOpts) ?? [];
    }

    public void Save()
    {
        Directory.CreateDirectory(DataDir);
        File.WriteAllText(FilePath, JsonSerializer.Serialize(_passengers, JsonOpts));
    }

    public (bool ok, string error) Add(Passenger p)
    {
        var (valid, error) = Passenger.Validate(p.Name, p.Age);
        if (!valid) return (false, error);
        _passengers.Add(p);
        Save();
        return (true, "");
    }

    public IReadOnlyList<Passenger> GetAll() => _passengers.AsReadOnly();

    public IEnumerable<Passenger> FindByName(string name) =>
        _passengers.Where(p => p.Name.Contains(name, StringComparison.OrdinalIgnoreCase));

    public bool Remove(Guid id)
    {
        int removed = _passengers.RemoveAll(p => p.Id == id);
        if (removed > 0) Save();
        return removed > 0;
    }

    public Passenger? GetById(Guid id) => _passengers.FirstOrDefault(p => p.Id == id);
}
