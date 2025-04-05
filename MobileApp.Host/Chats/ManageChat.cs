namespace MobileApp.Host.Chats;

[ApiController]
public class ManageChat(IMediator mediator) : ControllerBase
{
    [Authorize]
    [SwaggerOperation(Tags = new[] { "Messages" }, Summary = "Add chat")]
    [HttpPost("/api/chat")]
    public async Task<Result<Guid>> PostEventAsync([FromBody] ManageChatCommand command)
    {
        return await mediator.Send(command);
    }

    public class ManageChatCommand : IRequest<Result<Guid>>
    {
        public string? Name { get; set; }
        public Guid? UserEventId { get; set; }
        public List<string> ParticipantsIds { get; set; }
    }

    public class ManageChatCommandHandler : IRequestHandler<ManageChatCommand, Result<Guid>>
    {
        private readonly DataContext _db;
        private readonly ICurrentUserAccessor _currentUserAccessor;
        public ManageChatCommandHandler(DataContext db, ICurrentUserAccessor currentUserAccessor)
        {
            _db = db;
            _currentUserAccessor = currentUserAccessor;
        }

        public async Task<Result<Guid>> Handle(ManageChatCommand request, CancellationToken cancellationToken)
        {
            var currentUser = await _currentUserAccessor.GetCurrentUser();

            if (request.Name == null)
            {
                var userEvent = await _db.Events
                    .FirstOrDefaultAsync(e => e.Id == request.UserEventId, cancellationToken);

                request.Name = userEvent.EventType.Name;
            }

            var chat = new Chat
            {
                Name = request.Name,
                Participants = new List<ChatParticipant>(),
                Messages = new List<Message>()
            };

            chat.Participants.Add(new ChatParticipant
            {
                UserId = currentUser.Id
            });

            foreach (var userId in request.ParticipantsIds.Distinct())
            {
                if (userId != currentUser.Id)
                {
                    chat.Participants.Add(new ChatParticipant
                    {
                        UserId = userId
                    });
                }
            }

            await _db.AddAsync(chat, cancellationToken);

            await _db.SaveChangesAsync(cancellationToken);

            return Result.Ok(chat.Id);
        }
    }
}