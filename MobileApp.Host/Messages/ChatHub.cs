namespace MobileApp.Host.Messages;

public interface IChatHub
{
    Task JoinRoom(string chatId);
    Task LeaveRoom(string chatId);
    Task SendMessage(string chatId, string message);
}
public class ChatHub(DataContext db, ICurrentUserAccessor currentUserAccessor) : Hub, IChatHub
{
    public async Task JoinRoom(string chatId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, chatId);

        var username = Context.User?.Identity?.Name ?? "Nieznany";
        await Clients.Group(chatId).SendAsync("SystemMessage", $"{username} dołączył do pokoju.");
    }

    public async Task LeaveRoom(string chatId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId);

        var username = Context.User?.Identity?.Name ?? "Nieznany";
        await Clients.Group(chatId).SendAsync("SystemMessage", $"{username} opuścił pokój.");
    }

    public async Task SendMessage(string chatId, string message)
    {
        var username = Context.User?.Identity?.Name ?? "Nieznany";

        await Clients.Group(chatId).SendAsync("ReceiveMessage", new
        {
            chatId,
            sender = username,
            text = message,
            timestamp = DateTime.UtcNow
        });
    }
}