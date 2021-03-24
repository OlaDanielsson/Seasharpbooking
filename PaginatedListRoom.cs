using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Seasharpbooking.Models;

namespace Seasharpbooking
{
    public class PaginatedListRoom<RoomdescModel> : List<RoomdescModel>
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }

        public PaginatedListRoom(List<RoomdescModel> items, int count, int pageIndex, int pageSize)
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

        public static async Task<PaginatedListRoom<RoomdescModel>> CreatePaging(List<RoomdescModel> roomdescList, int pageIndex, int pageSize)
        {
            var count = roomdescList.Count();
            var items = roomdescList.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return new PaginatedListRoom<RoomdescModel>(items, count, pageIndex, pageSize);
        }
    }
}