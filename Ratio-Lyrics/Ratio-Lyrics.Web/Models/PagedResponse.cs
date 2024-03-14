using Microsoft.EntityFrameworkCore;

namespace Ratio_Lyrics.Web.Models
{
    public class PagedResponse<T> : BaseResponse
    {
        public List<T> Items { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage => PageIndex > 1;

        public bool HasNextPage => PageIndex < TotalPages;

        public PagedResponse(List<T> items, int pageIndex, int pageSize, int totalRecords, int totalPages)
        {
            Items = items;
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalRecords = totalRecords;
            TotalPages = totalPages;
        }

        public static async Task<PagedResponse<T>> CreateAsync<T>(IQueryable<T> list, int pageNumber, int pageSize)
        {
            var totalItems = await list.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            var pagingItems = await list.ToCustomPaging<T>(pageNumber, pageSize).ToListAsync();

            return new PagedResponse<T>(pagingItems, pageNumber, pageSize, totalItems, totalPages);
        }
    }

    public static class CustomPagingExtension
    {
        public static IQueryable<T> ToCustomPaging<T>(this IQueryable<T> list, int pageNumber, int pageSize)
        {
            if (pageNumber <= 0) pageNumber = CommonConstant.PageIndexDefault;
            if (pageSize <= 0) pageSize = CommonConstant.PageSizeDefault;

            list = list.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            return list;
        }
    }
    public static class CommonConstant
    {
        public const int PageIndexDefault = 1;
        public const int PageSizeDefault = 5;
    }

    public class BaseResponse
    {
        public BaseResponse()
        {
            Message = string.Empty;
            Successded = true;
            Failed = false;
            Error = null;
        }
        public bool Successded { get; set; }
        public bool Failed { get; set; }
        public string[] Error { get; set; }
        public string Message { get; set; }
    }
}
