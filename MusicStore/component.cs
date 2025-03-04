using System.Data.SqlClient;
using System.Data;

public class DatabaseManager
{
    private readonly string _connectionString;

    public DatabaseManager(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected SqlConnection GetConnection()
    {
        return new SqlConnection(_connectionString);
    }

    protected void ExecuteSql(string sql, SqlParameter[] parameters = null)
    {
        using (var connection = GetConnection())
        {
            var command = connection.CreateCommand();
            command.CommandText = sql;

            if (parameters != null)
                command.Parameters.AddRange(parameters);

            connection.Open();
            command.ExecuteNonQuery();
        }
    }

    protected SqlDataReader GetDataReader(string sql, SqlParameter[] parameters = null)
    {
        using (var connection = GetConnection())
        {
            var command = connection.CreateCommand();
            command.CommandText = sql;

            if (parameters != null)
                command.Parameters.AddRange(parameters);

            connection.Open();
            return command.ExecuteReader(CommandBehavior.CloseConnection);
        }
    }
}