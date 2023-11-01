namespace MobileApp.Host.Extensions;

public static class PagingExtensions
{
    public const int MaxPageSizeAllowable = 50;
    public static async Task<PaginationResult<T>> ToPagedResult<T>(this IQueryable<T> query, PaginationArgs paginationArgs, CancellationToken cancellationToken)
    {
        var pageSize = paginationArgs.PageSize > MaxPageSizeAllowable ? MaxPageSizeAllowable : paginationArgs.PageSize;
        var count = await query.CountAsync(cancellationToken);
        var items = await query.Skip(paginationArgs.Page * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginationResult<T>()
        {
            Items = items,
            ItemsCount = count,
            PageSize = pageSize,
            Page = paginationArgs.Page,
            PagesCount = (int)Math.Ceiling(count / (double)pageSize)
        };
    }
}

public class PaginationArgs
{
    public int Page { get; set; }
    public int PageSize { get; set; }
}

public class PaginationResult<T>
{
    public IEnumerable<T> Items { get; set; }
    public int ItemsCount { get; set; }
    public int PagesCount { get; set; }
    public int PageSize { get; set; }
    public int Page { get; set; }
}