using Microsoft.EntityFrameworkCore;
using TaskManagment.Data;
using TaskManagment.Mapping;
using Microsoft.OpenApi.Models;
using TaskManagment.Services;
using AspNetCoreRateLimit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// Rate-limiting services (use this approach since using .net6, not supported for built-in Microsoft.AspNetCore.RateLimiting)
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

    // Enable XML comments (optional but recommended)
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

// Services
builder.Services.AddScoped<ITaskService, TaskService>();

// Database
builder.Services.AddDbContext<TasksDbContext>(
    opt => opt.UseSqlite(builder.Configuration.GetConnectionString("Db")),
    contextLifetime: ServiceLifetime.Scoped
);

// AutoMapper
builder.Services.AddAutoMapper(typeof(TaskMapping));

var app = builder.Build();

// Enable the rate limit middleware
app.UseIpRateLimiting();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();

