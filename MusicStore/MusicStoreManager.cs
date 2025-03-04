using System.Collections.Generic;
using System.Data.SqlClient;
using System;

public class MusicStoreManager
{
    private readonly SqlConnection conn;

    public MusicStoreManager(string connectionString)
    {
        conn = new SqlConnection(connectionString);
    }

    // Добавление новой пластинки
    public void AddVinyl(Vinyl vinyl)
    {
        using (conn)
        {
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO Vinyl (Name, Band, Label, TrackCount, Genre, Year, CostPrice, SalePrice)
                VALUES (@Name, @Band, @Label, @TrackCount, @Genre, @Year, @CostPrice, @SalePrice)";

            cmd.Parameters.AddWithValue("@Name", vinyl.Name);
            cmd.Parameters.AddWithValue("@Band", vinyl.Band);
            cmd.Parameters.AddWithValue("@Label", vinyl.Label);
            cmd.Parameters.AddWithValue("@TrackCount", vinyl.TrackCount);
            cmd.Parameters.AddWithValue("@Genre", vinyl.Genre);
            cmd.Parameters.AddWithValue("@Year", vinyl.Year);
            cmd.Parameters.AddWithValue("@CostPrice", vinyl.CostPrice);
            cmd.Parameters.AddWithValue("@SalePrice", vinyl.SalePrice);

            cmd.ExecuteNonQuery();
        }
    }

    // Поиск пластинок
    public List<Vinyl> SearchVinyl(string name = null, string band = null, string genre = null)
    {
        var vinyls = new List<Vinyl>();

        using (conn)
        {
            conn.Open();
            var cmd = conn.CreateCommand();
            var sql = @"
                SELECT * FROM Vinyl 
                WHERE (@Name IS NULL OR Name LIKE @Name)
                AND (@Band IS NULL OR Band LIKE @Band)
                AND (@Genre IS NULL OR Genre = @Genre)";

            cmd.CommandText = sql;

            if (!string.IsNullOrEmpty(name))
                cmd.Parameters.AddWithValue("@Name", $"%{name}%");
            else
                cmd.Parameters.AddWithValue("@Name", DBNull.Value);

            if (!string.IsNullOrEmpty(band))
                cmd.Parameters.AddWithValue("@Band", $"%{band}%");
            else
                cmd.Parameters.AddWithValue("@Band", DBNull.Value);

            if (!string.IsNullOrEmpty(genre))
                cmd.Parameters.AddWithValue("@Genre", genre);
            else
                cmd.Parameters.AddWithValue("@Genre", DBNull.Value);

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    vinyls.Add(new Vinyl
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Band = reader.GetString(2),
                        Label = reader.GetString(3),
                        TrackCount = reader.GetInt32(4),
                        Genre = reader.GetString(5),
                        Year = reader.GetInt32(6),
                        CostPrice = reader.GetDecimal(7),
                        SalePrice = reader.GetDecimal(8)
                    });
                }
            }
        }

        return vinyls;
    }

    // Получение списка новинок
    public List<Vinyl> GetNewReleases(int limit = 10)
    {
        var vinyls = new List<Vinyl>();

        using (conn)
        {
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT TOP (@Limit) * FROM Vinyl 
                ORDER BY Year DESC";

            cmd.Parameters.AddWithValue("@Limit", limit);

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    vinyls.Add(new Vinyl
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Band = reader.GetString(2),
                        Label = reader.GetString(3),
                        TrackCount = reader.GetInt32(4),
                        Genre = reader.GetString(5),
                        Year = reader.GetInt32(6),
                        CostPrice = reader.GetDecimal(7),
                        SalePrice = reader.GetDecimal(8)
                    });
                }
            }
        }

        return vinyls;
    }

    // Создание резервирования
    public void ReserveVinyl(int customerId, int vinylId)
    {
        using (conn)
        {
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO Reservation (CustomerId, VinylId, Date)
                VALUES (@CustomerId, @VinylId, GETDATE())";

            cmd.Parameters.AddWithValue("@CustomerId", customerId);
            cmd.Parameters.AddWithValue("@VinylId", vinylId);

            cmd.ExecuteNonQuery();
        }
    }

    // Создание заказа
    public void CreateOrder(int customerId, List<int> vinylIds, List<int> quantities)
    {
        using (conn)
        {
            conn.Open();
            var transaction = conn.BeginTransaction();

            try
            {
                // Создаем заказ
                var orderCmd = conn.CreateCommand();
                orderCmd.Transaction = transaction;
                orderCmd.CommandText = @"
                    INSERT INTO OrderTable (CustomerId, Date, TotalAmount, Status)
                    VALUES (@CustomerId, GETDATE(), @TotalAmount, 'Pending')";

                decimal totalAmount = 0;

                // Вычисляем общую сумму
                foreach (var vinylId in vinylIds)
                {
                    var priceCmd = conn.CreateCommand();
                    priceCmd.Transaction = transaction;
                    priceCmd.CommandText = "SELECT SalePrice FROM Vinyl WHERE Id = @VinylId";
                    priceCmd.Parameters.AddWithValue("@VinylId", vinylId);

                    totalAmount += Convert.ToDecimal(priceCmd.ExecuteScalar()) * quantities[vinylIds.IndexOf(vinylId)];
                }

                orderCmd.Parameters.AddWithValue("@CustomerId", customerId);
                orderCmd.Parameters.AddWithValue("@TotalAmount", totalAmount);

                var orderId = Convert.ToInt32(orderCmd.ExecuteScalar());

                // Добавляем товары в заказ
                foreach (var vinylId in vinylIds)
                {
                    var itemCmd = conn.CreateCommand();
                    itemCmd.Transaction = transaction;
                    itemCmd.CommandText = @"
                        INSERT INTO OrderItem (OrderId, VinylId, Quantity, Price)
                        VALUES (@OrderId, @VinylId, @Quantity, 
                            (SELECT SalePrice FROM Vinyl WHERE Id = @VinylId))";

                    itemCmd.Parameters.AddWithValue("@OrderId", orderId);
                    itemCmd.Parameters.AddWithValue("@VinylId", vinylId);
                    itemCmd.Parameters.AddWithValue("@Quantity", quantities[vinylIds.IndexOf(vinylId)]);

                    itemCmd.ExecuteNonQuery();
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }

    // Получение списка популярных жанров за период
    public List<string> GetPopularGenres(DateTime startDate, DateTime endDate)
    {
        var genres = new List<string>();

        using (conn)
        {
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT DISTINCT Genre 
                FROM Vinyl v
                JOIN OrderItem oi ON v.Id = oi.VinylId
                JOIN [Order] o ON oi.OrderId = o.Id
                WHERE o.Date BETWEEN @StartDate AND @EndDate
                ORDER BY COUNT(*) DESC";

            cmd.Parameters.AddWithValue("@StartDate", startDate);
            cmd.Parameters.AddWithValue("@EndDate", endDate);

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    genres.Add(reader.GetString(0));
                }
            }
        }

        return genres;
    }
}