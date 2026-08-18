namespace Domain;

public enum DiscountTier
{
    Standard,
    Premium
}

public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DiscountTier DiscountTier { get; set; }
}
