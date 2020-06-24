using System.Collections.Generic;

namespace PlutoData.Collections
{
    public interface IPagedList<T>
    {

        /// <summary>
        /// Gets the page index (current).
        /// </summary>
        int PageIndex { get; }

        /// <summary>
        /// Gets the page size.
        /// </summary>
        int PageSize { get; }

        /// <summary>
        /// Gets the total count of the list of type <typeparamref name="T"/>
        /// </summary>
        int TotalCount { get; }


        /// <summary>
        /// Gets the current page items.
        /// </summary>
        IList<T> Items { get; }

    }
}