using FirebaseAdmin;
using Timora.Api.Extensions;
using Timora.Api.Repositories;
using Timora.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerServices();
builder.Services.AddEntityFrameworkServices(builder.Configuration);
builder.Services.AddFirebaseAuthentication(builder.Configuration);
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowedOrigins", policy =>
        policy.WithOrigins(
                "https://brave-plant-0f5043a03.1.azurestaticapps.net",
                "http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// Register Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IHolidayRequestRepository, HolidayRequestRepository>();
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<INoticeRepository, NoticeRepository>();

// Register Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IHolidayRequestService, HolidayRequestService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<INoticeService, NoticeService>();

var app = builder.Build();

// Ensure database is created (for Azure deployment)
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<Timora.Data.Data.TimoraDbContext>();
    try
    {
        dbContext.Database.EnsureCreated();
        app.Logger.LogInformation("Database ensured/created at: {ConnectionString}", 
            builder.Configuration.GetConnectionString("DefaultConnection"));
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Failed to ensure database creation");
    }
}

// Log Firebase initialization status
if (FirebaseApp.DefaultInstance != null)
{
    app.Logger.LogInformation("Firebase Admin SDK initialized successfully");
}
else
{
    app.Logger.LogWarning("Firebase Admin SDK not initialized - authentication will fail");
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerServices();
}

// Global exception handler - return detailed errors for debugging
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Unhandled exception for {Method} {Path}", 
            context.Request.Method, context.Request.Path);
        
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new 
        { 
            error = "Internal Server Error",
            message = ex.Message,
            stackTrace = ex.StackTrace,
            innerException = ex.InnerException?.Message
        });
    }
});

app.UseHttpsRedirection();
app.UseCors("AllowedOrigins");
app.UseAuthentication();
app.UseFirebaseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
