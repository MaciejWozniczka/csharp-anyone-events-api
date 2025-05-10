namespace MobileApp.Host.Infrastructure;

public class ApplicationStart(ILogger<ApplicationStart> logger)
{
    public void Start()
    {
        logger.LogInformation("App started");
    }
}