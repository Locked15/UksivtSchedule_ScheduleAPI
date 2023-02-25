using ScheduleAPI.Models.Result.Search.Result.Basic;

namespace ScheduleAPI.Models.Result.Search.Result
{
    public class SearchForTeacherResult : BasicSearchResult<string>
    {
        public override SearchType SearchType { get; init; } = SearchType.Teacher;

        public SearchForTeacherResult() : base(Enumerable.Empty<string>().ToList())
        {

        }

        public SearchForTeacherResult(List<string> results) : base(results)
        {

        }
    }
}
