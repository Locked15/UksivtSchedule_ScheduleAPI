using ScheduleAPI.Models.Result.Search.Result.Basic;

namespace ScheduleAPI.Models.Result.Search.Result
{
    public class SearchForGroupResult : BasicSearchResult<string>
    {
        public override SearchType SearchType { get; init; } = SearchType.Group;

        public SearchForGroupResult() : base(Enumerable.Empty<string>().ToList())
        {

        }

        public SearchForGroupResult(List<string> results) : base(results)
        {

        }
    }
}
