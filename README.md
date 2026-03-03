# Galactic Travel Agency

A C# (.NET 10) **ASP.NET Core Web API** for managing interplanetary passengers and flight bookings. Evolved from a single-file script into a layered OOP application with full CRUD, input validation, JSON-based persistence, and an interactive Swagger UI.

## Live Demo

> **[https://galactic-travel-agency.onrender.com](https://galactic-travel-agency.onrender.com)**
>
> Opens directly to Swagger UI — try the endpoints right in your browser. (Free tier may take ~30 s to spin up on first request.)

## Features

- **Passenger management** — add, list, search by name, remove
- **Booking system** — book trips to 5 planets, view, cancel
- **Automatic pricing** — ticket class × planet distance multiplier
- **Input validation** — enforced at the model layer (age range, non-empty name)
- **JSON persistence** — data survives restarts via `data/passengers.json` and `data/bookings.json`
- **Seed data** — 3 passengers + 3 bookings pre-loaded on first run

## Tech Stack

- **Language**: C# with top-level statements
- **Framework**: .NET 10 / ASP.NET Core Web API
- **API Docs**: Swashbuckle (Swagger UI served at `/`)
- **Serialization**: `System.Text.Json` (enums as strings)
- **Deployment**: Docker → Render.com (free tier)

## Project Structure

```
Galactic_Travel_Agency/
├── Models/
│   ├── Enums.cs          # TicketType, Planet enums
│   ├── Passenger.cs      # Passenger class + validation
│   └── Booking.cs        # Booking class + price calculation
├── Services/
│   ├── PassengerService.cs   # CRUD + JSON load/save
│   └── BookingService.cs     # CRUD + JSON load/save
├── data/                 # created at runtime
│   ├── passengers.json
│   └── bookings.json
├── Program.cs            # Web API setup + minimal endpoints
├── Dockerfile            # Multi-stage build for deployment
└── render.yaml           # Render.com service config
```

## Getting Started

```bash
dotnet run --project Galactic_Travel_Agency
# Open http://localhost:8080 → Swagger UI
```

## API Endpoints

| Method   | Path                                | Description                  |
|----------|-------------------------------------|------------------------------|
| `POST`   | `/passengers`                       | Add a new passenger          |
| `GET`    | `/passengers`                       | List all passengers          |
| `GET`    | `/passengers/search?name=`          | Search passengers by name    |
| `GET`    | `/passengers/{id}`                  | Get passenger by ID          |
| `DELETE` | `/passengers/{id}`                  | Remove a passenger           |
| `POST`   | `/bookings`                         | Book a trip                  |
| `GET`    | `/bookings`                         | View all bookings            |
| `GET`    | `/bookings/passenger/{passengerId}` | View bookings by passenger   |
| `DELETE` | `/bookings/{id}`                    | Cancel a booking             |

### Example — Book a trip

```json
POST /bookings
{
  "passengerId": "11111111-1111-1111-1111-111111111111",
  "destination": "Neptune",
  "departureDate": "2026-12-01T00:00:00"
}
```

## Pricing

| Ticket       | Mars  | Venus  | Jupiter | Saturn  | Neptune  |
|--------------|-------|--------|---------|---------|----------|
| Economy      | $500  | $600   | $1,250  | $1,500  | $2,250   |
| Business     | $1,500| $1,800 | $3,750  | $4,500  | $6,750   |
| First Class  | $5,000| $6,000 | $12,500 | $15,000 | $22,500  |

## C# Concepts Demonstrated

Fundamentals from the original script are preserved in `Passenger.cs`:

| Concept | Code |
|---|---|
| Explicit cast | `(double)Age` |
| Implicit cast | `double d = Age` |
| String conversion | `Convert.ToString(Age)` |
| Increment | `Age++` (shown in seed / add flow) |
