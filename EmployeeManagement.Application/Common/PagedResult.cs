namespace EmployeeManagement.Application.Common
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; } = [];
        public int TotalCount { get; set; }
        public int? LastSeenId { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

}