using Resend;
using Timora.Api.Repositories;
using Timora.Api.Services;

namespace Timora.Api.Extensions;

/// <summary>
/// Extension methods for registering application dependencies.
/// </summary>
public static class DependencyInjectionExtensions
{
    /// <summary>
    /// Registers all repository implementations.
    /// </summary>
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IHolidayRequestRepository, HolidayRequestRepository>();
        services.AddScoped<ICompanyRepository, CompanyRepository>();
        services.AddScoped<INoticeRepository, NoticeRepository>();
        services.AddScoped<IDocumentRepository, DocumentRepository>();
        return services;
    }

    /// <summary>
    /// Registers all service implementations.
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IHolidayRequestService, HolidayRequestService>();
        services.AddScoped<ICompanyService, CompanyService>();
        services.AddScoped<INoticeService, NoticeService>();
        services.AddScoped<IDocumentService, DocumentService>();
        return services;
    }

    /// <summary>
    /// Registers email service with Resend client.
    /// </summary>
    public static IServiceCollection AddEmailServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions();
        services.AddHttpClient<ResendClient>();
        services.Configure<ResendClientOptions>(o =>
        {
            o.ApiToken = configuration["Resend:ApiKey"]!;
        });
        services.AddTransient<IResend, ResendClient>();
        services.AddScoped<IEmailService, EmailService>();
        return services;
    }

    /// <summary>
    /// Configures CORS with allowed origins.
    /// </summary>
    public static IServiceCollection AddCorsPolicy(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowedOrigins", policy =>
                policy.WithOrigins(
                        "https://brave-plant-0f5043a03.1.azurestaticapps.net",
                        "http://localhost:4200")
                      .AllowAnyHeader()
                      .AllowAnyMethod());
        });
        return services;
    }
}
