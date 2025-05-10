namespace MobileApp.Host.Locations;

[ApiController]
public class CheckLocation(IMediator mediator) : ControllerBase
{
    [Authorize]
    [SwaggerOperation(Tags = ["Events"], Summary = "Check location")]
    [HttpGet("/api/location/{address}")]
    public async Task<Result<HereGeocode>> CheckLocationAsync(string address)
    {
        return await mediator.Send(new CheckLocationQuery(address));
    }

    public class CheckLocationQuery(string address) : IRequest<Result<HereGeocode>>
    {
        public string Address { get; set; } = address;
    }

    public class CheckLocationCommandHandler(IOptions<HereOptions> hereOptions)
        : IRequestHandler<CheckLocationQuery, Result<HereGeocode>>
    {
        private readonly HereOptions _hereOptions = hereOptions.Value;

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