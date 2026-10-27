using System;
using System.Collections.Generic;
using System.Text;

namespace Access.Application.Utilities.Pagination
{
    public class PaginationRequest
    {
        public const int DEFAULT_PAGE_SIZE = 15;
        public const int MAX_PAGE_SIZE = 50;

        public int PageNumber { get; }
        public int PageSize { get; private set; }

        public PaginationRequest(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber < 1 ? 1 : pageNumber;
            PageSize = Math.Min(pageSize, MAX_PAGE_SIZE);
            //PageSize = pageSize > MAX_PAGE_SIZE ? MAX_PAGE_SIZE : pageSize;
        }

        public static PaginationRequest Standart()
        {
            return new PaginationRequest(1, DEFAULT_PAGE_SIZE);
        }
    }
}
