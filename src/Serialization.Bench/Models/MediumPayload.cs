namespace Serialization.Bench.Models;

public class MediumPayload
{
    public int OrderId { get; set; }
    public string OrderNumber { get; set; }
    public DateTime OrderDate { get; set; }
    public OrderStatus Status { get; set; }
    public Customer Customer { get; set; }
    public List<OrderItem> Items { get; set; }
    public ShippingAddress ShippingAddress { get; set; }
    public decimal SubTotal { get; set; }
    public decimal Tax { get; set; }
    public decimal ShippingCost { get; set; }
    public decimal TotalAmount { get; set; }
    public string Notes { get; set; }
    
    public static MediumPayload CreateSample()
    {
        return new MediumPayload
        {
            OrderId = 98765,
            OrderNumber = "ORD-2024-001234",
            OrderDate = DateTime.UtcNow,
            Status = OrderStatus.Processing,
            Customer = new Customer
            {
                CustomerId = 5001,
                Name = "Jane Smith",
                Email = "jane.smith@example.com",
                Phone = "+1-555-0199",
                LoyaltyPoints = 1250
            },
            Items = Enumerable.Range(1, 15).Select(i => new OrderItem
            {
                ItemId = i,
                ProductName = $"Product {i}",
                SKU = $"SKU-{i:D5}",
                Quantity = i % 3 + 1,
                UnitPrice = 19.99m + i,
                Discount = i % 4 == 0 ? 5.0m : 0,
                TotalPrice = (19.99m + i) * (i % 3 + 1)
            }).ToList(),
            ShippingAddress = new ShippingAddress
            {
                Street = "123 Main Street",
                City = "Springfield",
                State = "IL",
                ZipCode = "62701",
                Country = "USA"
            },
            SubTotal = 450.85m,
            Tax = 40.58m,
            ShippingCost = 12.99m,
            TotalAmount = 504.42m,
            Notes = "Please deliver between 9 AM and 5 PM. Leave package at door if not home."
        };
    }
}

public class Customer
{
    public int CustomerId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public int LoyaltyPoints { get; set; }
}

public class OrderItem
{
    public int ItemId { get; set; }
    public string ProductName { get; set; }
    public string SKU { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }
    public decimal TotalPrice { get; set; }
}

public class ShippingAddress
{
    public string Street { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string ZipCode { get; set; }
    public string Country { get; set; }
}

public enum OrderStatus
{
    Pending,
    Processing,
    Shipped,
    Delivered,
    Cancelled
}