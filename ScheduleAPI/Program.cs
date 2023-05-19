using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;
using ScheduleAPI.Controllers.Data.General.Secrets;
using ScheduleAPI.Models.Entities;
using Serilog;

namespace ScheduleAPI
{
    public static class Program
    {
        #region Константы приложения.

        /// <summary>
        /// Внутренняя константа, содержащая название целевой политики CORS.
        /// </summary>
        private const string CorsPolicyName = "Cross-Domain CORS";

        /// <summary>
        /// Внутренняя константа, содержащая название ключа для пользовательских секретов (User Secrets).
        /// Данное значение хранит API-Ключ для Application Insights.
        /// </summary>
        private const string ApplicationInsightSecretKey = "ApplicationInsight.APIKey";

        /// <summary>
        /// Внутренняя константа, содержащая название ключа для пользовательских секретов (User Secrets).
        /// Данное значение хранит строку подключения к БД. В отличие от прочих ключей, этот является шаблоном.
        /// Итоговый ключ выстраивается на основе отправленных параметров приложения.
        /// </summary>
        private const string ConnectionStringSecretKeyTemplate = "DB{0}.Lite.ConnectionString";
        #endregion

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

            Log.Debug("Application is started up.");
            app.Run();
        }

        #region Конфигурация Приложения.

        /// <summary>
        /// Устанавливает настройки окружения перед запуском приложения.
        /// </summary>
        private static void ConfigureEnvironment()
        {
            Log.Logger = new LoggerConfiguration().WriteTo
                                                  .Console()
                                                  .CreateLogger();
        }

        /// <summary>
        /// Создает новый экземпляр класса-конструктора для экземпляра приложения.
        /// После этого выполняет настройку созданного объекта.
        /// </summary>
        /// <param name="args">Параметры запуска приложения.</param>
        /// <returns>Созданный объект-конструктор для создания экземпляра приложения.</returns>
        private static WebApplicationBuilder CreateAndConfigureAppBuilder(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var secretsManager = new ApplicationSecretsManager(SecretType.Connection);

            /* На этапе конфигурации сервисов приложения, указанные строки никогда не будут 'NULL'.
               Потому что значение по умолчанию всегда "string.Empty"). */
            builder.Host.UseSerilog();
            ConfigureServices(builder.Services,
                              secretsManager.GetValue(GetConnectionStringSecretKeyByArguments(args), string.Empty)!,
                              secretsManager.GetValue(ApplicationInsightSecretKey, string.Empty)!);

            Log.Debug("Application builder set up is completed.");
            return builder;
        }

        /// <summary>
        /// Устанавливает сервисы, подключения и службы веб-приложения.
        /// </summary>
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
            app.MapControllers();

            app.UseStatusCodePagesWithReExecute("/Home/Status", "?code={0}");
        }
        #endregion

        #region Прочие Функции.

        /// <summary>
        /// Создаёт итоговый вариант ключа к значению в файле секретов, содержащем строку подключения.
        /// В зависимости от аргументов приложения, итоговая строка может вести к различным базам данных.
        /// </summary>
        /// <param name="args">Отправленные параметры приложения.</param>
        /// <returns>Построенная по шаблону строка подключения к БД.</returns>
        private static string GetConnectionStringSecretKeyByArguments(string[] args)
        {
            var dbTypePrefix = string.Empty;
            var targetIndex = Array.FindIndex(args, arg =>
                                                    arg.Equals("--db-type", StringComparison.OrdinalIgnoreCase));
            if (targetIndex != -1 && args.ElementAtOrDefault(targetIndex + 1) is string rawPrefix)
            {
                rawPrefix = rawPrefix.Trim().ToLower();
                dbTypePrefix = string.Concat('.', char.ToUpper(rawPrefix[0]), rawPrefix[1..]);

                Log.Debug($"Default DB destination was overwritten with new ({dbTypePrefix}) target.");
            }

            return string.Format(ConnectionStringSecretKeyTemplate, dbTypePrefix);
        }

        /// <summary>
        /// Создаёт объект, представляющий собой набор CORS-правил.
        /// Использование этого набора обязательно для возможности подключения к API каких-либо клиентских приложений.
        /// </summary>
        /// <returns>Созданный объект CORS-правил.</returns>
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
