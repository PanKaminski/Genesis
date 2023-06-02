using Genesis.Common;
using Microsoft.EntityFrameworkCore;

namespace Genesis.DAL.Implementation.Extensions
{
    internal static class PagerExtensions
    {
        public static async Task<PagedModel<T>> PaginateAsync<T>(
            this IQueryable<T> queryCollection,
            int page,
            int limit,
            CancellationToken cancellationToken = default)
        {
            page = page <= 0 ? 1 : page;
            var start = (page - 1) * limit;

            var items = queryCollection.Skip(start).Take(limit).AsEnumerable();

            var count = await queryCollection.CountAsync(cancellationToken);
            return new PagedModel<T>
            {
                PageSize = limit,
                CurrentPage = page,
                TotalCount = count,
                Items = items
            };
        }
    }
}
