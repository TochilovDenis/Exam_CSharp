using System.Collections.Generic;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System;
using MusicStore;

public class StoreManager : DatabaseManager
{
    public StoreManager(string connectionString)
        : base(connectionString) { }

    public void AddAlbum(Album album)
    {
        const string sql = @"
        INSERT INTO Albums (Title, Artist, Label, TrackCount, GenreID, 
                          YearReleased, CostPrice, SellingPrice)
        VALUES (@Title, @Artist, @Label, @TrackCount, @GenreID, 
               @YearReleased, @CostPrice, @SellingPrice)";

        var parameters = new[]
        {
        new SqlParameter("@Title", album.Title),
        new SqlParameter("@Artist", album.Artist),
        new SqlParameter("@Label", album.Label ?? (object)DBNull.Value),
        new SqlParameter("@TrackCount", album.TrackCount == null ? (object)DBNull.Value : album.TrackCount),
        new SqlParameter("@GenreID", album.GenreID == null ? (object)DBNull.Value : album.GenreID),
        new SqlParameter("@YearReleased", album.YearReleased == null ? (object)DBNull.Value : album.YearReleased),
        new SqlParameter("@CostPrice", album.CostPrice),
        new SqlParameter("@SellingPrice", album.SellingPrice)
    };

        ExecuteSql(sql, parameters);
    }

    public List<Album> SearchAlbums(string searchTerm)
    {
        const string sql = @"
            SELECT * FROM Albums 
            WHERE Title LIKE @SearchTerm OR Artist LIKE @SearchTerm";

        using (var reader = GetDataReader(sql,
            new[] { new SqlParameter("@SearchTerm", $"%{searchTerm}%") }))
        {
            var albums = new List<Album>();
            while (reader.Read())
            {
                albums.Add(MapToAlbum(reader));
            }
            return albums;
        }
    }

    private Album MapToAlbum(SqlDataReader reader)
    {
        return new Album
        {
            AlbumID = reader.GetInt32("AlbumID"),
            Title = reader.GetString("Title"),
            Artist = reader.GetString("Artist"),
            Label = reader.IsDBNull("Label") ? null : reader.GetString("Label"),
            TrackCount = reader.IsDBNull("TrackCount") ? (int?)null : reader.GetInt32("TrackCount"),
            GenreID = reader.IsDBNull("GenreID") ? (int?)null : reader.GetInt32("GenreID"),
            YearReleased = reader.IsDBNull("YearReleased") ? (int?)null : reader.GetInt32("YearReleased"),
            CostPrice = reader.GetDecimal("CostPrice"),
            SellingPrice = reader.GetDecimal("SellingPrice")
        };
    }
}

public class CustomerManager : DatabaseManager
{
    public CustomerManager(string connectionString)
        : base(connectionString) { }

    public void RegisterCustomer(Customer customer)
    {
        const string sql = @"
            INSERT INTO Users (Login, PasswordHash, IsAdmin)
            VALUES (@Login, @PasswordHash, 0);
            
            DECLARE @UserID INT = SCOPE_IDENTITY();
            
            INSERT INTO Customers (UserID, TotalSpent, RegistrationDate)
            VALUES (@UserID, 0, GETDATE())";

        var parameters = new[]
        {
            new SqlParameter("@Login", customer.Login),
            new SqlParameter("@PasswordHash", HashPassword(customer.PasswordHash))
        };

        ExecuteSql(sql, parameters);
    }

    private string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}

public class PromotionManager : DatabaseManager
{
    public PromotionManager(string connectionString)
        : base(connectionString) { }

    public void CreatePromotion(Promotion promo)
    {
        const string sql = @"
            INSERT INTO CustomerDiscounts (CustomerID, MinPurchase, DiscountPercentage)
            VALUES (@CustomerID, @MinPurchase, @DiscountPercentage)";

        var parameters = new[]
        {
            new SqlParameter("@CustomerID", promo.CustomerID),
            new SqlParameter("@MinPurchase", promo.MinPurchase),
            new SqlParameter("@DiscountPercentage", promo.DiscountPercentage)
        };

        ExecuteSql(sql, parameters);
    }
}