namespace Serialization.Bench.Models;

public class SmallPayload
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public int Age { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
    public string PhoneNumber { get; set; }
    
    public static SmallPayload CreateSample()
    {
        return new SmallPayload
        {
            Id = 12345,
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Age = 32,
            CreatedAt = DateTime.UtcNow,
            IsActive = true,
            PhoneNumber = "+1-555-0123"
        };
    }
}