namespace SolicitorCRMApp.Models;

public sealed class TaskItem
{
    public int Id { get; set; }
    public bool IsUrgent { get; set; }
    public string ContactName { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    public string? ContactTelephone { get; set; }
    public string? ContactNotes { get; set; }
    public string TaskDescription { get; set; } = string.Empty;
    public DateTime TaskDeadline { get; set; }
    public int? AssignedUserId { get; set; }
    public int? AssignedPoolId { get; set; }
    public int CreatedByUserId { get; set; }
    public int TotalMinutes { get; set; }
    public DateTime DateAdded { get; set; }
    public DateTime DateUpdated { get; set; }
}
