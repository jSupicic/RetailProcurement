using System.Collections.Generic;

namespace Retail.Application.Pagination;

/// <inheritdoc />
/// <summary>
/// Paged list interface
/// </summary>
public interface IPagedList<T> : IList<T>
{
    /// <summary>
    /// Page index
    /// </summary>
    int PageIndex { get; }

    /// <summary>
    /// Page size
    /// </summary>
    int PageSize { get; }

    /// <summary>
    /// Total count
    /// </summary>
    int TotalCount { get; }

    /// <summary>
    /// Total pages
    /// </summary>
    int TotalPages { get; }
}
