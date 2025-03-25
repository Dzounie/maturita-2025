namespace Eshop.Frontend.Utils;

public class ChangeRoleRequest
{
    public int UserId { get; set; }
    public string NewRole { get; set; } = string.Empty;
}
