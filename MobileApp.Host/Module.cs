using System.Text;

namespace MobileApp.Host;

public interface IModule
{
    void GetServices(IServiceCollection services, ConfigurationManager configuration);
    Task Run(IServiceProvider serviceProvider);
}

public class Module : IModule
{
    public void GetServices(IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddDbContext<DataContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        });

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(cfg =>
            {
                cfg.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidIssuer = configuration["Authentication:Issuer"],
                    ValidAudience = configuration["Authentication:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Authentication:SecretKey"]))
                };
            });

        services.AddAuthorization(o =>
        {
            var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme);
            defaultAuthorizationPolicyBuilder = defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();
            o.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();
        });

        IdentityModelEventSource.ShowPII = false;
        services.AddScoped<IUserService, UserService>();

        services.AddCors(o => o.AddPolicy("default", builder =>
        {
            builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        }));

        services.AddIdentity<User, IdentityRole>(cfg =>
            {
                cfg.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<DataContext>();

        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        services.AddMediatR(typeof(Program));
        services.AddValidatorsFromAssembly(typeof(Program).Assembly);
        services.AddScoped<ICurrentUserAccessor, CurrentUserAccessor>();

        services.Configure<TokenOption>(configuration.GetSection("Authentication"));
        services.Configure<HereOptions>(configuration.GetSection("Here"));

        services.AddControllers();

        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerGenDefaults>();

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "MobileApp", Version = "v1" });
            c.EnableAnnotations();

            var jwtSecurityScheme = new OpenApiSecurityScheme
            {
                BearerFormat = "JWT",
                Name = "JWT Authentication",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = JwtBearerDefaults.AuthenticationScheme,
                Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",

                Reference = new OpenApiReference
                {
                    Id = JwtBearerDefaults.AuthenticationScheme,
                    Type = ReferenceType.SecurityScheme
                }
            };

            c.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {{ jwtSecurityScheme, Array.Empty<string>() }});
        });
    }

    public async Task Run(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        await scope.ServiceProvider.GetRequiredService<DataContext>().Database.MigrateAsync();
    }
}