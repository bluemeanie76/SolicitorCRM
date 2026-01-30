namespace SolicitorCRMApp.Models;

public sealed class TaskTimeEntryCreateModel
{
    public int TaskId { get; set; }
    public int LoggedByUserId { get; set; }
    public int Hours { get; set; }
    public int Minutes { get; set; }
}
