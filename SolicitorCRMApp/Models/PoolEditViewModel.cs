namespace SolicitorCRMApp.Models;

public sealed class PoolEditViewModel
{
    public int? Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool Enabled { get; set; }
    public List<User> Users { get; set; } = new();
    public List<User> AssignedUsers { get; set; } = new();
    public int? SelectedUserId { get; set; }
}
