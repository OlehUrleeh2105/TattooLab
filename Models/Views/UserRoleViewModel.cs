namespace TatooLab.Models.Views;

public class UserRoleViewModel
{
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public List<string>? AvailableRoles { get; set; }
    public string? SelectedRole { get; set; }
}