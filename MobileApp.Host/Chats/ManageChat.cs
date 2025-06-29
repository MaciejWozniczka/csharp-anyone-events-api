namespace MobileApp.Host.Chats;

[ApiController]
public class ManageChat(IMediator mediator) : ControllerBase
{
    [Authorize]
    [SwaggerOperation(Tags = ["Messages"], Summary = "Add chat")]
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

    public class ManageChatCommandHandler(DataContext db, ICurrentUserAccessor currentUserAccessor) : IRequestHandler<ManageChatCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(ManageChatCommand request, CancellationToken cancellationToken)
        {
            var currentUser = await currentUserAccessor.GetCurrentUser();

            if (request.Name == null)
            {
                var userEvent = await db.Events
                    .Include(userEvent => userEvent.EventType)
                    .FirstOrDefaultAsync(e => e.Id == request.UserEventId, cancellationToken);

                request.Name = userEvent.EventType.Name;
            }

            var chat = new Chat
            {
                Name = request.Name,
                Participants =
                [
                    new ChatParticipant
                    {
                        UserId = currentUser.Id
                    }

                ],
                Messages = []
            };

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

            await db.AddAsync(chat, cancellationToken);

            await db.SaveChangesAsync(cancellationToken);

            return Result.Ok(chat.Id);
        }
    }
}