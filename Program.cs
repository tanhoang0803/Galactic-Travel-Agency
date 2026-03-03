// ─── Galactic Travel Agency ───────────────────────────────────────────────────
// Original concepts preserved in Passenger model layer:
//   • Variables: Name, Age, TicketType, PreferredPlanet
//   • Increment: AgeAsDouble uses explicit cast; the increment demo runs below
//   • Explicit conversion: (double)Age  → AgeAsDouble property
//   • Implicit conversion: double d = Age → AgeImplicit property
//   • Convert.ToString: Convert.ToString(Age) → AgeAsString property
// ──────────────────────────────────────────────────────────────────────────────

using GalacticTravelAgency.Models;
using GalacticTravelAgency.Services;

var passengerSvc = new PassengerService();
var bookingSvc   = new BookingService();
passengerSvc.Load();
bookingSvc.Load();

while (true)
{
    Console.WriteLine();
    Console.WriteLine("=== Galactic Travel Agency ===");
    Console.WriteLine("1. Add passenger");
    Console.WriteLine("2. List all passengers");
    Console.WriteLine("3. Search passenger by name");
    Console.WriteLine("4. Remove passenger");
    Console.WriteLine("5. Book a trip");
    Console.WriteLine("6. View all bookings");
    Console.WriteLine("7. View bookings by passenger");
    Console.WriteLine("8. Cancel booking");
    Console.WriteLine("0. Exit");
    Console.Write("> ");

    string choice = Console.ReadLine()?.Trim() ?? "";

    switch (choice)
    {
        case "1": AddPassenger();          break;
        case "2": ListPassengers();        break;
        case "3": SearchPassenger();       break;
        case "4": RemovePassenger();       break;
        case "5": BookTrip();              break;
        case "6": ViewAllBookings();       break;
        case "7": ViewBookingsByPassenger(); break;
        case "8": CancelBooking();         break;
        case "0":
            Console.WriteLine("Safe travels!");
            return;
        default:
            Console.WriteLine("Unknown option.");
            break;
    }
}

// ── Helpers ───────────────────────────────────────────────────────────────────

void AddPassenger()
{
    string name = PromptNonEmpty("Passenger name");

    int age = PromptInt("Age", min: 1, max: 120);

    TicketType ticket = PromptEnum<TicketType>("Ticket type");
    Planet planet     = PromptEnum<Planet>("Preferred planet");

    var p = new Passenger
    {
        Name            = name,
        Age             = age,
        TicketType      = ticket,
        PreferredPlanet = planet,
    };

    // Demo original increment concept
    int ageBeforeIncrement = p.Age;
    p.Age++;
    Console.WriteLine($"  (Original age: {ageBeforeIncrement} → after increment: {p.Age})");
    Console.WriteLine($"  Explicit cast to double : {p.AgeAsDouble}");
    Console.WriteLine($"  Implicit cast to double : {p.AgeImplicit}");
    Console.WriteLine($"  Convert.ToString(Age)   : {p.AgeAsString}");
    p.Age--;  // restore to what was entered

    var (ok, error) = passengerSvc.Add(p);
    Console.WriteLine(ok ? $"Added passenger {p.Name} (ID: {p.Id})" : $"Error: {error}");
}

void ListPassengers()
{
    var list = passengerSvc.GetAll();
    if (list.Count == 0) { Console.WriteLine("No passengers."); return; }
    Console.WriteLine();
    foreach (var p in list)
        PrintPassenger(p);
}

void SearchPassenger()
{
    string term = PromptNonEmpty("Search name");
    var results = passengerSvc.FindByName(term).ToList();
    if (results.Count == 0) { Console.WriteLine("No matches."); return; }
    foreach (var p in results)
        PrintPassenger(p);
}

void RemovePassenger()
{
    Guid id = PromptGuid("Passenger ID to remove");
    bool ok = passengerSvc.Remove(id);
    Console.WriteLine(ok ? "Passenger removed." : "Passenger not found.");
}

