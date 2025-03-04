using System;

public class Vinyl
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Band { get; set; }
    public string Label { get; set; }
    public int TrackCount { get; set; }
    public string Genre { get; set; }
    public int Year { get; set; }
    public decimal CostPrice { get; set; }
    public decimal SalePrice { get; set; }
}

public class Customer
{
    public int Id { get; set; }
    public string Login { get; set; }
    public string PasswordHash { get; set; }
    public decimal TotalSpent { get; set; }
    public string DiscountLevel { get; set; } // "Basic", "Silver", "Gold"
}

public class Order
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public DateTime Date { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } // "Pending", "Completed", "Cancelled"
}

public class Promotion
{
    public int Id { get; set; }
    public string Genre { get; set; }
    public float DiscountPercentage { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}

public class Reservation
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public int VinylId { get; set; }
    public DateTime Date { get; set; }
}