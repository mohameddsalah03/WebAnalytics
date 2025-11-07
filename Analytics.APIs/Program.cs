using Analytics.APIs.Extensions;
using Analytics.APIs.Middlewares;
using Analytics.Core.Application;
using Analytics.Infrastructure;
using Analytics.Infrastructure.Persistence;
using Microsoft.OpenApi.Models;

namespace Analytics.APIs
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Configure Service

            // Controllers
            builder.Services
                .AddControllers()
                .AddApplicationPart(typeof(Analytics.APIs.Controllers.AssemblyInformation).Assembly);


            // Swagger/OpenAPI 
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "WebAnalytics API",
                    Version = "v1",
                    Description = "Analytics Data Aggregator API - ElectroPi Hiring Quest",
                    Contact = new OpenApiContact
                    {
                        Name = "ElectroPi Team"
                    }
                });

                // JWT Authentication in Swagger
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token",
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
                        }
                    },
                    Array.Empty<string>()
                }
                });

            });

            // CORS Policy
            builder.Services.AddCors(corsOptions =>
            {
                corsOptions.AddPolicy("WebAnalyticsPolicy", policyBuilder =>
                {
                    policyBuilder
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowAnyOrigin();
                });
            });

            // Application Layers 
            builder.Services.AddPersistenceServices(builder.Configuration);  
            builder.Services.AddApplicationServices();                       
            builder.Services.AddInfrastructureServices(builder.Configuration); 
            builder.Services.AddJWTServices(builder.Configuration); 


            #endregion

            var app = builder.Build();

            #region Database Migration

            await app.InitializeDbContext();

            #endregion

            #region Configure Middleware Pipeline


            app.UseMiddleware<ExceptionHandlerMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebAnalytics API v1");
                    c.RoutePrefix = "swagger";
                });
            }

            app.UseHttpsRedirection();

            app.UseCors("WebAnalyticsPolicy");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();


            #endregion

            app.Run();


        }
    }
}
