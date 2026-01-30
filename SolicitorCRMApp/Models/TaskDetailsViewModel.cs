namespace SolicitorCRMApp.Models;

public sealed class TaskDetailsViewModel
{
    public TaskFormViewModel Task { get; set; } = new();
    public IReadOnlyList<TaskNote> Notes { get; set; } = Array.Empty<TaskNote>();
    public IReadOnlyList<TaskTimeEntry> TimeEntries { get; set; } = Array.Empty<TaskTimeEntry>();
    public string? NewNote { get; set; }
    public int LogHours { get; set; }
    public int LogMinutes { get; set; }
}
