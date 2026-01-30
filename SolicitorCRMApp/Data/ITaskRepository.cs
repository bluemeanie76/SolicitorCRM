using SolicitorCRMApp.Models;

namespace SolicitorCRMApp.Data;

public interface ITaskRepository
{
    Task<TaskItem?> GetByIdAsync(int taskId);
    Task<IReadOnlyList<TaskItem>> GetAllAsync();
    Task<IReadOnlyList<TaskItem>> GetAssignedToUserAsync(int userId);
    Task<IReadOnlyList<TaskItem>> GetAssignedToUserPoolsAsync(int userId);
    Task<int> CreateAsync(TaskCreateModel model);
    Task UpdateAsync(TaskUpdateModel model);
    Task<int> AddNoteAsync(int taskId, int createdByUserId, string note);
    Task<IReadOnlyList<TaskNote>> GetNotesAsync(int taskId);
    Task<int> LogTimeAsync(TaskTimeEntryCreateModel model);
    Task<IReadOnlyList<TaskTimeEntry>> GetTimeEntriesAsync(int taskId);
}
