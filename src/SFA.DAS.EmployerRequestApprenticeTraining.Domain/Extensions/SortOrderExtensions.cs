using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Types;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Domain.Extensions
{
    public static class SortOrderExtensions
    {
        public static SortOrder Reverse(this SortOrder sortOrder)
        {
            return sortOrder == SortOrder.Ascending
                ? SortOrder.Descending
                : SortOrder.Ascending;
        }
    }
}
