namespace MobileApp.Host.Locations;

[ApiController]
public class CheckLocation : ControllerBase
{
    private readonly IMediator _mediator;
    public CheckLocation(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [SwaggerOperation(Tags = ["Events"], Summary = "Check location")]
    [HttpGet("/api/location/{address}")]
    public async Task<Result<HereGeocode>> CheckLocationAsync(string address)
    {
        return await _mediator.Send(new CheckLocationQuery(address));
    }

    public class CheckLocationQuery : IRequest<Result<HereGeocode>>
    {
        public string Address { get; set; }
        public CheckLocationQuery(string address)
        {
            Address = address; ;
        }
    }

    public class CheckLocationCommandHandler : IRequestHandler<CheckLocationQuery, Result<HereGeocode>>
    {
        private readonly HereOptions _hereOptions;
        public CheckLocationCommandHandler(IOptions<HereOptions> hereOptions)
        {
            _hereOptions = hereOptions.Value;
        }
        public async Task<Result<HereGeocode>> Handle(CheckLocationQuery request, CancellationToken cancellationToken)
        {
            var result = await _hereOptions.Url
                .AppendPathSegment("geocode")
                .SetQueryParams(new
                {
                    q = request.Address,
                    apiKey = _hereOptions.ApiKey,
                })
                .GetJsonAsync<HereGeocode>(cancellationToken);

            return result != null ? Result.Ok(result) : Result.NotFound<HereGeocode>("Address not found");
        }
    }
}