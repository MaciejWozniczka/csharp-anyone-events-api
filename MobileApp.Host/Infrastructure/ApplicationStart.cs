namespace MobileApp.Host.Infrastructure;

public class ApplicationStart
{
    private readonly ILogger<ApplicationStart> _logger;
    public ApplicationStart(ILogger<ApplicationStart> logger)
    {
        _logger = logger;
    }

    public void Start()
    {
        _logger.LogInformation("App started");
    }
}