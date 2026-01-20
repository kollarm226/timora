using System.Text.Json.Serialization;
using Timora.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Prevent circular reference errors when serializing entities with navigation properties
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddOpenApi();
builder.Services.AddSwaggerServices();
builder.Services.AddEntityFrameworkServices(builder.Configuration);
builder.Services.AddFirebaseAuthentication(builder.Configuration);
builder.Services.AddCorsPolicy();
builder.Services.AddRepositories();
builder.Services.AddApplicationServices();
builder.Services.AddEmailServices(builder.Configuration);

var app = builder.Build();

// Initialize database and log status
app.EnsureDatabase(builder.Configuration);
app.LogFirebaseStatus();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerServices();
}

app.UseGlobalExceptionHandler();
app.UseHttpsRedirection();
app.UseCors("AllowedOrigins");
app.UseAuthentication();
app.UseFirebaseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
