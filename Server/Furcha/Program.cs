using Domain.Configuration;
using FurchaAdminApi.Infrustructures;
using FurchaAdminApi.Middlewares;
using Microsoft.OpenApi.Models;
using MQTTnet.Client;
using MQTTnet;
using Npgsql;
using BLL.Services;
using FurchaBLL.Services;
using FurchaBLL.Interfaces;

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
            /*            builder.Services.AddCors(options =>
                             {
                                 options.AddDefaultPolicy(builder =>
                                 {
                                     builder.WithOrigins("http://192.168.0.129:3033/")
                                            .AllowAnyHeader()
                                            .AllowAnyMethod()
                                            .AllowCredentials();
                                 });
                             });*/

            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyHeader()
                           .AllowAnyMethod();
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

            builder.Services.AddSingleton<IMqttClient>(sp =>
            {
                var factory = new MqttFactory();
                return factory.CreateMqttClient();
            });
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

            builder.Services.AddSingleton<IMqttApiService, MqttApiService>();
            builder.Services.AddHostedService<MqttHostedService>();

            builder.Services.AddSingleton<IMqttClient>(new MqttFactory().CreateMqttClient());

            builder.Services.AddSingleton<MqttClientOptions>(sp =>
            {
                var config = sp.GetRequiredService<IConfiguration>().GetSection("MqttSettings");
                return new MqttClientOptionsBuilder()
                    .WithClientId(config["ClientId"])
                    .WithTcpServer(config["Server"], int.Parse(config["Port"]))
                    .WithCredentials(config["Username"], config["Password"])
                    .Build();
            });

            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseCors();
            app.UseRouting();
            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware<PermissionMiddleware>();
            app.MapControllers();
            app.Run();
        }
    }
}
