using System.Data;
using Microsoft.Data.SqlClient;
using SolicitorCRMApp.Models;

namespace SolicitorCRMApp.Data;

public sealed class PoolRepository : IPoolRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public PoolRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<Pool>> GetAllAsync()
    {
        var pools = new List<Pool>();
        await using var connection = _connectionFactory.CreateConnection();
        await using var command = new SqlCommand("dbo.usp_Pool_GetAll", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        await connection.OpenAsync();
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            pools.Add(MapPool(reader));
        }

        return pools;
    }

    public async Task<int> CreateAsync(Pool pool)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await using var command = new SqlCommand("dbo.usp_Pool_Insert", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.AddWithValue("@Name", pool.Name);
        command.Parameters.AddWithValue("@Description", pool.Description ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@Enabled", pool.Enabled);

        await connection.OpenAsync();
        var result = await command.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }

    public async Task UpdateAsync(Pool pool)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await using var command = new SqlCommand("dbo.usp_Pool_Update", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.AddWithValue("@PoolId", pool.Id);
        command.Parameters.AddWithValue("@Name", pool.Name);
        command.Parameters.AddWithValue("@Description", pool.Description ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@Enabled", pool.Enabled);

        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();
    }

    public async Task SetEnabledAsync(int poolId, bool enabled)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await using var command = new SqlCommand("dbo.usp_Pool_SetEnabled", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.AddWithValue("@PoolId", poolId);
        command.Parameters.AddWithValue("@Enabled", enabled);

        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();
    }

    public async Task AssignUserAsync(int poolId, int userId)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await using var command = new SqlCommand("dbo.usp_Pool_AssignUser", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.AddWithValue("@PoolId", poolId);
        command.Parameters.AddWithValue("@UserId", userId);

        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();
    }

    public async Task RemoveUserAsync(int poolId, int userId)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await using var command = new SqlCommand("dbo.usp_Pool_RemoveUser", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.AddWithValue("@PoolId", poolId);
        command.Parameters.AddWithValue("@UserId", userId);

        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();
    }

    public async Task<IReadOnlyList<User>> GetUsersAsync(int poolId)
    {
        var users = new List<User>();
        await using var connection = _connectionFactory.CreateConnection();
        await using var command = new SqlCommand("dbo.usp_Pool_GetUsers", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.AddWithValue("@PoolId", poolId);

        await connection.OpenAsync();
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            users.Add(new User
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Title = reader.GetString(reader.GetOrdinal("Title")),
                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                Surname = reader.GetString(reader.GetOrdinal("Surname")),
                Email = reader.GetString(reader.GetOrdinal("Email")),
                JobTitle = reader.GetString(reader.GetOrdinal("JobTitle")),
                Department = reader.GetString(reader.GetOrdinal("Department")),
                UserTypeId = reader.GetInt32(reader.GetOrdinal("UserTypeId")),
                UserTypeName = reader.GetString(reader.GetOrdinal("UserTypeName")),
                Enabled = reader.GetBoolean(reader.GetOrdinal("Enabled")),
                DateAdded = reader.GetDateTime(reader.GetOrdinal("DateAdded")),
                DateUpdated = reader.GetDateTime(reader.GetOrdinal("DateUpdated"))
            });
        }

        return users;
    }

    private static Pool MapPool(SqlDataReader reader)
    {
        return new Pool
        {
            Id = reader.GetInt32(reader.GetOrdinal("Id")),
            Name = reader.GetString(reader.GetOrdinal("Name")),
            Description = reader.IsDBNull(reader.GetOrdinal("Description"))
                ? null
                : reader.GetString(reader.GetOrdinal("Description")),
            Enabled = reader.GetBoolean(reader.GetOrdinal("Enabled")),
            DateAdded = reader.GetDateTime(reader.GetOrdinal("DateAdded")),
            DateUpdated = reader.GetDateTime(reader.GetOrdinal("DateUpdated"))
        };
    }
}
