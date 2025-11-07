using AspNetCoreRateLimit;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TaskManagment.Data;
using TaskManagment.Mapping;
using TaskManagment.Services;

namespace TaskManagment.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddProjectServices(this IServiceCollection services, IConfiguration configuration)
        {
            //Controllers & Endpoints
            services.AddControllers();
            services.AddEndpointsApiExplorer();

            //Rate Limiting
            services.AddMemoryCache();
            services.Configure<IpRateLimitOptions>(configuration.GetSection("IpRateLimiting"));
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
            services.AddInMemoryRateLimiting();

            //Swagger Configuration
            var swaggerConfig = configuration.GetSection("Swagger");
            services.AddSwaggerGen(c =>
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

                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }
            });

            //CORS
            var allowedOrigins = configuration.GetSection("AllowedOrigins").Get<string[]>();
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.WithOrigins(allowedOrigins)
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            //Database
            services.AddDbContext<TasksDbContext>(
                opt => opt.UseSqlite(configuration.GetConnectionString("Db")),
                contextLifetime: ServiceLifetime.Scoped
            );

            //AutoMapper
            services.AddAutoMapper(typeof(TaskMapping));

            //Application Business Services
            services.AddScoped<ITaskService, TaskService>();

            return services;
        }
    }
}