void BookTrip()
{
    Guid id = PromptGuid("Passenger ID");
    var passenger = passengerSvc.GetById(id);
    if (passenger is null) { Console.WriteLine("Passenger not found."); return; }

    Planet destination = PromptEnum<Planet>("Destination planet");
    DateTime departure  = PromptDate("Departure date (yyyy-MM-dd)");

    var booking = bookingSvc.Book(passenger, destination, departure);
    Console.WriteLine($"Booked! Flight {booking.FlightNumber} on {booking.DepartureDate:yyyy-MM-dd} — Price: {booking.Price:C}");
}

void ViewAllBookings()
{
    var list = bookingSvc.GetAll();
    if (list.Count == 0) { Console.WriteLine("No bookings."); return; }
    Console.WriteLine();
    foreach (var b in list)
        PrintBooking(b);
}

void ViewBookingsByPassenger()
{
    Guid id = PromptGuid("Passenger ID");
    var passenger = passengerSvc.GetById(id);
    if (passenger is null) { Console.WriteLine("Passenger not found."); return; }

    var results = bookingSvc.GetByPassenger(id).ToList();
    if (results.Count == 0) { Console.WriteLine("No bookings for this passenger."); return; }
    foreach (var b in results)
        PrintBooking(b);
}

void CancelBooking()
{
    Guid id = PromptGuid("Booking ID to cancel");
    bool ok = bookingSvc.Cancel(id);
    Console.WriteLine(ok ? "Booking cancelled." : "Booking not found.");
}

// ── Display helpers ───────────────────────────────────────────────────────────

void PrintPassenger(Passenger p)
{
    Console.WriteLine($"  [{p.Id}] {p.Name}, Age {p.Age}, {p.TicketType}, Prefers: {p.PreferredPlanet}");
}

void PrintBooking(Booking b)
{
    var passenger = passengerSvc.GetById(b.PassengerId);
    string name = passenger?.Name ?? b.PassengerId.ToString();
    Console.WriteLine($"  [{b.Id}] {b.FlightNumber} | {name} → {b.Destination} | {b.DepartureDate:yyyy-MM-dd} | {b.Price:C}");
}

// ── Input helpers ─────────────────────────────────────────────────────────────

string PromptNonEmpty(string label)
{
    while (true)
    {
        Console.Write($"{label}: ");
        string value = Console.ReadLine()?.Trim() ?? "";
        if (!string.IsNullOrWhiteSpace(value)) return value;
        Console.WriteLine("  Value cannot be empty. Try again.");
    }
}

int PromptInt(string label, int min, int max)
{
    while (true)
    {
        Console.Write($"{label} ({min}–{max}): ");
        string raw = Console.ReadLine()?.Trim() ?? "";
        if (int.TryParse(raw, out int value) && value >= min && value <= max)
            return value;
        Console.WriteLine($"  Must be an integer between {min} and {max}. Try again.");
    }
}

T PromptEnum<T>(string label) where T : struct, Enum
{
    string[] names = Enum.GetNames<T>();
    while (true)
    {
        Console.WriteLine($"{label}:");
        for (int i = 0; i < names.Length; i++)
            Console.WriteLine($"  {i + 1}. {names[i]}");
        Console.Write("> ");
        string raw = Console.ReadLine()?.Trim() ?? "";
        if (int.TryParse(raw, out int idx) && idx >= 1 && idx <= names.Length)
            return Enum.Parse<T>(names[idx - 1]);
        Console.WriteLine("  Invalid selection. Try again.");
    }
}

Guid PromptGuid(string label)
{
    while (true)
    {
        Console.Write($"{label}: ");
        string raw = Console.ReadLine()?.Trim() ?? "";
        if (Guid.TryParse(raw, out Guid id)) return id;
        Console.WriteLine("  Invalid ID format. Try again.");
    }
}

DateTime PromptDate(string label)
{
    while (true)
    {
        Console.Write($"{label}: ");
        string raw = Console.ReadLine()?.Trim() ?? "";
        if (DateTime.TryParseExact(raw, "yyyy-MM-dd",
            System.Globalization.CultureInfo.InvariantCulture,
            System.Globalization.DateTimeStyles.None, out DateTime dt))
            return dt;
        Console.WriteLine("  Invalid date. Use format yyyy-MM-dd. Try again.");
    }
}
