using AspNetCoreRateLimit;
using Microsoft.Extensions.FileProviders;
using Serilog;
using TaskManagment.Extensions;
using TaskManagment.Middlewares;

var builder = WebApplication.CreateBuilder(args);

//Logging
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

//Register all app services
builder.Services.AddProjectServices(builder.Configuration);

var app = builder.Build();

app.UseIpRateLimiting();
app.UseHttpsRedirection();

app.UseSwagger(); // for demo purpose: temporary moved outside IsDevelopment block
app.UseSwaggerUI(); // for demo purpose: temporary moved outside IsDevelopment block

if (app.Environment.IsDevelopment())
{
    var logsPath = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(logsPath),
        RequestPath = "/logs",
        ServeUnknownFileTypes = true
    });
}

//Middleware
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseMiddleware<InputValidationMiddleware>();

app.UseCors();
app.UseAuthorization();
app.MapControllers();
app.Run();
