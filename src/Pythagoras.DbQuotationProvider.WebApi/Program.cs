
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.Hosting.WindowsServices;
using System.Text.Json.Serialization;
using System.Text.Json;
using NLog.Web;
using Microsoft.AspNetCore.Http.Connections;
using Pythagoras.Infrastructure.CubeClients.QProvider;
using Pythagoras.Infrastructure.CubeServers.QProvider;
using Pythagoras.DbQuotationProvider.Persistence;
using Microsoft.Extensions.Configuration;

namespace Pythagoras.DbQuotationProvider.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var options = new WebApplicationOptions
            {
                Args = args,
                ContentRootPath = WindowsServiceHelpers.IsWindowsService() ? AppContext.BaseDirectory : default
            };

            var builder = WebApplication.CreateBuilder(options);

            builder.Services.AddHttpLogging(httpLoggingOptions =>
            {
                httpLoggingOptions.LoggingFields =
                    HttpLoggingFields.All;
                //HttpLoggingFields.RequestPath |
                //HttpLoggingFields.RequestMethod |
                //HttpLoggingFields.ResponseStatusCode;
            });

            // Add services to the container.

            var appDbConnectionString = builder.Configuration.GetConnectionString("AppDb")
                ?? throw new ArgumentException("Configuration 'ConnectionStrings:AppDb' is NULL");

            builder.Services.AddRepository(appDbConnectionString);

            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                // by default
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            builder.Services.AddSignalR()
                //added by defult
                .AddJsonProtocol(options =>
                {
                    //by default
                    options.PayloadSerializerOptions.PropertyNameCaseInsensitive = true;
                    options.PayloadSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                })
                //not added by defult
                .AddMessagePackProtocol();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSignalRSwaggerGen(options =>
                {
                    options.ScanAssembly(typeof(QProviderHub).Assembly);
                });
            });

            builder.Logging.ClearProviders();
            builder.Host.UseNLog();

            builder.Services.AddCors(options => options.AddPolicy("AllowAny", builder => builder
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod()));

            var app = builder.Build();

            app.UseHttpLogging();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.MapHub<QProviderHub>(QProviderClient.HUB_URL_PATH, options =>
            {
                // only WebSockets
                options.Transports = HttpTransportType.WebSockets;
            });

            app.Run();
        }
    }
}