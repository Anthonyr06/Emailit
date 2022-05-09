using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Emailit.Models.Pagination
{
    public class PagedList<T> : List<T>
    {
        /// <summary>
        /// It is on the page where the user is currently located.
        /// </summary>
        public int CurrentPage { get; private set; }
        /// <summary>
        /// Total number of pages.
        /// </summary>
        public int TotalPages { get; private set; }
        /// <summary>
        /// Number of items that were extracted from the DB
        /// </summary>
        public int PageSize { get; private set; }
        /// <summary>
        /// Total number of items that are stored in the DB.
        /// </summary>
        public int TotalCount { get; private set; }

        /// <summary>
        /// Interactive list of numbers for the Frontend. Use it to create the pagination bar in the view.
        /// </summary>
        public IEnumerable<int> NavPaginationNumbers { get; private set; }

        /// <summary>
        /// Indicates if it is available to go to the previous page.
        /// </summary>
        public bool HasPrevious => CurrentPage > 1;
        /// <summary>
        /// Indicates if it is available to go to the next page.
        /// </summary>
        public bool HasNext => CurrentPage < TotalPages;

        public PagedList(List<T> items, int count, int pageNumber, int pageSize, int? NavPaginationMaxNumber)
        {
            TotalCount = count;
            PageSize = pageSize;
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            if (NavPaginationMaxNumber != null)
            {
                NavPaginationNumbers = PaginasAMostrar(pageNumber, TotalPages, NavPaginationMaxNumber.GetValueOrDefault());
            }

            AddRange(items);
        }

        /// <summary>
        /// Method to convert an IQueryable object to a PagedList object
        /// </summary>
        /// <param name="source">IQuerable object</param>
        /// <param name="pageNumber">Actual page</param>
        /// <param name="pageSize">Number of items to return</param>
        /// <param name="NavPaginationMaxNumber">Number of boxes that will be displayed in the pagination bar in the Frontend.</param>
        /// <returns>PagedList object</returns>
        public static async Task<PagedList<T>> ToPagedListAsync(IQueryable<T> source, int pageNumber, int pageSize, int? NavPaginationMaxNumber)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            return new PagedList<T>(items, count, pageNumber, pageSize, NavPaginationMaxNumber);
        }
        private IEnumerable<int> PaginasAMostrar(int currentPage, int totalPages, int maxPages)
        {
            int startPage, endPage;
            if (totalPages <= maxPages)
            {
                //Total pages less than page max, so show all pages
                startPage = 1;
                endPage = totalPages;
            }
            else
            {
                // Total pages more than max pages so calculate start and end pages
                var maxPagesBeforeCurrentPage = (int)Math.Floor((decimal)maxPages / 2);
                var maxPagesAfterCurrentPage = (int)Math.Ceiling((decimal)maxPages / 2) - 1;
                if (currentPage <= maxPagesBeforeCurrentPage)
                {
                    // Current page near top
                    startPage = 1;
                    endPage = maxPages;
                }
                else if (currentPage + maxPagesAfterCurrentPage >= totalPages)
                {
                    // Current page near the end
                    startPage = totalPages - maxPages + 1;
                    endPage = totalPages;
                }
                else
                {
                    // Current page somewhere in the middle
                    startPage = currentPage - maxPagesBeforeCurrentPage;
                    endPage = currentPage + maxPagesAfterCurrentPage;
                }
            }

            //var startIndex = (pageNumber - 1) * pageSize;
            //var endIndex = Math.Min(startIndex + pageSize - 1, TotalCount - 1);

            // Create a variety of scrollable pages
            return Enumerable.Range(startPage, endPage + 1 - startPage);
        }

    }
}
