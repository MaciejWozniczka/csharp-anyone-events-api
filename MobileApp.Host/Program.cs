using Hangfire;
using Microsoft.AspNetCore.Diagnostics;
using Serilog.Context;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var configuration = builder.Configuration;

services.AddLogging(loggingBuilder =>
{
    loggingBuilder.ClearProviders();
    loggingBuilder.AddSerilog(dispose: true);
});

builder.Host.UseSerilog((host,
    log) =>
{
    log.Enrich.FromLogContext();
    log.MinimumLevel.Warning();
    log.MinimumLevel.Override("TpaFinder", LogEventLevel.Information);
    log.WriteTo.File(
        Path.Combine("Logs", "log.txt"),
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [X-Request-ID: {RequestId}] {Message:lj}{NewLine}{Exception}"
    );
    log.WriteTo.Console();
});

new MobileApp.Host.Module().GetServices(services, configuration);

services.AddSpaStaticFiles(c =>
{
    c.RootPath = "ClientApp/dist";
});

var app = builder.Build();

app.Use(async (context, next) =>
{
    string requestId = context.TraceIdentifier;

    using (LogContext.PushProperty("RequestId", requestId))
    {
        context.Response.Headers.Add("X-Request-ID", requestId);

        await next.Invoke();
    }
});

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseCors("default");
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
        var exception = exceptionHandlerFeature.Error;

        Log.Logger.Error(exception, "Unhandled Exception");
    });
});

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHangfireDashboard();
});

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TpaFinder v1"));

app.UseStaticFiles();
app.UseSpaStaticFiles();
app.UseSpa(spa =>
{
    spa.Options.SourcePath = "ClientApp";

    if (app.Environment.IsDevelopment())
    {
        spa.UseProxyToSpaDevelopmentServer("http://localhost:5173/");
    }
});

app.UseHangfireDashboard();
app.UseHangfireServer(new BackgroundJobServerOptions());

GlobalConfiguration.Configuration.UseSerilogLogProvider();

BackgroundJob.Enqueue<ApplicationStart>(s => s.Start());

new MobileApp.Host.Module().Run(app.Services);

app.Run();