namespace ScheduleAPI.Models.Elements.Search.Result.Basic
{
    public abstract class BasicSearchResult<T>
    {
        public abstract SearchType SearchType { get; init; }

        public List<T> Results { get; protected set; }

        public BasicSearchResult(List<T> results)
        {
            Results = results;
        }
    }
}
