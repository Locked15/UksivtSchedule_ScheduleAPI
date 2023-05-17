using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;
using ScheduleAPI.Models.Entities;
using Serilog;

namespace ScheduleAPI
{
    public class Program
    {
        /// <summary>
        /// ���������� ���������, ���������� �������� ������� �������� CORS.
        /// </summary>
        private const string CorsPolicyName = "Cross-Domain CORS";

        /// <summary>
        /// ���������� ���������, ���������� �������� ����� ��� ���������������� �������� (User Secrets).
        /// ������ �������� ������ ������ ����������� � ��.
        /// </summary>
        private const string ConnectionStringSecretKey = "DB.Local.Lite.ConnectionString";

        /// <summary>
        /// ���������� ���������, ���������� �������� ����� ��� ���������������� �������� (User Secrets).
        /// ������ �������� ������ API-���� ��� Application Insights.
        /// </summary>
        private const string ApplicationInsightSecretKey = "APPLICATIONINSIGHTS_CONNECTION_STRING";

        /// <summary>
        /// ����� ����� � ���������.
        /// </summary>
        /// <param name="args">��������� ����������.</param>
        private static void Main(string[] args)
        {
            ConfigureEnvironment();
            WebApplicationBuilder builder = CreateAndConfigureAppBuilder(args);

            var app = builder.Build();
            SetRoutings(app);
            ConfigureAppSettings(app);

            Log.Debug("Application is started up.");
            app.Run();
        }

        #region ������������ ����������.

        /// <summary>
        /// ������������� ��������� ��������� ����� �������� ����������.
        /// </summary>
        private static void ConfigureEnvironment()
        {
            Log.Logger = new LoggerConfiguration().WriteTo
                                                  .Console()
                                                  .CreateLogger();
        }

        /// <summary>
        /// ������� ����� ��������� ������-������������ ��� ���������� ����������.
        /// ����� ����� ��������� ��������� ���������� �������.
        /// </summary>
        /// <param name="args">��������� ������� ����������.</param>
        /// <returns>��������� ������-����������� ��� �������� ���������� ����������.</returns>
        private static WebApplicationBuilder CreateAndConfigureAppBuilder(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Configuration.AddUserSecrets<Program>();

            builder.Host.UseSerilog();
            ConfigureServices(builder.Services,
                              builder.Configuration.GetValue<string>(ConnectionStringSecretKey) ?? string.Empty,
                              builder.Configuration.GetValue<string>(ApplicationInsightSecretKey) ?? string.Empty);

            Log.Debug("Application builder set up is completed.");
            return builder;
        }

        /// <summary>
        /// ������������� �������, ����������� � ������ ���-����������.
        /// </summary>
        /// <param name="builder">Builder ��� ���������� ���-����������.</param>
        private static void ConfigureServices(IServiceCollection services, string dbConnectionString, string appInsightsConnectionString)
        {
            services.AddCors(options =>
                             options.AddPolicy(CorsPolicyName, GenerateCors()));
            services.AddDbContext<DataContext>(options =>
                                               options.UseNpgsql(dbConnectionString)
                                                      .UseLazyLoadingProxies()
                                                      .LogTo(Log.Logger.Information, LogLevel.Information));

            services.AddMemoryCache();
            services.AddControllers();
            services.AddControllersWithViews();

            services.AddEndpointsApiExplorer();
            services.AddApplicationInsightsTelemetry(options =>
                options.ConnectionString = appInsightsConnectionString);

            Log.Debug("Application services are configured.");
        }

        /// <summary>
        /// ������������� �������� ������������� ��� API.
        /// � ������ ������ ����� ��� ��������� ��������� �� �������� �� ���������.
        /// </summary>
        /// <param name="app">��������� ���-����������, ������� �������������.</param>
        private static void SetRoutings(WebApplication app)
        {
            app.UseRouting();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}");

            app.UseExceptionHandler("/Home/Error");
        }

        /// <summary>
        /// ������������� ��������� ���-����������.
        /// </summary>
        /// <param name="app">������������� ���-����������.</param>
        private static void ConfigureAppSettings(WebApplication app)
        {
            app.UseCors(CorsPolicyName);

            app.UseStaticFiles();
            app.MapControllers();

            app.UseStatusCodePagesWithReExecute("/Home/Status", "?code={0}");
        }
        #endregion

        #region ������ �������.

        /// <summary>
        /// ������ ������, �������������� ����� ����� CORS-������.
        /// ������������� ����� ������ ����������� ��� ����������� ����������� � API �����-���� ���������� ����������.
        /// </summary>
        /// <returns>��������� ������ CORS-������.</returns>
        private static CorsPolicy GenerateCors()
        {
            var corsBuilder = new CorsPolicyBuilder().AllowAnyHeader()
                                                     .AllowAnyMethod()
                                                     .AllowAnyOrigin();
            return corsBuilder.Build();
        }
        #endregion
    }
}
