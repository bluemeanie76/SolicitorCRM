using System.Data;
using Microsoft.Data.SqlClient;
using SolicitorCRMApp.Models;

namespace SolicitorCRMApp.Data;

public sealed class UserRepository : IUserRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public UserRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand("dbo.usp_User_GetByEmail", (SqlConnection)connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.AddWithValue("@Email", email);

        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            return null;
        }

        return MapUser(reader);
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand("dbo.usp_User_GetById", (SqlConnection)connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.AddWithValue("@UserId", id);

        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            return null;
        }

        return MapUser(reader);
    }

    public async Task<IReadOnlyList<User>> GetAllAsync()
    {
        var users = new List<User>();
        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand("dbo.usp_User_GetAll", (SqlConnection)connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            users.Add(MapUser(reader));
        }

        return users;
    }

    public async Task<IReadOnlyList<UserType>> GetUserTypesAsync()
    {
        var types = new List<UserType>();
        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand("dbo.usp_UserTypes_GetAll", (SqlConnection)connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            types.Add(new UserType
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Name = reader.GetString(reader.GetOrdinal("Name"))
            });
        }

        return types;
    }

    public async Task<int> CreateAsync(UserFormViewModel model, string passwordHash, string passwordSalt)
    {
        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand("dbo.usp_User_Insert", (SqlConnection)connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        AddUserParameters(command, model, passwordHash, passwordSalt, includePassword: true);

        await connection.OpenAsync();
        var result = await command.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }

    public async Task UpdateAsync(UserFormViewModel model, string? passwordHash, string? passwordSalt)
    {
        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand("dbo.usp_User_Update", (SqlConnection)connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.AddWithValue("@UserId", model.Id!.Value);
        AddUserParameters(command, model, passwordHash, passwordSalt, includePassword: !string.IsNullOrWhiteSpace(passwordHash));

        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();
    }

    public async Task SetEnabledAsync(int id, bool enabled)
    {
        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand("dbo.usp_User_SetEnabled", (SqlConnection)connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.AddWithValue("@UserId", id);
        command.Parameters.AddWithValue("@Enabled", enabled);

        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();
    }

    private static void AddUserParameters(SqlCommand command, UserFormViewModel model, string? passwordHash, string? passwordSalt, bool includePassword)
    {
        command.Parameters.AddWithValue("@Title", model.Title);
        command.Parameters.AddWithValue("@FirstName", model.FirstName);
        command.Parameters.AddWithValue("@Surname", model.Surname);
        command.Parameters.AddWithValue("@Email", model.Email);
        command.Parameters.AddWithValue("@JobTitle", model.JobTitle);
        command.Parameters.AddWithValue("@Department", model.Department);
        command.Parameters.AddWithValue("@UserTypeId", model.UserTypeId);
        command.Parameters.AddWithValue("@Enabled", model.Enabled);

        if (includePassword)
        {
            command.Parameters.AddWithValue("@PasswordHash", passwordHash ?? string.Empty);
            command.Parameters.AddWithValue("@PasswordSalt", passwordSalt ?? string.Empty);
        }
    }

    private static User MapUser(SqlDataReader reader)
    {
        return new User
        {
            Id = reader.GetInt32(reader.GetOrdinal("Id")),
            Title = reader.GetString(reader.GetOrdinal("Title")),
            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
            Surname = reader.GetString(reader.GetOrdinal("Surname")),
            Email = reader.GetString(reader.GetOrdinal("Email")),
            PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
            PasswordSalt = reader.GetString(reader.GetOrdinal("PasswordSalt")),
            JobTitle = reader.GetString(reader.GetOrdinal("JobTitle")),
            Department = reader.GetString(reader.GetOrdinal("Department")),
            UserTypeId = reader.GetInt32(reader.GetOrdinal("UserTypeId")),
            UserTypeName = reader.GetString(reader.GetOrdinal("UserTypeName")),
            Enabled = reader.GetBoolean(reader.GetOrdinal("Enabled")),
            DateAdded = reader.GetDateTime(reader.GetOrdinal("DateAdded")),
            DateUpdated = reader.GetDateTime(reader.GetOrdinal("DateUpdated"))
        };
    }
}
