using Microsoft.AspNetCore.SignalR;

namespace MobileApp.Host.Messages;

public class ChatHub : Hub
{
    private readonly DataContext _db;
    private readonly ICurrentUserAccessor _currentUserAccessor;
    public ChatHub(DataContext db, ICurrentUserAccessor currentUserAccessor)
    {
        _db = db;
        _currentUserAccessor = currentUserAccessor;
    }

    public async Task SendMessage(string roomName, string messageText)
    {
        var currentUser = await _currentUserAccessor.GetCurrentUser();

        var message = new Message
        {
            RoomId = roomName,
            SenderUserId = currentUser.Id,
            SenderUsername = currentUser.FirstName,
            Text = messageText
        };

        _db.Messages.Add(message);
        await _db.SaveChangesAsync();

        await Clients.Group(roomName).SendAsync("ReceiveMessage", message.SenderUsername, messageText);
    }
}