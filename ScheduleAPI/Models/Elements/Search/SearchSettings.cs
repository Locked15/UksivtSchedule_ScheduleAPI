namespace ScheduleAPI.Models.Elements.Search
{
    public class SearchSettings
    {
        public bool CaseSensitive { get; set; }

        public bool NormalizeRequest { get; set; }

        public static SearchSettings DefaultSettings { get; }

        public SearchSettings()
        {
            CaseSensitive = false;
            NormalizeRequest = true;
        }

        public SearchSettings(bool caseSensitive, bool normalizeRequests)
        {
            CaseSensitive = caseSensitive;
            NormalizeRequest = normalizeRequests;
        }

        static SearchSettings()
        {
            DefaultSettings = new();
        }
    }
}
