# Galactic Travel Agency — Claude Instructions

## Project Overview
A C# console application (.NET 10) that manages passengers and bookings for a fictional Galactic Travel Agency. Evolved from a toy script into a proper OOP application with CRUD operations, input validation, and JSON persistence.

## Project Structure
```
Galactic_Travel_Agency/
├── Models/
│   ├── Enums.cs          # TicketType, Planet enums
│   ├── Passenger.cs      # Passenger class with validation
│   └── Booking.cs        # Booking class with price calculation
├── Services/
│   ├── PassengerService.cs   # Passenger CRUD + JSON persistence
│   └── BookingService.cs     # Booking CRUD + JSON persistence
├── data/                 # auto-created at runtime (gitignored)
│   ├── passengers.json
│   └── bookings.json
├── Program.cs            # Top-level menu loop + input helpers
├── CLAUDE.md
└── Galactic_Travel_Agency.csproj
```

## Coding Conventions
- Top-level statements in `Program.cs` — no explicit `Main` method
- Namespace: `GalacticTravelAgency.Models` / `GalacticTravelAgency.Services`
- Class naming: PascalCase; variable/parameter naming: camelCase
- Implicit usings enabled — no need to add `using System;`
- Nullable reference types enabled
- No NuGet packages — only BCL (`System.Text.Json` built into .NET 10)

## Architecture Rules
- **Models** hold data and validation only — no I/O
- **Services** own persistence (`Load()` / `Save()`) and business logic
- **Program.cs** handles all console I/O via local helper functions (`PromptNonEmpty`, `PromptInt`, `PromptEnum<T>`, `PromptGuid`, `PromptDate`)
- JSON files are written to `AppContext.BaseDirectory/data/` (next to the compiled binary)

## Original Concepts Preserved (in `Passenger.cs`)
These C# fundamentals from the original script are kept alive as computed properties:
- `AgeAsDouble` — explicit cast: `(double)Age`
- `AgeImplicit` — implicit cast: `double d = Age`
- `AgeAsString` — `Convert.ToString(Age)`
- Age increment is demonstrated live in the "Add passenger" flow

## Enums
```
TicketType : Economy | Business | FirstClass
Planet     : Mars | Venus | Jupiter | Saturn | Neptune
```

## Pricing Model
Base price × planet multiplier:
- Economy $500 / Business $1500 / FirstClass $5000
- Mars ×1.0 / Venus ×1.2 / Jupiter ×2.5 / Saturn ×3.0 / Neptune ×4.5

## Running the Project
```bash
dotnet run --project Galactic_Travel_Agency
```

## Menu Options
```
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
