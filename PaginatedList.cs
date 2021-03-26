using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Seasharpbooking.Models;

namespace Seasharpbooking
{
    public class PaginatedList<BookingModel> : List<BookingModel>
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }

        public PaginatedList(List<BookingModel> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            this.AddRange(items);
        }

        public bool HasPreviousPage
        {
            get
            {
                return (PageIndex > 1);
            }
        }

        public bool HasNextPage
        {
            get
            {
                return (PageIndex < TotalPages);
            }
        }

        public static async Task<PaginatedList<BookingModel>> CreatePaging(List<BookingModel> bookingList, int pageIndex, int pageSize)
        {
            var count = bookingList.Count();
            var items = bookingList.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return new PaginatedList<BookingModel>(items, count, pageIndex, pageSize);
        }
    }
}
