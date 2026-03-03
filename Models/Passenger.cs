namespace GalacticTravelAgency.Models;

public class Passenger
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; set; } = "";
    public int Age { get; set; }
    public TicketType TicketType { get; set; }
    public Planet PreferredPlanet { get; set; }

    // Preserve original concepts: explicit cast, implicit conversion, Convert.ToString
    public double AgeAsDouble => (double)Age;           // explicit conversion
    public double AgeImplicit { get { double d = Age; return d; } } // implicit conversion
    public string AgeAsString => Convert.ToString(Age); // Convert.ToString

    public static (bool valid, string error) Validate(string name, int age)
    {
        if (string.IsNullOrWhiteSpace(name))
            return (false, "Name cannot be empty.");
        if (age < 1 || age > 120)
            return (false, "Age must be between 1 and 120.");
        return (true, "");
    }
}
