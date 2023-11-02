namespace MobileApp.Host.Ping;

[ApiController]
public class GetPing : ControllerBase
{
    private readonly ILogger<GetPing> _logger;
    public GetPing(ILogger<GetPing> logger)
    {
        _logger = logger;
    }

    [SwaggerOperation(Tags = new[] { "Ping" }, Summary = "Check service status")]
    [HttpGet("/api/ping")]
    public IActionResult Ping()
    {
        _logger.LogInformation("Service status check");
        return Ok("Pong");
    }
}