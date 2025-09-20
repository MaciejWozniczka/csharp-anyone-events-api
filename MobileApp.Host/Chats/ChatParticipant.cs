using AnyOneApi.Host.Models;
using AnyOneApi.Host.Users;

namespace AnyOneApi.Host.Chats;

public class ChatParticipant : BaseModel
{
    public Guid ChatId { get; set; }
    public Chat Chat { get; set; }

    public string UserId { get; set; }
    public User User { get; set; }
}