namespace ScheduleAPI.Controllers.Data.Getter.Db
{
    internal static class StoredFunctionsInfo
    {
        public static class UtilityFunctionsInfo
        {
            public const string GetTeachersFunctionName = "utility_get_teachers";

            public const string GetTeachersFunctionParameters = """
                {0},
                {1},
                {2}
                """;
        }

        public static class TargetGroupFunctionsInfo
        {
            public const string GetGroupReplacementsFunctionName = "lessons_group_replacement";

            public const string GetGroupReplacementsFunctionParameters = """
                {0},
                {1}
                """;

            public const string GetGroupFinalScheduleFunctionName = "lessons_group_final";

            public const string GetGroupFinalScheduleFunctionParameters = """
                {0},
                {1}
                """;
        }

        public static class TargetTeacherFunctionsInfo
        {
            public const string GetTeacherReplacementsFunctionName = "lessons_teacher_replacement";

            public const string GetTeacherReplacementsFunctionParametersById = """
                {0},
                {1}
                """;

            public const string GetTeacherReplacementsFunctionParametersByBio = """
                {0},
                {1},
                {2},
                {3}
                """;

            public const string GetTeacherFinalScheduleFunctionName = "lessons_teacher_final";

            public const string GetTeacherFinalScheduleFunctionParametersById = """
                {0},
                {1}
                """;

            public const string GetTeacherFinalScheduleFunctionParametersByBio = """
                {0},
                {1},
                {2},
                {3}
                """;
        }
    }
}
