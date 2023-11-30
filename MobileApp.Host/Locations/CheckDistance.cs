namespace MobileApp.Host.Locations
{
    [ApiController]
    public class CheckDistance : ControllerBase
    {
        private readonly IMediator _mediator;
        public CheckDistance(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize]
        [SwaggerOperation(Tags = new[] { "Events" }, Summary = "Check distance")]
        [HttpGet("/api/location/{address}/distance/{destination}")]
        public async Task<Result<CheckDistanceDto>> CheckDistanceAsync(string address, string destination)
        {
            return await _mediator.Send(new CheckDistanceQuery(address, destination));
        }

        public class CheckDistanceQuery : IRequest<Result<CheckDistanceDto>>
        {
            public string Address { get; set; }
            public string Destination { get; set; }
            public CheckDistanceQuery(string address, string destination)
            {
                Address = address; ;
                Destination = destination;
            }
        }

        public class CheckDistanceDto
        {
            public double Distance { get; set; }
        }

        public class CheckDistanceCommandHandler : IRequestHandler<CheckDistanceQuery, Result<CheckDistanceDto>>
        {
            private readonly HereOptions _hereOptions;
            public CheckDistanceCommandHandler(IOptions<HereOptions> hereOptions)
            {
                _hereOptions = hereOptions.Value;
            }
            public async Task<Result<CheckDistanceDto>> Handle(CheckDistanceQuery request, CancellationToken cancellationToken)
            {
                var address = await _hereOptions.Url
                    .AppendPathSegment("geocode")
                    .SetQueryParams(new
                    {
                        q = request.Address,
                        apiKey = _hereOptions.ApiKey,
                    })
                    .GetJsonAsync<HereGeocode>(cancellationToken);

                if (address == null)
                {
                    return Result.NotFound<CheckDistanceDto>("Address not found");
                }

                var destination = await _hereOptions.Url
                    .AppendPathSegment("geocode")
                    .SetQueryParams(new
                    {
                        q = request.Destination,
                        apiKey = _hereOptions.ApiKey,
                    })
                    .GetJsonAsync<HereGeocode>(cancellationToken);

                if (destination == null)
                {
                    return Result.NotFound<CheckDistanceDto>("Destination not found");
                }

                var addressCoordinate = new GeoCoordinate(address.Items.FirstOrDefault().Position.Lat.Value, address.Items.FirstOrDefault().Position.Lng.Value);
                var destinationCoordinate = new GeoCoordinate(destination.Items.FirstOrDefault().Position.Lat.Value, destination.Items.FirstOrDefault().Position.Lng.Value);

                var distance = addressCoordinate.GetDistanceTo(destinationCoordinate);

                return Result.Ok(new CheckDistanceDto { Distance = distance });
            }
        }
    }
}