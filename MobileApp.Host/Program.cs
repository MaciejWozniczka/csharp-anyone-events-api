using AnyOneApi.Host;
using AnyOneApi.Host.Fakers;
using AnyOneApi.Host.Messages;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var configuration = builder.Configuration;

GlobalConfiguration.Configuration
    .UsePostgreSqlStorage(cn => cn.UseNpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
    .UseSerilogLogProvider();

services.AddLogging(loggingBuilder =>
{
    loggingBuilder.ClearProviders();
    loggingBuilder.AddSerilog(dispose: true);
});

var options = new PostgreSqlStorageOptions
{
    CountersAggregateInterval = TimeSpan.FromHours(24),
    JobExpirationCheckInterval = TimeSpan.FromHours(1)
};

services.AddHangfire(c => c
    .UseSerilogLogProvider()
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UsePostgreSqlStorage(cn =>
        cn.UseNpgsqlConnection(configuration.GetConnectionString("DefaultConnection")), options)
    .UseFilter(new AutomaticRetryAttribute { Attempts = 1 }));

services.AddHangfireServer(serverOptions =>
{
    serverOptions.Queues = [ "default", "events" ];
    serverOptions.WorkerCount = 4;
});

services.AddSignalR();

builder.Host.UseSerilog((host, log) =>
{
    log.Enrich.FromLogContext();
    log.MinimumLevel.Warning();
    log.MinimumLevel.Override("AnyOneApi", LogEventLevel.Information);
    log.WriteTo.File(
        Path.Combine("Logs", "log.txt"),
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [X-Request-ID: {RequestId}] {Message:lj}{NewLine}{Exception}"
    );
    log.WriteTo.Console();
});

new Module().GetServices(services, configuration);

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
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AnyOneApi v1"));

app.UseHangfireDashboard();

app.MapHub<ChatHub>("/chathub");

foreach (var job in JobStorage.Current.GetConnection().GetRecurringJobs())
{
    RecurringJob.RemoveIfExists(job.Id);
}

new Logger<Program>(new LoggerFactory()).LogInformation("Anyone App started!");

RecurringJob.AddOrUpdate<IFakerService>("events", "events", s => s.CreateFakeEvents(CancellationToken.None), Cron.Daily(6));

await new Module().Run(app.Services);

app.Run();