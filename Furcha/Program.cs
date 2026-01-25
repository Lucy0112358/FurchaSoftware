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

            builder.Services.AddTransient<IDbConnection>(sp =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();
                var connectionString = configuration.GetConnectionString("PostgreSqlConnection");
                return new NpgsqlConnection(connectionString);
            });

            var encryptionSettingsSection = builder.Configuration.GetSection(nameof(EncryptionSettings));
            builder.Services.Configure<EncryptionSettings>(encryptionSettingsSection);
            var encryptionSettings = new EncryptionSettings();
            encryptionSettingsSection.Bind(encryptionSettings);

            var app = builder.Build();
            app.UseCors(options =>
options.WithOrigins("http://localhost:5173")
.AllowAnyMethod()
.AllowAnyHeader());

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
