using AnyOneApi.Host.Infrastructure;

namespace AnyOneApi.Host.Locations;

[ApiController]
public class CheckDistance(IMediator mediator) : ControllerBase
{
    [Authorize]
    [SwaggerOperation(Tags = ["Events"], Summary = "Check distance")]
    [HttpGet("/api/location/{address}/distance/{destination}")]
    public async Task<Result<CheckDistanceDto>> CheckDistanceAsync(
        /// <summary>Adres początkowy</summary>
        string address, 
        /// <summary>Adres docelowy</summary>
        string destination)
    {
        return await mediator.Send(new CheckDistanceQuery(address, destination));
    }

    public class CheckDistanceQuery(string address, string destination) : IRequest<Result<CheckDistanceDto>>
    {
        /// <summary>Adres początkowy</summary>
        public string Address { get; set; } = address;
        /// <summary>Adres docelowy</summary>
        public string Destination { get; set; } = destination;
    }

    public class CheckDistanceDto
    {
        /// <summary>Odległość w metrach</summary>
        public double Distance { get; set; }
    }

    public class CheckDistanceCommandHandler(IOptions<HereOptions> hereOptions)
        : IRequestHandler<CheckDistanceQuery, Result<CheckDistanceDto>>
    {
        private readonly HereOptions _hereOptions = hereOptions.Value;

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