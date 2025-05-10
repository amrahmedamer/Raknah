using FluentValidation;
using FluentValidation.AspNetCore;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.IdentityModel.Tokens;
using Raknah.Authentications;
using Raknah.Middelware;
using Raknah.Persistence;
using Raknah.Setting;
using Serilog;
using System.Reflection;
using System.Text;

namespace Raknah;

public static class DependencyInjections
{
    public static IServiceCollection RegisterServices(this IServiceCollection services, WebApplicationBuilder builder)
    {
        services.Configure<EmailSetting>(builder.Configuration.GetSection("EmailSetting"));

        services
            .AddProblemDetails()
            .AddExceptionHandler<GlobalExceptionHandler>()
           .AddHttpContextAccessor()
           .AddHttpClient()
           .AddEndpointsApiExplorer()
           .AddSwaggerGen()
           .AddControllers();

        services
           .AddHangFireConfig(builder)
            .SerilogConfig(builder)
            .DbContextConfig(builder)
            .FluentValidationConfig()
            .MapsterConfig()
            .AuthConfig();

        services
            .AddScoped<IAuthService, AuthService>()
            .AddScoped<IUserService, UserService>()
            .AddScoped<IJwtProvider, JwtProvider>()
            .AddScoped<IParkingSpotServices, ParkingSpotServices>()
            .AddScoped<IReservationServices, ResservationService>()
            .AddScoped<IEmailSender, EmailService>()
            .AddTransient<IExceptionHandler, GlobalExceptionHandler>();


        return services;
    }
    public static IServiceCollection DbContextConfig(this IServiceCollection services, WebApplicationBuilder builder)
    {


        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
        });

        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        return services;
    }
    public static IServiceCollection MapsterConfig(this IServiceCollection services)
    {
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(Assembly.GetExecutingAssembly());
        services.AddSingleton(config);

        return services;
    }


    public static IServiceCollection AuthConfig(this IServiceCollection services)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = false;
            options.TokenValidationParameters = new()
            {

                ValidateIssuer = true,
                ValidIssuer = "Raknah",
                ValidateAudience = true,
                ValidAudience = "Users",
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("R2gMz5peB4b6C4du9q8ZvHh6UL1Z8q3K"))
            };
        });

        return services;
    }



    public static IServiceCollection FluentValidationConfig(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation()
            .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        return services;
    }

    public static IServiceCollection AddHangFireConfig(this IServiceCollection services, WebApplicationBuilder builder)
    {
        // Add Hangfire services.
        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnection")));

        // Add the processing server as IHostedService
        services.AddHangfireServer();
        return services;
    }
    public static IServiceCollection SerilogConfig(this IServiceCollection services, WebApplicationBuilder builder)
    {

        builder.Host.UseSerilog((context, configuration) =>
            configuration.ReadFrom.Configuration(context.Configuration)
        );

        return services;
    }

}
