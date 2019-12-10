namespace ProductMgt.ApplicationService
{
    public abstract class PaginatedResult
    {
        public int PageIndex { get; set; }
        public int RecordsPerPage { get; set; }
        public int TotalItems { get; set; }
    }
}