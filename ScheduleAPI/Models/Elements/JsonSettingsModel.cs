using System.Text.Json;
using System.Text.Json.Serialization;

namespace ScheduleAPI.Models.Elements
{
    /// <summary>
    /// Класс форматтера, нужный для преобразования json в другое форматирование.
    /// </summary>
    public static class JsonSettingsModel
    {
        /// <summary>
        /// Параметры сериализации.
        /// </summary>
        public static JsonSerializerOptions JsonOptions { get; private set; }

        /// <summary>
        /// Статический конструктор.
        /// </summary>
        static JsonSettingsModel()
        {
            JsonOptions = new JsonSerializerOptions()
            {
                WriteIndented = true,
                PropertyNameCaseInsensitive = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            JsonOptions.Converters.Add(new JsonStringEnumConverter());
        }
    }
}
