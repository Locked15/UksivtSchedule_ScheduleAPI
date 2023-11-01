using System.Text.RegularExpressions;

namespace ScheduleAPI.Models.Elements
{
    public partial class AffiliationsInfo
    {
        public string BranchName { get; set; }

        public List<string> Affiliations { get; set; }

        public AffiliationsInfo(string branchName, List<string> affiliations)
        {
            BranchName = branchName;
            Affiliations = affiliations;
        }

        [GeneratedRegex("[а-яА-Я]")]
        public static partial Regex BranchExtractionRegEx();
    }
}
