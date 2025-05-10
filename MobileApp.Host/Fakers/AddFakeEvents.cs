namespace MobileApp.Host.Fakers;

[ApiController]
public class AddFakeEvents(IMediator mediator) : ControllerBase
{
    [SwaggerOperation(Tags = ["Faker"], Summary = "Add fake services")]
    [HttpPost("/api/events/fake")]
    public async Task<Result> AddFakeEventsAsync([FromBody] AddFakeEventsQuery addFakeEventsRequestBody)
    {
        return await mediator.Send(new AddFakeEventsQuery());
    }

    public class AddFakeEventsQuery : IRequest<Result>
    {
    }

    public class AddFakeEventsQueryHandler(IFakerService fakerService) : IRequestHandler<AddFakeEventsQuery, Result>
    {
        public async Task<Result> Handle(AddFakeEventsQuery request, CancellationToken cancellationToken)
        {
            await fakerService.CreateFakeEvents(cancellationToken);

            return Result.Ok();
        }
    }
}