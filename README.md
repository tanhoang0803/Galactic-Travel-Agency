# Galactic Travel Agency

A C# (.NET 10) console application for managing interplanetary passengers and flight bookings. Evolved from a single-file script into a layered OOP application with full CRUD, input validation, and JSON-based persistence.

## Features

- **Passenger management** — add, list, search by name, remove
- **Booking system** — book trips to 5 planets, view, cancel
- **Automatic pricing** — ticket class × planet distance multiplier
- **Input validation** — retry loops for age range, date format, enum selection
- **JSON persistence** — data survives restarts via `data/passengers.json` and `data/bookings.json`

## Tech Stack

- **Language**: C# with top-level statements
- **Framework**: .NET 10
- **Serialization**: `System.Text.Json` (built-in, no extra packages)
- **Project type**: Console Application

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
└── Program.cs            # Interactive menu loop
```

## Getting Started

```bash
dotnet run --project Galactic_Travel_Agency
```

## Menu

```
=== Galactic Travel Agency ===
1. Add passenger
2. List all passengers
3. Search passenger by name
4. Remove passenger
5. Book a trip
6. View all bookings
7. View bookings by passenger
8. Cancel booking
0. Exit
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
| Increment | `Age++` shown live when adding a passenger |
