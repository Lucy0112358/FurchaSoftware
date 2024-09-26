using Domain.Configuration;
using FurchaAdminApi.Infrustructures;
using MqttService.Infrastructure;
using Npgsql;

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

            JwtConfiguration.SetupJwtAuthentication(builder, EncryptionSettings.EncryptionKey, issuer: EncryptionSettings.Issuer, audience: EncryptionSettings.Audience);

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

    }
}
