namespace GalacticTravelAgency.Models;

class Booking
{
    // Base prices per ticket type
    private static readonly Dictionary<TicketType, decimal> BasePrices = new()
    {
        [TicketType.Economy]    = 500m,
        [TicketType.Business]   = 1500m,
        [TicketType.FirstClass] = 5000m,
    };

    // Per-planet distance multipliers
    private static readonly Dictionary<Planet, decimal> PlanetMultipliers = new()
    {
        [Planet.Mars]    = 1.0m,
        [Planet.Venus]   = 1.2m,
        [Planet.Jupiter] = 2.5m,
        [Planet.Saturn]  = 3.0m,
        [Planet.Neptune] = 4.5m,
    };

    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid PassengerId { get; init; }
    public string FlightNumber { get; init; } = "";
    public DateTime DepartureDate { get; init; }
    public decimal Price { get; init; }
    public Planet Destination { get; init; }

    public static Booking Create(Passenger passenger, Planet destination, DateTime departureDate)
    {
        decimal price = BasePrices[passenger.TicketType] * PlanetMultipliers[destination];
        string flightNum = $"GT-{destination.ToString()[..2].ToUpper()}-{DateTime.UtcNow:yyyyMMddHHmmss}";

        return new Booking
        {
            PassengerId   = passenger.Id,
            FlightNumber  = flightNum,
            DepartureDate = departureDate,
            Price         = price,
            Destination   = destination,
        };
    }
}
