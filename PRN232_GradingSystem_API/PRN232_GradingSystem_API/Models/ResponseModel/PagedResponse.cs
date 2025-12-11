using System.Collections.Generic;

namespace PRN232_GradingSystem_API.Models.ResponseModel
{
    public class PagedResponse<T>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public IReadOnlyList<T> Items { get; set; }
    }
}


