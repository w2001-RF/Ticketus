namespace Ticketus.Models
{
    public class PaginationFilter
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; } = null;
    }
}
