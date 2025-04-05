namespace MobileApp.Host.Chats;

public class Chat : BaseModel
{
    public string Name { get; set; }
    public List<ChatParticipant> Participants { get; set; }
    public List<Message> Messages { get; set; }
}