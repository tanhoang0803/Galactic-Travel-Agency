// ─── Galactic Travel Agency — ASP.NET Core Web API ───────────────────────────
// Original C# concepts preserved in Passenger model layer:
//   • Explicit conversion : (double)Age  → AgeAsDouble property
//   • Implicit conversion : double d = Age → AgeImplicit property
//   • Convert.ToString    : Convert.ToString(Age) → AgeAsString property
// ──────────────────────────────────────────────────────────────────────────────

using System.Text.Json.Serialization;
using GalacticTravelAgency.Models;
using GalacticTravelAgency.Services;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// ── DI ────────────────────────────────────────────────────────────────────────
builder.Services.AddSingleton<PassengerService>();
builder.Services.AddSingleton<BookingService>();

// ── JSON: enums as strings ────────────────────────────────────────────────────
builder.Services.ConfigureHttpJsonOptions(opts =>
    opts.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// ── Swagger / OpenAPI ─────────────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Galactic Travel Agency", Version = "v1" });
});

var app = builder.Build();

// ── Persist / seed on startup ─────────────────────────────────────────────────
var passengerSvc = app.Services.GetRequiredService<PassengerService>();
var bookingSvc   = app.Services.GetRequiredService<BookingService>();
passengerSvc.Load();
bookingSvc.Load();
SeedIfEmpty(passengerSvc, bookingSvc);

// ── Swagger at root ───────────────────────────────────────────────────────────
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Galactic Travel Agency v1");
    c.RoutePrefix = string.Empty; // serve at "/"
});

// ── Endpoints ─────────────────────────────────────────────────────────────────

// POST /passengers — add a new passenger
app.MapPost("/passengers", (CreatePassengerRequest req, PassengerService svc) =>
{
    var passenger = new Passenger
    {
        Name            = req.Name,
        Age             = req.Age,
        TicketType      = req.TicketType,
        PreferredPlanet = req.PreferredPlanet,
    };
    var (ok, error) = svc.Add(passenger);
    return ok
        ? Results.Created($"/passengers/{passenger.Id}", passenger)
        : Results.BadRequest(new { error });
})
.WithName("CreatePassenger")
.WithTags("Passengers");

// GET /passengers — list all passengers
app.MapGet("/passengers", (PassengerService svc) => svc.GetAll())
   .WithName("ListPassengers")
   .WithTags("Passengers");

// GET /passengers/search?name= — search by name
app.MapGet("/passengers/search", ([FromQuery] string name, PassengerService svc) =>
    svc.FindByName(name))
   .WithName("SearchPassengers")
   .WithTags("Passengers");

// GET /passengers/{id} — get one passenger by ID
app.MapGet("/passengers/{id:guid}", (Guid id, PassengerService svc) =>
{
    var p = svc.GetById(id);
    return p is not null ? Results.Ok(p) : Results.NotFound();
})
.WithName("GetPassenger")
.WithTags("Passengers");

// DELETE /passengers/{id} — remove a passenger
app.MapDelete("/passengers/{id:guid}", (Guid id, PassengerService svc) =>
{
    bool removed = svc.Remove(id);
    return removed ? Results.Ok(new { message = "Passenger removed." }) : Results.NotFound();
})
.WithName("RemovePassenger")
.WithTags("Passengers");

// POST /bookings — book a trip
app.MapPost("/bookings", (CreateBookingRequest req, PassengerService passSvc, BookingService bookSvc) =>
{
    var passenger = passSvc.GetById(req.PassengerId);
    if (passenger is null)
        return Results.NotFound(new { error = "Passenger not found." });

    var booking = bookSvc.Book(passenger, req.Destination, req.DepartureDate);
    return Results.Created($"/bookings/{booking.Id}", booking);
})
.WithName("CreateBooking")
.WithTags("Bookings");

// GET /bookings — list all bookings
app.MapGet("/bookings", (BookingService svc) => svc.GetAll())
   .WithName("ListBookings")
   .WithTags("Bookings");

// GET /bookings/passenger/{passengerId} — bookings for a specific passenger
app.MapGet("/bookings/passenger/{passengerId:guid}", (Guid passengerId, BookingService svc) =>
    svc.GetByPassenger(passengerId))
   .WithName("GetBookingsByPassenger")
   .WithTags("Bookings");

// DELETE /bookings/{id} — cancel a booking
app.MapDelete("/bookings/{id:guid}", (Guid id, BookingService svc) =>
{
    bool cancelled = svc.Cancel(id);
    return cancelled ? Results.Ok(new { message = "Booking cancelled." }) : Results.NotFound();
})
.WithName("CancelBooking")
.WithTags("Bookings");

// ── Start ─────────────────────────────────────────────────────────────────────
string port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Run($"http://+:{port}");

// ── Seed ─────────────────────────────────────────────────────────────────────
static void SeedIfEmpty(PassengerService passSvc, BookingService bookSvc)
{
    if (passSvc.GetAll().Count > 0) return;

    var zara = new Passenger
    {
        Id              = new Guid("11111111-1111-1111-1111-111111111111"),
        Name            = "Zara Voss",
        Age             = 34,
        TicketType      = TicketType.FirstClass,
        PreferredPlanet = Planet.Neptune,
    };
    var dex = new Passenger
    {
        Id              = new Guid("22222222-2222-2222-2222-222222222222"),
        Name            = "Dex Kowalski",
        Age             = 27,
        TicketType      = TicketType.Economy,
        PreferredPlanet = Planet.Mars,
    };
    var mira = new Passenger
    {
        Id              = new Guid("33333333-3333-3333-3333-333333333333"),
        Name            = "Mira Solano",
        Age             = 41,
        TicketType      = TicketType.Business,
        PreferredPlanet = Planet.Jupiter,
    };

    passSvc.Add(zara);
    passSvc.Add(dex);
    passSvc.Add(mira);

    bookSvc.Book(zara,  Planet.Neptune, new DateTime(2026, 7, 1));
    bookSvc.Book(dex,   Planet.Mars,    new DateTime(2026, 8, 15));
    bookSvc.Book(mira,  Planet.Jupiter, new DateTime(2026, 9, 30));
}

// ── Request DTOs ─────────────────────────────────────────────────────────────
record CreatePassengerRequest(string Name, int Age, TicketType TicketType, Planet PreferredPlanet);
record CreateBookingRequest(Guid PassengerId, Planet Destination, DateTime DepartureDate);
