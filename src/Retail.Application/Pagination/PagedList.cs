namespace Retail.Application.Pagination;

/// <inheritdoc cref="IPagedList{T}" />
/// <summary>
/// Paged list
/// </summary>
/// <typeparam name="T">T</typeparam>
[Serializable]
public class PagedList<T> : List<T>, IPagedList<T>
{
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="source">source</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="getOnlyTotalCount">A value in indicating whether you want to load only total number of records. Set to "true" if you don't want to load data from database</param>
    public PagedList(IList<T> source, int pageIndex, int pageSize, int? totalCount = null)
    {
        //min allowed page size is 1
        pageSize = Math.Max(pageSize, 1);

        TotalCount = totalCount ?? source.Count;
        TotalPages = TotalCount / pageSize;

        if (TotalCount % pageSize > 0)
            TotalPages++;

        PageSize = pageSize;
        PageIndex = pageIndex;
        AddRange(totalCount != null ? source : source.Skip(pageIndex * pageSize).Take(pageSize));
    }
    /// <summary>
    /// Page index
    /// </summary>
    public int PageIndex { get; }

    /// <summary>
    /// Page size
    /// </summary>
    public int PageSize { get; }

    /// <summary>
    /// Total count
    /// </summary>
    public int TotalCount { get; }

    /// <summary>
    /// Total pages
    /// </summary>
    public int TotalPages { get; }
}
