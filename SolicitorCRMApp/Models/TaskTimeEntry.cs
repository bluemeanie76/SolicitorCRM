namespace SolicitorCRMApp.Models;

public sealed class TaskTimeEntry
{
    public int Id { get; set; }
    public int TaskId { get; set; }
    public int LoggedByUserId { get; set; }
    public int Hours { get; set; }
    public int Minutes { get; set; }
    public int TotalMinutes { get; set; }
    public DateTime DateAdded { get; set; }
}
