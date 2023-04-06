using Microsoft.AspNetCore.Cors.Infrastructure;
using Serilog;

namespace ScheduleAPI
{
    public static class Program
    {
        /// <summary>
        /// ����� ����� � ���������.
        /// </summary>
        /// <param name="args">��������� ����������.</param>
        private static void Main(string[] args)
        {
            ConfigureEnvironment();

            var builder = WebApplication.CreateBuilder(args);
            ConfigureServices(builder.Services,
                builder.Configuration.GetValue<string>("APPLICATIONINSIGHTS_CONNECTION_STRING") ?? string.Empty);

            var app = builder.Build();
            SetRoutings(app);
            ConfigureAppSettings(app);

            app.Run();
        }

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
        /// ������������� �������, ����������� � ������ ���-����������.
        /// </summary>
        /// <param name="builder">Builder ��� ���������� ���-����������.</param>
        private static void ConfigureServices(IServiceCollection services, string appInsightsConnectionString)
        {
            services.AddCors(options =>
                             options.AddPolicy("CORS-Policy", GenerateCors()));

            services.AddMemoryCache();
            services.AddControllers();
            services.AddControllersWithViews();

            services.AddEndpointsApiExplorer();
            services.AddApplicationInsightsTelemetry(options =>
                options.ConnectionString = appInsightsConnectionString);
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
            app.UseStaticFiles();
            app.MapControllers();

            app.UseStatusCodePagesWithReExecute("/Home/Status", "?code={0}");
        }

        #region Standalone Functions.

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
