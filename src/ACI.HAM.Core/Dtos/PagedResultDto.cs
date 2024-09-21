namespace ACI.HAM.Core.Dtos
{
    public class PagedResultDto<T>
    {
        public PagedResultDto(IEnumerable<T> results, int currentPage, int pageSize, int? rowCount)
        {
            CurrentPage = currentPage;
            PageSize = pageSize;
            RowCount = rowCount;
            Results = new List<T>(results);
        }

        public int CurrentPage { get; set; }

        public int? PageCount => RowCount.HasValue ? (int)Math.Ceiling(RowCount.Value / (double)PageSize) : null;

        public int PageSize { get; set; }

        public List<T> Results { get; private set; }

        public int? RowCount { get; set; }
    }
}
