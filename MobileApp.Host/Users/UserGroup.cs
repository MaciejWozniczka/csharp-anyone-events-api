namespace MobileApp.Host.Users;

public class UserGroup : BaseModel
{
    public List<PendingUser> Users { get; set; }
    public string ShortText { get; set; }
    public bool IsVisible { get; set; } = false;
}

public class PendingUser : BaseModel
{
    public string UserId { get; set; }
    public bool Accepted { get; set; } = false;
}