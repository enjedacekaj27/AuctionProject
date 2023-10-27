using Contracts;
using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Repository;

namespace AuctionProject.Extensions;

public static class ServiceExtensions
{
    public static void ConfigureCors(this IServiceCollection services) =>
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", builder =>
                builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
        });

    public static void ConfigureIISIntegration(this IServiceCollection services) =>
        services.Configure<IISOptions>(options =>
        {

        });



    public static void ConfigureSqlContext(this IServiceCollection services, IConfiguration configuration) =>
        services.AddDbContext<AuctionProjectDbContext>(opts =>
            opts.UseSqlServer(configuration.GetConnectionString("AuctionAppConnectionString"), b => b.MigrationsAssembly("AuctionProject")));



    public static void ConfigureRepositoryManager(this IServiceCollection services) =>
      services.AddScoped<IRepositoryManager, RepositoryManager>();

    public static void ConfigureSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(s =>
        {
            s.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "AUCTION API",
                Version = "v1",
                Description = "AUCTION API by Enjeda",
            });

            s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Name = "Authorization",
                Description = "Please insert JWT with Bearer into field",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            s.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Name = "Bearer",
                    },
                    new List<string>()
                }
            });
        });
    }
}
