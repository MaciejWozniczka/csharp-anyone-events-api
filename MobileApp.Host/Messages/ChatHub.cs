namespace MobileApp.Host.Messages;

public interface IChatHub
{
    Task JoinRoom(string chatId);
    Task LeaveRoom(string chatId);
    Task SendMessage(string chatId, string message);
}
public class ChatHub(DataContext db, ICurrentUserAccessor currentUserAccessor) : Hub, IChatHub
{
    public async Task JoinRoom(string roomId)
    {
        var chat = await db.Chats
            .Where(c => c.RoomId == roomId)
            .Include(chat => chat.Participants)
            .FirstOrDefaultAsync();

        var user = await currentUserAccessor.GetCurrentUser();

        var senderUserName = user.FirstName;

        if (user.LastName != null)
        {
            senderUserName += $" {user.LastName.Substring(0, 1)}";
        }

        chat.Participants.Add(new ChatParticipant
        {
            UserId = user.Id
        });

        await Groups.AddToGroupAsync(Context.ConnectionId, roomId);

        await Clients.Group(roomId).SendAsync("SystemMessage", $"{senderUserName} dołączył do pokoju.");
    }

    public async Task LeaveRoom(string roomId)
    {
        var chat = await db.Chats
            .Where(c => c.RoomId == roomId)
            .Include(chat => chat.Participants)
            .FirstOrDefaultAsync();

        var user = await currentUserAccessor.GetCurrentUser();

        var senderUserName = user.FirstName;

        if (user.LastName != null)
        {
            senderUserName += $" {user.LastName.Substring(0, 1)}";
        }

        var participant = chat.Participants.FirstOrDefault(p => p.UserId == user.Id);

        chat.Participants.Remove(participant);

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);

        await Clients.Group(roomId).SendAsync("SystemMessage", $"{senderUserName} opuścił pokój.");
    }

    public async Task SendMessage(string roomId, string message)
    {
        var user = await currentUserAccessor.GetCurrentUser();

        var senderUserName = user.FirstName;

        if (user.LastName != null)
        {
            senderUserName += $" {user.LastName.Substring(0, 1)}";
        }

        var messageDate = DateTime.UtcNow;

        var chatId = await db.Chats
            .Where(c => c.RoomId == roomId)
            .Select(c => c.Id)
            .FirstOrDefaultAsync();

        var messageModel = new Message
        {
            Id = Guid.NewGuid(),
            ChatId = chatId,
            RoomId = roomId,
            SenderUserId = user.Id,
            SenderUsername = senderUserName,
            Text = message,
            CreateDate = messageDate,
            IsDeleted = false
        };

        db.Messages.Add(messageModel);
        await db.SaveChangesAsync();

        await Clients.Group(roomId).SendAsync("ReceiveMessage", new
        {
            chatId,
            sender = senderUserName,
            text = message,
            timestamp = messageDate
        });
    }
}