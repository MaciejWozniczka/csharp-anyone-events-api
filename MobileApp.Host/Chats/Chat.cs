using AnyOneApi.Host.Events;
using AnyOneApi.Host.Messages;
using AnyOneApi.Host.Models;

namespace AnyOneApi.Host.Chats;

public class Chat : BaseModel
{
    public string Name { get; set; }
    public string RoomId { get; set; }
    public List<ChatParticipant> Participants { get; set; }
    public Guid? EventId { get; set; }
    public UserEvent? Event { get; set; }
    public List<Message> Messages { get; set; }
}