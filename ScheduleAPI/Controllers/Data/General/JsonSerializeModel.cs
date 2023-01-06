using System.Text.Json;
using System.Text.Json.Serialization;

namespace ScheduleAPI.Controllers.Other.General
{
    /// <summary>
    /// Класс форматтера, нужный для преобразования json в другое форматирование.
    /// </summary>
    public static class JsonSerializeBinder
    {
        /// <summary>
        /// Параметры сериализации.
        /// </summary>
        public static JsonSerializerOptions JsonOptions { get; private set; }

        /// <summary>
        /// Статический конструктор.
        /// </summary>
        static JsonSerializeBinder()
        {
            JsonOptions = new JsonSerializerOptions()
            {
                WriteIndented = true,
                PropertyNameCaseInsensitive = true
            };

            JsonOptions.Converters.Add(new JsonStringEnumConverter());
        }
    }
}
