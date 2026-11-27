using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Security.Claims;
using System.Text;
using TheatreMasterService.Api.Routing;
using TheatreMasterService.Api.Workers;

namespace TheatreMasterService.Api.Extensions
{
    /// <summary>
    /// Provides extension methods for registering API-level controllers, JWT authentication, and Swagger documentation.
    /// </summary>
    public static class ApiServiceCollectionExtensions
    {
        /// <summary>
        /// Adds controllers and API explorer endpoints.
        /// </summary>
        public static IServiceCollection AddApiControllers(this IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                options.Conventions.Add(new RouteTokenTransformerConvention(new KebabCaseTransformer()));
            });
            services.AddEndpointsApiExplorer();

            // Background Workers (RabbitMQ Subscriber)
            services.AddHostedService<BookingConfirmedConsumer>();

            return services;
        }

        /// <summary>
        /// Configures and adds JWT-based token authentication and authorization.
        /// </summary>
        public static IServiceCollection AddApiJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JwtSettings");
            var key = Encoding.ASCII.GetBytes(jwtSettings.GetValue<string>("Key") ?? throw new InvalidOperationException("JWT Key is not configured."));

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.GetValue<string>("Issuer"),
                    ValidateAudience = true,
                    ValidAudience = jwtSettings.GetValue<string>("Audience"),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    RoleClaimType = ClaimTypes.Role
                };
            });

            services.AddAuthorization();
            return services;
        }

        /// <summary>
        /// Configures Swagger documentation and Bearer token security requirements.
        /// </summary>
        public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new() { Title = "Theatre Master Service", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter: Bearer {your JWT token}"
                });

                c.AddSecurityRequirement(document =>
                {
                    OpenApiSecuritySchemeReference schemeRef = new("Bearer");
                    return new OpenApiSecurityRequirement
                    {
                        [schemeRef] = []
                    };
                });
            });

            return services;
        }
    }
}