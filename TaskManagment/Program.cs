using Microsoft.EntityFrameworkCore;
using TaskManagment.Data;
using TaskManagment.Mapping;
using Microsoft.OpenApi.Models;
using TaskManagment.Services;
using AspNetCoreRateLimit;
using Serilog;
using Microsoft.Extensions.FileProviders;
using TaskManagment.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// Rate-limiting services (use this approach since .net6, not supported for built-in Microsoft.AspNetCore.RateLimiting)
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddInMemoryRateLimiting();

// Swagger
var swaggerConfig = builder.Configuration.GetSection("Swagger");
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc(swaggerConfig["Version"], new OpenApiInfo
    {
        Title = swaggerConfig["Title"],
        Version = swaggerConfig["Version"],
        Description = swaggerConfig["Description"],
        Contact = new OpenApiContact
        {
            Name = swaggerConfig.GetSection("Contact")["Name"],
            Url = new Uri(swaggerConfig.GetSection("Contact")["Url"])
        }
    });

    // Enable XML comments
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Cors
var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod();
        });
});

// Logging
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

// Services
builder.Services.AddScoped<ITaskService, TaskService>(); //Consider to create static class to build all services as project grows.

// Database
builder.Services.AddDbContext<TasksDbContext>(
    opt => opt.UseSqlite(builder.Configuration.GetConnectionString("Db")),
    contextLifetime: ServiceLifetime.Scoped
);

// AutoMapper
builder.Services.AddAutoMapper(typeof(TaskMapping));

// Serilog
builder.Host.UseSerilog();

var app = builder.Build();

// Enable the rate limit middleware
app.UseIpRateLimiting();

app.UseHttpsRedirection();


if (app.Environment.IsDevelopment())
{
    // Configure the HTTP request pipeline.
    app.UseSwagger();
    app.UseSwaggerUI();

    // Expose log file for development only (e.g: http://localhost:7069/logs/20251107.log)
    var logsPath = Path.Combine(Directory.GetCurrentDirectory(), "Logs");

    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(logsPath),
        RequestPath = "/logs",
        ServeUnknownFileTypes = true
    });
}

// Sanitizer
app.UseInputSanitization();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();

