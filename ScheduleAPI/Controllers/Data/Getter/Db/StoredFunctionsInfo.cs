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

            public const string GetCycleIdFromDateFunctionName = "utility_cycle_id_from_date";

            public const string GetCycleIdFromDateFunctionParameters = """
                {0}
                """;

            public const string GetDayIndexFromDateFunctionName = "utility_day_index_from_date";

            public const string GetDayIndexFromDateFunctionParameters = """
                {0}
                """;
        }

        public static class TargetGroupFunctionsInfo
        {
            public const string GetGroupBasicScheduleFunctionName = "lessons_group_basic";

            public const string GetGroupBasicScheduleFunctionParameters = """
                {0},
                {1}
                """;

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
            public const string GetTeacherBasicScheduleFunctionName = "lessons_teacher_basic";

            public const string GetTeacherBasicScheduleFunctionParametersById = """
                {0},
                {1}
                """;

            public const string GetTeacherBasicScheduleFunctionParametersByBio = """
                {0},
                {1},
                {2},
                {3}
                """;

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
