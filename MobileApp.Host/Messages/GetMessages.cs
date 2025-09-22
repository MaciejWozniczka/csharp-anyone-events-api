using AnyOneApi.Host.Extensions;
using AnyOneApi.Host.Infrastructure;

namespace AnyOneApi.Host.Messages;

[ApiController]
public class GetMessages(IMediator mediator)
{
    [Authorize]
    [SwaggerOperation(Tags = ["Messages"], Summary = "Get messages")]
    [HttpGet("/api/message/{id}")]
    public async Task<Result<List<GetMessagesDto>>> GetMessagesAsync(
        /// <summary>Przesunięcie dla paginacji (opcjonalny)</summary>
        int? offset, 
        /// <summary>Liczba elementów na stronę (opcjonalny)</summary>
        int? limit, 
        /// <summary>ID czatu</summary>
        Guid id)
    {
        var pagination = new PaginationArgs
        {
            Page = offset ?? 0,
            PageSize = limit ?? 10
        };
        return await mediator.Send(new GetMessagesQuery(id, pagination));
    }

    public class GetMessagesQuery(Guid id, PaginationArgs paginationArgs) : IRequest<Result<List<GetMessagesDto>>>
    {
        /// <summary>ID czatu</summary>
        public Guid Id { get; set; } = id;
        /// <summary>Parametry paginacji</summary>
        public PaginationArgs PaginationArgs { get; set; } = paginationArgs;
    }

    public class GetMessagesDto
    {
        /// <summary>ID wiadomości</summary>
        public Guid Id { get; set; }
        /// <summary>ID pokoju</summary>
        public string RoomId { get; set; }
        /// <summary>Nazwa czatu</summary>
        public string ChatName { get; set; }
        /// <summary>ID nadawcy</summary>
        public string SenderUserId { get; set; }
        /// <summary>Nazwa użytkownika nadawcy</summary>
        public string SenderUsername { get; set; }
        /// <summary>Treść wiadomości</summary>
        public string Text { get; set; }
        /// <summary>Data utworzenia</summary>
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