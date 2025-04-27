namespace MobileApp.Host.Messages;

[ApiController]
public class GetMessages(IMediator mediator)
{
    [Authorize]
    [SwaggerOperation(Tags = ["Messages"], Summary = "Get messages")]
    [HttpGet("/api/message/{id}")]
    public async Task<Result<List<GetMessagesDto>>> GetMessagesAsync(int? offset, int? limit, Guid id)
    {
        var pagination = new PaginationArgs
        {
            Page = offset ?? 0,
            PageSize = limit ?? 10
        };
        return await mediator.Send(new GetMessagesQuery(id, pagination));
    }

    public class GetMessagesQuery : IRequest<Result<List<GetMessagesDto>>>
    {
        public Guid Id { get; set; }
        public PaginationArgs PaginationArgs { get; set; }
        public GetMessagesQuery(Guid id, PaginationArgs paginationArgs)
        {
            Id = id;
            PaginationArgs = paginationArgs;
        }
    }

    public class GetMessagesDto
    {
        public Guid Id { get; set; }
        public string RoomId { get; set; }
        public string ChatName { get; set; }
        public string SenderUserId { get; set; }
        public string SenderUsername { get; set; }
        public string Text { get; set; }
        public DateTimeOffset CreateDate { get; set; }
    }

    public class GetMessagesQueryHandler(DataContext db) : IRequestHandler<GetMessagesQuery, Result<List<GetMessagesDto>>>
    {
        public async Task<Result<List<GetMessagesDto>>> Handle(GetMessagesQuery request, CancellationToken cancellationToken)
        {
            var messages = await db.Messages
                .Where(m => m.ChatId == request.Id)
                .OrderByDescending(m => m.CreateDate)
                .Skip(request.PaginationArgs.PageSize * request.PaginationArgs.Page)
                .Take(request.PaginationArgs.PageSize)
                .Select(m => new GetMessagesDto
                {
                    Id = m.Id,
                    RoomId = m.RoomId,
                    ChatName = m.Chat.Name,
                    SenderUserId = m.SenderUserId,
                    SenderUsername = m.SenderUsername,
                    Text = m.Text,
                    CreateDate = m.CreateDate
                })
                .ToListAsync(cancellationToken);

            return Result.Ok(messages);
        }
    }
}