namespace MobileApp.Host.Fakers;

[ApiController]
public class AddFakeEvents : ControllerBase
{
    private readonly IMediator _mediator;
    public AddFakeEvents(IMediator mediator)
    {
        _mediator = mediator;
    }

    [SwaggerOperation(Tags = new[] { "Faker" }, Summary = "Add fake services")]
    [HttpPost("/api/events/fake")]
    public async Task<Result> AddFakeEventsAsync([FromBody] AddFakeEventsQuery addFakeEventsRequestBody)
    {
        return await _mediator.Send(new AddFakeEventsQuery());
    }

    public class AddFakeEventsQuery : IRequest<Result>
    {
    }

    public class AddFakeEventsQueryHandler : IRequestHandler<AddFakeEventsQuery, Result>
    {
        private readonly IFakerService _fakerService;
        public AddFakeEventsQueryHandler(IFakerService fakerService)
        {
            _fakerService = fakerService;
        }

        public async Task<Result> Handle(AddFakeEventsQuery request, CancellationToken cancellationToken)
        {
            await _fakerService.CreateFakeEvents(cancellationToken);

            return Result.Ok();
        }
    }
}