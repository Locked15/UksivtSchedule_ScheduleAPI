using NPOI.XWPF.UserModel;
using ScheduleAPI.Models.Cache.CachedTypes;
using ScheduleAPI.Models.Cache.CachedTypes.Basic;
using System.Text.Json.Serialization;

namespace ScheduleAPI.Models.Elements.Documents
{
    /// <summary>
    /// Класс, оборачивающий в себя документ с заменами. <br />
    /// Кроме самого документа содержит дату, на которую предназначен документ.
    /// </summary>
    public class ReplacementsDocument : ICacheable<ReplacementsDocument, ReplacementsDocumentCache>
    {
        #region Область: Свойства.

        /// <summary>
        /// Объект, содержащий документ, который будет прочитан для получения замен.
        /// </summary>
        public XWPFDocument Document { get; init; }

        /// <summary>
        /// Дата, на которую предназначаются замены.
        /// </summary>
        public DateOnly? DocumentDate { get; set; }

        [JsonIgnore]
        public bool CachingIsEnabled { get; } = false;
        #endregion

        #region Область: Конструкторы.

        /// <summary>
        /// Конструктор класса.
        /// </summary>
        /// <param name="document">Документ, содержащий замены.</param>
        public ReplacementsDocument(XWPFDocument document)
        {
            Document = document;
        }

        /// <summary>
        /// Конструктор класса.
        /// </summary>
        /// <param name="document">Документ с заменами.</param>
        /// <param name="documentDate">Дата, на которую предназначен документ.</param>
        public ReplacementsDocument(XWPFDocument document, DateOnly documentDate)
        {
            Document = document;
            DocumentDate = documentDate;
        }
        #endregion

        #region Область: Методы.

        public ReplacementsDocumentCache? GenerateCachedValue(params object[] args)
        {
            if (CachingIsEnabled)
                return new ReplacementsDocumentCache(this);
            else
                return null;
        }
        #endregion
    }
}
