namespace MobileApp.Host.Messages;

public interface IChatHub
{
    Task SendMessage(Guid chatId, string messageText);
}
public class ChatHub(DataContext db, ICurrentUserAccessor currentUserAccessor) : Hub, IChatHub
{
    public async Task SendMessage(Guid chatId, string messageText)
    {
        var currentUser = await currentUserAccessor.GetCurrentUser();

        var message = new Message
        {
            RoomId = chatId.ToString(),
            ChatId = chatId,
            SenderUserId = currentUser.Id,
            SenderUsername = currentUser.FirstName,
            Text = messageText
        };

        db.Messages.Add(message);
        await db.SaveChangesAsync();

        await Clients.Group(message.RoomId).SendAsync("ReceiveMessage", message.SenderUsername, messageText);
    }
}