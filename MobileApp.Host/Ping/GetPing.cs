namespace MobileApp.Host.Ping;

[ApiController]
public class GetPing(ILogger<GetPing> logger) : ControllerBase
{
    [SwaggerOperation(Tags = ["Ping"], Summary = "Check service status")]
    [HttpGet("/api/ping")]
    public IActionResult Ping()
    {
        logger.LogInformation("Service status check");
        return Ok("Pong");
    }
}