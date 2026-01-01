namespace Applications.Dto.Pagination
{
    public class PagedRequest
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Search { get; set; }

        public string? Status { get; set; }
        public int? CategoryId { get; set; } 
        public DateTime? DateFilter { get; set; }
    }

}
