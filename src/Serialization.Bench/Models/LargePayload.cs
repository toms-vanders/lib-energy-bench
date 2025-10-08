namespace Serialization.Bench.Models;

public class LargePayload
{
    public int CatalogId { get; set; }
    public string CatalogName { get; set; }
    public DateTime GeneratedAt { get; set; }
    public List<Product> Products { get; set; }
    public CatalogMetadata Metadata { get; set; }
    
    public static LargePayload CreateSample()
    {
        return new LargePayload
        {
            CatalogId = 2024,
            CatalogName = "Electronics Catalog Q1 2024",
            GeneratedAt = DateTime.UtcNow,
            Products = Enumerable.Range(1, 50).Select(i => new Product
            {
                ProductId = i,
                Name = $"Product {i}",
                Description = $"This is a detailed description for product {i}. It includes multiple features, specifications, and benefits that make it an excellent choice for customers. The product has been designed with quality and performance in mind.",
                Category = i % 4 == 0 ? "Electronics" : i % 4 == 1 ? "Accessories" : i % 4 == 2 ? "Software" : "Hardware",
                Price = 99.99m + (i * 10),
                StockQuantity = 100 + i,
                Manufacturer = $"Manufacturer {i % 10}",
                SKU = $"PROD-{i:D6}",
                Tags = new List<string> { $"tag{i}", $"category{i % 5}", "featured" },
                Specifications = new Dictionary<string, string>
                {
                    { "Weight", $"{i * 0.5}kg" },
                    { "Dimensions", $"{i}x{i}x{i}cm" },
                    { "Color", i % 2 == 0 ? "Black" : "Silver" },
                    { "Warranty", "2 years" }
                },
                Reviews = Enumerable.Range(1, 5).Select(r => new Review
                {
                    ReviewId = i * 100 + r,
                    Rating = 3 + (r % 3),
                    Title = $"Review title {r}",
                    Comment = $"This is review comment {r} for product {i}. The product quality is good and meets expectations. Would recommend to others.",
                    ReviewerName = $"User{r}",
                    ReviewDate = DateTime.UtcNow.AddDays(-r * 10)
                }).ToList(),
                Images = Enumerable.Range(1, 4).Select(img => new ProductImage
                {
                    ImageId = i * 10 + img,
                    Url = $"https://example.com/images/product-{i}-{img}.jpg",
                    AltText = $"Product {i} image {img}",
                    IsPrimary = img == 1
                }).ToList()
            }).ToList(),
            Metadata = new CatalogMetadata
            {
                TotalProducts = 50,
                TotalCategories = 4,
                LastUpdated = DateTime.UtcNow,
                Version = "1.0.0"
            }
        };
    }
}

public class Product
{
    public int ProductId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Category { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string Manufacturer { get; set; }
    public string SKU { get; set; }
    public List<string> Tags { get; set; }
    public Dictionary<string, string> Specifications { get; set; }
    public List<Review> Reviews { get; set; }
    public List<ProductImage> Images { get; set; }
}

public class Review
{
    public int ReviewId { get; set; }
    public int Rating { get; set; }
    public string Title { get; set; }
    public string Comment { get; set; }
    public string ReviewerName { get; set; }
    public DateTime ReviewDate { get; set; }
}

public class ProductImage
{
    public int ImageId { get; set; }
    public string Url { get; set; }
    public string AltText { get; set; }
    public bool IsPrimary { get; set; }
}

public class CatalogMetadata
{
    public int TotalProducts { get; set; }
    public int TotalCategories { get; set; }
    public DateTime LastUpdated { get; set; }
    public string Version { get; set; }
}