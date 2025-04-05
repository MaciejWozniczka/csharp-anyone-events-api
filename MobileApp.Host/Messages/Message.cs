namespace MobileApp.Host.Messages;

public class Message : BaseModel
{
    public string RoomId { get; set; }
    public string SenderUserId { get; set; }
    public string SenderUsername { get; set; }
    public string Text { get; set; }
}