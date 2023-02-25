namespace ScheduleAPI
{
    public class Program
    {
        /// <summary>
        /// Точка входа в программу.
        /// </summary>
        /// <param name="args">Аргументы приложения.</param>
        private static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            ConfigureServices(builder);

            var app = builder.Build();
            SetRoutings(app);
            ConfigureAppSettings(app);

            app.Run();
        }

        /// <summary>
        /// Устанавливает сервисы, подключения и службы веб-приложения.
        /// </summary>
        /// <param name="builder">Builder для экземпляра веб-приложения.</param>
        private static void ConfigureServices(WebApplicationBuilder builder)
        {
            builder.Services.AddMemoryCache();
            builder.Services.AddControllers();
            builder.Services.AddControllersWithViews();

            builder.Services.AddSwaggerGen();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddApplicationInsightsTelemetry(options =>
                options.ConnectionString = builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]);
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
            app.UseStaticFiles();
            app.MapControllers();

            app.UseStatusCodePagesWithReExecute("/Home/Status", "?code={0}");
        }
    }
}
