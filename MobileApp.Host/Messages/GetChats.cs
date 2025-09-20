using AnyOneApi.Host.Infrastructure;

namespace AnyOneApi.Host.Messages;

[ApiController]
public class GetChats(IMediator mediator, ICurrentUserAccessor currentUserAccessor)
{
    [Authorize]
    [SwaggerOperation(Tags = ["Messages"], Summary = "Get chats list")]
    [HttpGet("/api/chat/")]
    public async Task<Result<List<GetChatsDto>>> GetChatsAsync()
    {
        var currentUser = await currentUserAccessor.GetCurrentUser();

        return await mediator.Send(new GetChatsQuery(currentUser.Id));
    }

    public class GetChatsQuery(string id) : IRequest<Result<List<GetChatsDto>>>
    {
        public string Id { get; set; } = id;
    }

    public class GetChatsDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string EventDate { get; set; }
        public List<string> Participants { get; set; }
        public DateTimeOffset LastMessageDate { get; set; }
    }

    public class GetChatsQueryHandler(DataContext db, ICurrentUserAccessor currentUserAccessor) : IRequestHandler<GetChatsQuery, Result<List<GetChatsDto>>>
    {
        public async Task<Result<List<GetChatsDto>>> Handle(GetChatsQuery request, CancellationToken cancellationToken)
        {
            var currentUser = await currentUserAccessor.GetCurrentUser();

            var chats = await db.Chats
                .Where(c => c.Participants.Select(p => p.UserId).Contains(currentUser.Id))
                .Select(ch => new GetChatsDto
                {
                    Id = ch.Id,
                    Name = ch.Name,
                    Participants = ch.Participants.Select(p => p.User.FirstName).ToList(),
                    LastMessageDate = ch.Messages.OrderByDescending(m => m.CreateDate).FirstOrDefault().CreateDate,
                    EventDate = ch.Event.EventDateTime.DateTime.ToShortDateString()
                })
                .OrderByDescending(c => c.LastMessageDate)
                .ToListAsync(cancellationToken);

            return Result.Ok(chats);
        }
    }
}