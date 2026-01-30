namespace SolicitorCRMApp.Models;

public sealed class TaskFormViewModel
{
    public int? Id { get; set; }
    public bool IsUrgent { get; set; }
    public string ContactName { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    public string? ContactTelephone { get; set; }
    public string? ContactNotes { get; set; }
    public string TaskDescription { get; set; } = string.Empty;
    public DateTime TaskDeadline { get; set; } = DateTime.UtcNow.Date.AddDays(1);
    public int? AssignedUserId { get; set; }
    public int? AssignedPoolId { get; set; }
    public bool CanEditPool { get; set; }
    public List<User> Users { get; set; } = new();
    public List<Pool> Pools { get; set; } = new();
}
