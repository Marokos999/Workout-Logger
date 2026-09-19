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

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.Services.AddOpenApi();
if (!builder.Environment.IsEnvironment("Testing"))
    builder.AddNpgsqlDbContext<AppDbContext>("workoutdb");
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<WorkoutService>();
builder.Services.AddScoped<ExerciseService>();
builder.Services.AddScoped<ProgressService>();


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

if (!app.Environment.IsEnvironment("Testing"))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
    await DataSeeder.SeedExerciseAsync(db);
}
app.MapOpenApi();
app.MapScalarApiReference();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapDefaultsEndpoint();
app.MapAuthEndpoints();
app.MapWorkoutEndpoints();
app.MapExerciseEndpoints();
app.MapProgressEndpoints();

app.Run();

public partial class Program { }
