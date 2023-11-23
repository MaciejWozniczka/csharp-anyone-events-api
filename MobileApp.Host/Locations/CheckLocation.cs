namespace MobileApp.Host.Locations
{
    [ApiController]
    public class CheckLocation : ControllerBase
    {
        private readonly IMediator _mediator;
        public CheckLocation(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize]
        [SwaggerOperation(Tags = new[] { "Events" }, Summary = "Check location")]
        [HttpGet("/api/location")]
        public async Task<Result<List<HereGeocode>>> CheckLocationAsync(string address)
        {
            return await _mediator.Send(new CheckLocationQuery(address));
        }

        public class CheckLocationQuery : IRequest<Result<List<HereGeocode>>>
        {
            public CheckLocationQuery(string address)
            {
                Address = address;
            }
            public string Address { get; set; }
        }

        public class CheckLocationCommandHandler : IRequestHandler<CheckLocationQuery, Result<List<HereGeocode>>>
        {
            private readonly HereOptions _hereOptions;
            public CheckLocationCommandHandler(IOptions<HereOptions> hereOptions)
            {
                _hereOptions = hereOptions.Value;
            }

            public async Task<Result<List<HereGeocode>>> Handle(CheckLocationQuery request, CancellationToken cancellationToken)
            {
                var result = await _hereOptions.Url
                    .AppendPathSegment("geocode")
                    .SetQueryParams(new
                    {
                        q = request.Address,
                        apiKey = _hereOptions.ApiKey,
                    })
                    .GetJsonAsync<List<HereGeocode>>(cancellationToken);

                return result != null ? Result.Ok(result) : Result.NotFound<List<HereGeocode>>("Address not found");
            }
        }
    }
}