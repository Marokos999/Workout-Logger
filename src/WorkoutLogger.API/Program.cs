using Microsoft.EntityFrameworkCore;
using WorkoutLogger.API.Infrastructure;
using WorkoutLogger.API.Features.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Scalar.AspNetCore;
using WorkoutLogger.API.Features.Workouts;
using WorkoutLogger.API.Features.Exercises;
using WorkoutLogger.API.Features.Progress;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.Services.AddOpenApi();
if (!builder.Environment.IsEnvironment("Testing"))
    builder.AddNpgsqlDbContext<AppDbContext>("workoutdb");
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<WorkoutService>();
builder.Services.AddScoped<ExerciseService>();
builder.Services.AddScoped<ProgressService>();

if (!builder.Environment.IsEnvironment("Testing"))
    builder.Services.AddHostedService<MigrationHostedService>();


builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("auth", ctx => RateLimitPartition.GetFixedWindowLimiter(
        ctx.Connection.RemoteIpAddress?.ToString() ?? "anon",
        _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 10,
            Window = TimeSpan.FromMinutes(1),
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            QueueLimit = 0
        }));
    options.RejectionStatusCode = 429;
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseExceptionHandler(errApp => errApp.Run(async ctx =>
{
    ctx.Response.StatusCode = 500;
    ctx.Response.ContentType = "application/json";
    var feature = ctx.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
    var message = app.Environment.IsDevelopment() && feature?.Error is not null
        ? feature.Error.Message
        : "Interna greška servera.";
    await ctx.Response.WriteAsJsonAsync(new { error = message });
}));

app.MapOpenApi();
app.MapScalarApiReference();

app.UseHttpsRedirection();

app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapDefaultsEndpoint();
app.MapAuthEndpoints();
app.MapWorkoutEndpoints();
app.MapExerciseEndpoints();
app.MapProgressEndpoints();

app.Run();

public partial class Program { }
