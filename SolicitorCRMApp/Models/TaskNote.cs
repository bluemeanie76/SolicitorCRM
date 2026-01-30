namespace SolicitorCRMApp.Models;

public sealed class TaskNote
{
    public int Id { get; set; }
    public int TaskId { get; set; }
    public string Note { get; set; } = string.Empty;
    public int CreatedByUserId { get; set; }
    public DateTime DateAdded { get; set; }
}
