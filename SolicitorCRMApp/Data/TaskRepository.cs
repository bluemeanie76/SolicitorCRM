using System.Data;
using Microsoft.Data.SqlClient;
using SolicitorCRMApp.Models;

namespace SolicitorCRMApp.Data;

public sealed class TaskRepository : ITaskRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public TaskRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<TaskItem?> GetByIdAsync(int taskId)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await using var command = new SqlCommand("dbo.usp_Task_GetById", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.AddWithValue("@TaskId", taskId);

        await connection.OpenAsync();
        await using var reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
        {
            return null;
        }

        return MapTask(reader);
    }

    public async Task<IReadOnlyList<TaskItem>> GetAllAsync()
    {
        var tasks = new List<TaskItem>();
        await using var connection = _connectionFactory.CreateConnection();
        await using var command = new SqlCommand("dbo.usp_Task_GetAll", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        await connection.OpenAsync();
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            tasks.Add(MapTask(reader));
        }

        return tasks;
    }

    public async Task<IReadOnlyList<TaskItem>> GetAssignedToUserAsync(int userId)
    {
        return await GetTasksAsync("dbo.usp_Task_GetAssignedToUser", userId);
    }

    public async Task<IReadOnlyList<TaskItem>> GetAssignedToUserPoolsAsync(int userId)
    {
        return await GetTasksAsync("dbo.usp_Task_GetAssignedToUserPools", userId);
    }

    public async Task<int> CreateAsync(TaskCreateModel model)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await using var command = new SqlCommand("dbo.usp_Task_Insert", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        AddTaskParameters(command, model.IsUrgent, model.ContactName, model.ContactEmail, model.ContactTelephone,
            model.ContactNotes, model.TaskDescription, model.TaskDeadline, model.AssignedUserId, model.AssignedPoolId);
        command.Parameters.AddWithValue("@CreatedByUserId", model.CreatedByUserId);

        await connection.OpenAsync();
        var result = await command.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }

    public async Task UpdateAsync(TaskUpdateModel model)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await using var command = new SqlCommand("dbo.usp_Task_Update", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.AddWithValue("@TaskId", model.Id);
        AddTaskParameters(command, model.IsUrgent, model.ContactName, model.ContactEmail, model.ContactTelephone,
            model.ContactNotes, model.TaskDescription, model.TaskDeadline, model.AssignedUserId, model.AssignedPoolId);

        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();
    }

    public async Task<int> AddNoteAsync(int taskId, int createdByUserId, string note)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await using var command = new SqlCommand("dbo.usp_Task_AddNote", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.AddWithValue("@TaskId", taskId);
        command.Parameters.AddWithValue("@CreatedByUserId", createdByUserId);
        command.Parameters.AddWithValue("@Note", note);

        await connection.OpenAsync();
        var result = await command.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }

    public async Task<IReadOnlyList<TaskNote>> GetNotesAsync(int taskId)
    {
        var notes = new List<TaskNote>();
        await using var connection = _connectionFactory.CreateConnection();
        await using var command = new SqlCommand("dbo.usp_Task_GetNotes", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.AddWithValue("@TaskId", taskId);

        await connection.OpenAsync();
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            notes.Add(new TaskNote
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                TaskId = reader.GetInt32(reader.GetOrdinal("TaskId")),
                Note = reader.GetString(reader.GetOrdinal("Note")),
                CreatedByUserId = reader.GetInt32(reader.GetOrdinal("CreatedByUserId")),
                DateAdded = reader.GetDateTime(reader.GetOrdinal("DateAdded"))
            });
        }

        return notes;
    }

    public async Task<int> LogTimeAsync(TaskTimeEntryCreateModel model)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await using var command = new SqlCommand("dbo.usp_Task_LogTime", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.AddWithValue("@TaskId", model.TaskId);
        command.Parameters.AddWithValue("@LoggedByUserId", model.LoggedByUserId);
        command.Parameters.AddWithValue("@Hours", model.Hours);
        command.Parameters.AddWithValue("@Minutes", model.Minutes);

        await connection.OpenAsync();
        var result = await command.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }

    public async Task<IReadOnlyList<TaskTimeEntry>> GetTimeEntriesAsync(int taskId)
    {
        var entries = new List<TaskTimeEntry>();
        await using var connection = _connectionFactory.CreateConnection();
        await using var command = new SqlCommand("dbo.usp_Task_GetTimeEntries", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.AddWithValue("@TaskId", taskId);

        await connection.OpenAsync();
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            entries.Add(new TaskTimeEntry
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                TaskId = reader.GetInt32(reader.GetOrdinal("TaskId")),
                LoggedByUserId = reader.GetInt32(reader.GetOrdinal("LoggedByUserId")),
                Hours = reader.GetInt32(reader.GetOrdinal("Hours")),
                Minutes = reader.GetInt32(reader.GetOrdinal("Minutes")),
                TotalMinutes = reader.GetInt32(reader.GetOrdinal("TotalMinutes")),
                DateAdded = reader.GetDateTime(reader.GetOrdinal("DateAdded")),
                LoggedByName = reader.GetString(reader.GetOrdinal("LoggedByName"))
            });
        }

        return entries;
    }

    private async Task<IReadOnlyList<TaskItem>> GetTasksAsync(string storedProcedure, int userId)
    {
        var tasks = new List<TaskItem>();
        await using var connection = _connectionFactory.CreateConnection();
        await using var command = new SqlCommand(storedProcedure, connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.AddWithValue("@UserId", userId);

        await connection.OpenAsync();
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            tasks.Add(MapTask(reader));
        }

        return tasks;
    }

    private static void AddTaskParameters(SqlCommand command, bool isUrgent, string contactName, string contactEmail,
        string? contactTelephone, string? contactNotes, string taskDescription, DateTime taskDeadline, int? assignedUserId,
        int? assignedPoolId)
    {
        command.Parameters.AddWithValue("@IsUrgent", isUrgent);
        command.Parameters.AddWithValue("@ContactName", contactName);
        command.Parameters.AddWithValue("@ContactEmail", contactEmail);
        command.Parameters.AddWithValue("@ContactTelephone", contactTelephone ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@ContactNotes", contactNotes ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@TaskDescription", taskDescription);
        command.Parameters.AddWithValue("@TaskDeadline", taskDeadline);
        command.Parameters.AddWithValue("@AssignedUserId", assignedUserId ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@AssignedPoolId", assignedPoolId ?? (object)DBNull.Value);
    }

    private static TaskItem MapTask(SqlDataReader reader)
    {
        return new TaskItem
        {
            Id = reader.GetInt32(reader.GetOrdinal("Id")),
            IsUrgent = reader.GetBoolean(reader.GetOrdinal("IsUrgent")),
            ContactName = reader.GetString(reader.GetOrdinal("ContactName")),
            ContactEmail = reader.GetString(reader.GetOrdinal("ContactEmail")),
            ContactTelephone = reader.IsDBNull(reader.GetOrdinal("ContactTelephone"))
                ? null
                : reader.GetString(reader.GetOrdinal("ContactTelephone")),
            ContactNotes = reader.IsDBNull(reader.GetOrdinal("ContactNotes"))
                ? null
                : reader.GetString(reader.GetOrdinal("ContactNotes")),
            TaskDescription = reader.GetString(reader.GetOrdinal("TaskDescription")),
            TaskDeadline = reader.GetDateTime(reader.GetOrdinal("TaskDeadline")),
            AssignedUserId = reader.IsDBNull(reader.GetOrdinal("AssignedUserId"))
                ? null
                : reader.GetInt32(reader.GetOrdinal("AssignedUserId")),
            AssignedUserName = reader.IsDBNull(reader.GetOrdinal("AssignedUserName"))
                ? null
                : reader.GetString(reader.GetOrdinal("AssignedUserName")),
            AssignedPoolId = reader.IsDBNull(reader.GetOrdinal("AssignedPoolId"))
                ? null
                : reader.GetInt32(reader.GetOrdinal("AssignedPoolId")),
            AssignedPoolName = reader.IsDBNull(reader.GetOrdinal("AssignedPoolName"))
                ? null
                : reader.GetString(reader.GetOrdinal("AssignedPoolName")),
            CreatedByUserId = reader.GetInt32(reader.GetOrdinal("CreatedByUserId")),
            TotalMinutes = reader.GetInt32(reader.GetOrdinal("TotalMinutes")),
            DateAdded = reader.GetDateTime(reader.GetOrdinal("DateAdded")),
            DateUpdated = reader.GetDateTime(reader.GetOrdinal("DateUpdated"))
        };
    }
}
