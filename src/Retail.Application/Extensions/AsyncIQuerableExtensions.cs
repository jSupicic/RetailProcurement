using Retail.Application.Pagination;
using Microsoft.EntityFrameworkCore;

namespace Retail.Application.Extensions;

public static class AsyncIQuerableExtensions
{
    /// <summary>
    /// Asynchronously creates a paged list from an IQueryable source with optional total count retrieval.
    /// </summary>
    /// <typeparam name="T">The type of elements in the source IQueryable.</typeparam>
    /// <param name="source">The IQueryable to paginate.</param>
    /// <param name="pageIndex">The index of the page to retrieve (zero-based).</param>
    /// <param name="pageSize">The size of the page to retrieve. Minimum value is 1.</param>
    /// <param name="getOnlyTotalCount">If true, only the total count of items is retrieved without fetching the data.</param>
    /// <param name="ct">The cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a paged list of the elements.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the source IQueryable is null.</exception>
    public static async Task<IPagedList<T>> ToPagedListAsync<T>(this IQueryable<T> source, int pageIndex, int pageSize, bool getOnlyTotalCount = false, CancellationToken ct = default)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source), "Source cannot be null.");

        // Ensure pageSize is at least 1
        pageSize = Math.Max(pageSize, 1);

        // Count total items in the source
        var count = await source.CountAsync(ct);

        // If only total count is needed, return early
        if (getOnlyTotalCount)
        {
            return new PagedList<T>(new List<T>(), pageIndex, pageSize, count);
        }

        // Fetch the required data page
        var data = await source.Skip(pageIndex * pageSize).Take(pageSize).ToListAsync(ct);

        return new PagedList<T>(data, pageIndex, pageSize, count);
    }
}
