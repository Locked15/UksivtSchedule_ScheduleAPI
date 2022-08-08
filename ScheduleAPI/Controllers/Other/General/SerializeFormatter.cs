using System.Text.Json;

namespace ScheduleAPI.Controllers.Other.General
{
    /// <summary>
    /// Класс форматтера, нужный для преобразования json в другое форматирование.
    /// </summary>
    public static class SerializeFormatter
    {
        /// <summary>
        /// Параметры сериализации.
        /// </summary>
        public static JsonSerializerOptions JsonOptions { get; private set; }

        /// <summary>
        /// Статический конструктор.
        /// </summary>
        static SerializeFormatter()
        {
            JsonOptions = new JsonSerializerOptions()
            {
                WriteIndented = true
            };
        }
    }
}
