using Microsoft.AspNetCore.SignalR;

namespace MobileApp.Host.Messages;

public interface IChatHub
{
    Task SendMessage(string roomName, string messageText);
}
public class ChatHub(DataContext db, ICurrentUserAccessor currentUserAccessor) : Hub, IChatHub
{
    public async Task SendMessage(string roomName, string messageText)
    {
        var currentUser = await currentUserAccessor.GetCurrentUser();

        var message = new Message
        {
            RoomId = roomName,
            SenderUserId = currentUser.Id,
            SenderUsername = currentUser.FirstName,
            Text = messageText
        };

        db.Messages.Add(message);
        await db.SaveChangesAsync();

        await Clients.Group(roomName).SendAsync("ReceiveMessage", message.SenderUsername, messageText);
    }
}