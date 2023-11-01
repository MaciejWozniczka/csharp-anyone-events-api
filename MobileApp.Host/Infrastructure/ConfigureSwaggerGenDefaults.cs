namespace MobileApp.Host.Infrastructure;

public class ConfigureSwaggerGenDefaults : IConfigureOptions<SwaggerGenOptions>
{
    public void Configure(SwaggerGenOptions options)
    {
        options.OperationFilter<DefaultOperationIdFilter>();
    }
}