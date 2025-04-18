using System.Diagnostics;
using MongoDB.Driver;
using WritingServer.Services;
using WritingServer.Utils;

var startTime = DateTime.UtcNow;
var builder = WebApplication.CreateBuilder(args);

// Load .env file
DotNetEnv.Env.Load();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddControllersWithViews();
builder.Services.AddHealthChecks();

// Add CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClientApp", policy =>
    {
        policy.AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin();
    });
});

// Add MongoDB client
builder.Services.AddSingleton<IMongoClient>(sp => 
{
    var connectionString = Environment.GetEnvironmentVariable("MONGODB_URI");
    if (string.IsNullOrEmpty(connectionString))
    {
        throw new InvalidOperationException("MongoDB connection string not found in environment variables");
    }
    return new MongoClient(connectionString);
});

// Add MongoDB database
builder.Services.AddSingleton(sp => 
{
    var client = sp.GetRequiredService<IMongoClient>();
    var url = new MongoUrl(Environment.GetEnvironmentVariable("MONGODB_URI"));
    return client.GetDatabase(url.DatabaseName);
});

// Add our services
builder.Services.AddSingleton<IDiscordNotificationService, DiscordNotificationService>();
builder.Services.AddScoped<IUserManagementService, UserManagementService>();
builder.Services.AddSingleton<IQuestionManagementService, QuestionManagementService>();
builder.Services.AddSingleton<IAdminTokenService, AdminTokenService>();
builder.Services.AddSingleton<IUserTokenService, UserTokenService>();

// Configuration
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);
builder.Configuration.AddEnvironmentVariables();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapHealthChecks("/health");

// Use CORS before routing
app.UseCors("AllowClientApp");

var startupTime = DateTime.UtcNow - startTime;
var process = Process.GetCurrentProcess();
var memoryUsageMb = process.WorkingSet64 / (1024 * 1024);

var discordService = app.Services.GetRequiredService<IDiscordNotificationService>();
_ = discordService.SendStartupNotification(startupTime, memoryUsageMb);

app.UseGlobalExceptionHandler();
app.MapControllers();
app.Run();

