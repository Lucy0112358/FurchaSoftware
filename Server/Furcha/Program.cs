using Domain.Configuration;
using FurchaAdminApi.Infrustructures;
using Microsoft.OpenApi.Models;
using Npgsql;

namespace FurchaAdminApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure settings
            builder.Configuration.AddJsonFile("appsettings.json");

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policyBuilder =>
                {
                    policyBuilder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });

            builder.Services.AddScoped<NpgsqlConnection>(provider =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();
                var connectionString = configuration.GetConnectionString("PostgreSqlConnection");
                return new NpgsqlConnection(connectionString);
            });

            var encryptionSettings = builder.Configuration.GetSection("EncryptionSettings");
            EncryptionSettings.EncryptionKey = encryptionSettings["EncryptionKey"];
            EncryptionSettings.Issuer = encryptionSettings["Issuer"];
            EncryptionSettings.Audience = encryptionSettings["Audience"];


            // Setup JWT Authentication
            JwtConfiguration.SetupJwtAuthentication(builder, EncryptionSettings.EncryptionKey, EncryptionSettings.Issuer, EncryptionSettings.Audience);
            builder.Services.GenerateInjectionAdmin();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

                // Add JWT Authentication to Swagger
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter your JWT token in the format: Bearer <token>"
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
            new string[] { }
        }
    });
            });
            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("AllowAll");

            app.UseHttpsRedirection(); 
            //app.UseAuthentication(); 
            //app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
