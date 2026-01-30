namespace SolicitorCRMApp.Models;

public sealed class TaskDashboardViewModel
{
    public IReadOnlyList<TaskItem> AssignedTasks { get; set; } = Array.Empty<TaskItem>();
    public IReadOnlyList<TaskItem> PoolTasks { get; set; } = Array.Empty<TaskItem>();
}
