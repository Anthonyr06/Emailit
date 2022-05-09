using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emailit.Models.Pagination
{
    public abstract class PaginationParameters
    {
        /// <summary>
        /// Default page number (if PageNumber is not assigned)
        /// </summary>
        private int _PageNumber = 1;
        /// <summary>
        /// Page requested by the user
        /// </summary>
        public int PageNumber
        {
            get
            {
                return _PageNumber;
            }
            set
            {
                _PageNumber = value > 0 ? value : _PageNumber;
            }
        }

        /// <summary>
        /// Maximum number of items that can be extracted from the DB
        /// </summary>
        const int maxPageSize = 50;
        /// <summary>
        /// Default number of items that are extracted from the DB (if PageSize is not assigned)
        /// </summary>
        private int _pageSize = 8;
        /// <summary>
        /// Number of items to be extracted from the DB
        /// </summary>
        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = value > maxPageSize || value < 0 ? maxPageSize : value;
            }
        }

        /// <summary>
        /// Number of boxes that will be displayed in the pagination bar in the Frontend.
        /// </summary>
        public int? NavPaginationMaxNumber { get; set; }
    }
}
