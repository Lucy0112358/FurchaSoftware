using Domain.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MqttService.Infrastructure;
using Npgsql;
using System.Data;

namespace MqttService
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

            // Register PostgreSQL connection
            builder.Services.AddTransient<IDbConnection>(sp =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();
                var connectionString = configuration.GetConnectionString("PostgreSqlConnection");
                return new NpgsqlConnection(connectionString);
            });

            builder.Services.AddScoped<NpgsqlConnection>(provider =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();
                var connectionString = configuration.GetConnectionString("PostgreSqlConnection");
                return new NpgsqlConnection(connectionString);
            });
            var encryptionSettingsSection = builder.Configuration.GetSection(nameof(EncryptionSettings));
            builder.Services.Configure<EncryptionSettings>(encryptionSettingsSection);
            var encryptionSettings = new EncryptionSettings();
            encryptionSettingsSection.Bind(encryptionSettings);

            SetupJwtAuthentication(builder, EncryptionSettings.EncryptionKey, issuer: EncryptionSettings.Issuer, audience: EncryptionSettings.Audience);

            builder.Services.GenerateInjectionAdmin();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
        static void SetupJwtAuthentication(WebApplicationBuilder builder, string jwtAuthenticationSecret, string issuer, string audience)
        {
            var services = builder.Services;

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
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(jwtAuthenticationSecret)),
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    ValidateIssuer = true,     
                    ValidateAudience = true,   
                    ClockSkew = TimeSpan.Zero 
                };
            });
        }

    }
}
