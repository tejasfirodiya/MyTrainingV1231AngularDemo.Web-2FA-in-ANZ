using System;

namespace MyTrainingV1231AngularDemo.Common
{
    public static class DtoSortingHelper
    {
        public static string ReplaceSorting(string sorting, Func<string, string> replaceFunc)
        {
            var sortFields = sorting.Split(',');
            for (var i = 0; i < sortFields.Length; i++)
            {
                sortFields[i] = replaceFunc(sortFields[i].Trim());
            }

            return string.Join(",", sortFields);
        }
    }
}
