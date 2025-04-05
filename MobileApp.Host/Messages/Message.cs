namespace MobileApp.Host.Messages;

public class Message : BaseModel
{
    public string RoomId { get; set; }
    public string SenderUserId { get; set; }
    public string SenderUsername { get; set; }
    public string Text { get; set; }
    public Guid ChatId { get; set; }
    public Chat Chat { get; set; }
}