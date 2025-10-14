using Timora.Api.Extensions;
using FirebaseAdmin;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerServices();
builder.Services.AddEntityFrameworkServices(builder.Configuration);
builder.Services.AddFirebaseAuthentication(builder.Configuration);

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
app.UseAuthentication();
app.UseFirebaseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
