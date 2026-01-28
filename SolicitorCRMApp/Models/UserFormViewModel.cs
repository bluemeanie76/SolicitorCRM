using System.ComponentModel.DataAnnotations;

namespace SolicitorCRMApp.Models;

public sealed class UserFormViewModel
{
    public int? Id { get; set; }

    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    public string Surname { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string JobTitle { get; set; } = string.Empty;

    [Required]
    public string Department { get; set; } = string.Empty;

    [Required]
    public int UserTypeId { get; set; }

    public bool Enabled { get; set; }

    public List<UserType> UserTypes { get; set; } = new();
}
