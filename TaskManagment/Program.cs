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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    var logsPath = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(logsPath),
        RequestPath = "/logs",
        ServeUnknownFileTypes = true
    });
}

//Middleware
app.UseMiddleware<InputValidationMiddleware>();

app.UseCors();
app.UseAuthorization();
app.MapControllers();
app.Run();
