namespace MobileApp.Host.Ping;

[ApiController]
public class GetPing : ControllerBase
{
    [SwaggerOperation(Tags = new[] { "Ping" }, Summary = "Check service status")]
    [HttpGet("/api/ping")]
    public IActionResult Ping()
    {
        return Ok("Pong");
    }
}