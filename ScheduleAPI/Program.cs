using Microsoft.AspNetCore.Cors.Infrastructure;
using Serilog;

namespace ScheduleAPI
{
    public static class Program
    {
        private const string CorsPolicyName = "Cross-Domain CORS";

        /// <summary>
        /// Точка входа в программу.
        /// </summary>
        /// <param name="args">Аргументы приложения.</param>
        private static void Main(string[] args)
        {
            ConfigureEnvironment();
            WebApplicationBuilder builder = CreateAndConfigureAppBuilder(args);

            var app = builder.Build();
            SetRoutings(app);
            ConfigureAppSettings(app);

            app.Run();
        }

        /// <summary>
        /// Устанавливает настройки окружения перед запуском приложения.
        /// </summary>
        private static void ConfigureEnvironment()
        {
            Log.Logger = new LoggerConfiguration().WriteTo
                                                  .Console()
                                                  .CreateLogger();
        }

        private static WebApplicationBuilder CreateAndConfigureAppBuilder(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            ConfigureServices(builder.Services,
                builder.Configuration.GetValue<string>("APPLICATIONINSIGHTS_CONNECTION_STRING") ?? string.Empty);

            return builder;
        }

        /// <summary>
        /// Устанавливает сервисы, подключения и службы веб-приложения.
        /// </summary>
        /// <param name="builder">Builder для экземпляра веб-приложения.</param>
        private static void ConfigureServices(IServiceCollection services, string appInsightsConnectionString)
        {
            services.AddCors(options =>
                             options.AddPolicy(CorsPolicyName, GenerateCors()));

            services.AddMemoryCache();
            services.AddControllers();
            services.AddControllersWithViews();

            services.AddEndpointsApiExplorer();
            services.AddApplicationInsightsTelemetry(options =>
                options.ConnectionString = appInsightsConnectionString);
        }

        /// <summary>
        /// Устанавливает значения маршрутизации для API.
        /// В данном случае нужен для установки маршрутов до действий по умолчанию.
        /// </summary>
        /// <param name="app">Экземпляр веб-приложения, которое настраивается.</param>
        private static void SetRoutings(WebApplication app)
        {
            app.UseRouting();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}");

            app.UseExceptionHandler("/Home/Error");
        }

        /// <summary>
        /// Устанавливает настройки веб-приложения.
        /// </summary>
        /// <param name="app">Настраиваемое веб-приложение.</param>
        private static void ConfigureAppSettings(WebApplication app)
        {
            app.UseCors(CorsPolicyName);

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
