using System.Reflection;
using System.Text.Json;

namespace ScheduleAPI.Controllers.Data.General.Secrets
{
    public class ApplicationSecretsManager
    {
        #region Область: Поля.

        private readonly Dictionary<string, string?> secrets;
        #endregion

        #region Область: Свойства.

        public SecretType SecretType { get; init; }
        #endregion

        #region Область: Константы.

        private const string AppManifestSecretsPathTemplate = "ScheduleAPI.Properties.Secrets.";
        #endregion

        #region Область: Инициализаторы.

        public ApplicationSecretsManager(SecretType secretType)
        {
            SecretType = secretType;
            secrets = ReadSecretFile();
        }

        private Dictionary<string, string?> ReadSecretFile()
        {
            var targetManifestPath = GetTargetManifestPath();
            Dictionary<string, string?>? results = null;

            using Stream? stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(targetManifestPath);
            if (stream != null)
            {
                using StreamReader reader = new(stream);
                results = JsonSerializer.Deserialize<Dictionary<string, string?>>(reader.ReadToEnd());
            }

            return results ?? new Dictionary<string, string?>(1);
        }

        private string GetTargetManifestPath()
        {
            return SecretType switch
            {
                SecretType.Connection => string.Concat(AppManifestSecretsPathTemplate, "Connections.json"),

                _ => string.Concat(AppManifestSecretsPathTemplate, "Common.json")
            };
        }
        #endregion

        #region Область: Функции.

        public string? GetValue(string key) =>
               GetValue(key, null);

        public string? GetValue(string key, string? defaultValue) =>
               secrets.GetValueOrDefault(key, defaultValue);
        #endregion
    }
}
