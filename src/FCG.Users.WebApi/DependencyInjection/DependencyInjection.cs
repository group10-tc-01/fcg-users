using Asp.Versioning;
using FCG.Users.Infrastructure.SqlServer.Persistance;
using FCG.Users.WebApi.Filters;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Diagnostics.CodeAnalysis;
using Serilog.Sinks.ElmahIo;

namespace FCG.Users.WebApi.DependencyInjection
{
    [ExcludeFromCodeCoverage]
    public static class DependencyInjection
    {
        public static IServiceCollection AddWebApi(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddVersioning();
            services.AddFilters();
            services.AddHealthChecks().AddDbContextCheck<FcgUserDbContext>();
            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddSwaggerConfiguration(configuration);
            services.AddSerilogLogging(configuration);
            services.AddElmahIo(configuration);

            return services;
        }

        private static void AddSwaggerConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var imageVersion = configuration["IMAGE_VERSION"];
            var description = string.IsNullOrWhiteSpace(imageVersion)
                ? "Image: unknown"
                : $"Image: {imageVersion}";

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "FCG.Users - V1",
                    Version = "v1.0",
                    Description = description
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme.
                      Enter 'Bearer' [space] and then your token in the text input below.
                      Example: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });
            });
        }

        private static void AddVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            }).AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });
        }

        private static void AddFilters(this IServiceCollection services)
        {
            services.AddMvc(options =>
            {
                options.Filters.Add<TrimStringsActionFilter>();
            });
        }

        private static void AddSerilogLogging(this IServiceCollection services, IConfiguration configuration)
        {
            var seqUrl = configuration["Serilog:WriteTo:1:Args:serverUrl"]
                         ?? configuration["Serilog:SeqUrl"]
                         ?? "http://localhost:5341";

            var loggerConfig = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithProperty("Application", "FCG.Users")
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{CorrelationId}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.Seq(seqUrl);

            var apiKey = configuration["ElmahIo:ApiKey"];
            var logIdRaw = configuration["ElmahIo:LogId"];

            if (!string.IsNullOrEmpty(apiKey) && Guid.TryParse(logIdRaw, out var logId))
            {
                loggerConfig.WriteTo.ElmahIo(new ElmahIoSinkOptions(apiKey, logId)
                {
                    MinimumLogEventLevel = Serilog.Events.LogEventLevel.Warning
                });
            }

            Log.Logger = loggerConfig.CreateLogger();

            Log.Information("Starting FCG.Users application");
            Log.Information("Seq URL configured: {SeqUrl}", seqUrl);
            Log.Information("Environment: {Environment}", configuration["ASPNETCORE_ENVIRONMENT"]);

            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddSerilog();
            });
        }

        private static void AddElmahIo(this IServiceCollection services, IConfiguration configuration)
        {
            var apiKey = configuration["ElmahIo:ApiKey"];
            var logIdRaw = configuration["ElmahIo:LogId"];

            if (!string.IsNullOrEmpty(apiKey) && Guid.TryParse(logIdRaw, out var logId))
            {
                services.AddElmahIo(options =>
                {
                    options.ApiKey = apiKey;
                    options.LogId = logId;
                });
            }
        }
    }
}
