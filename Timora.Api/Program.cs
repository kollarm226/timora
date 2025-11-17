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

app.UseHttpsRedirection();
app.UseCors("AllowedOrigins");
app.UseAuthentication();
app.UseFirebaseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
