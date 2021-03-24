using Seasharpbooking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seasharpbooking
{
    public class BookingListViewHelper
    {
        public static List<BookingModel> ColumnSwitch(string sortOrder, List<BookingModel> bookingList)
        {
            switch (sortOrder)
            {
                case "bookingID_descending":
                    bookingList = bookingList.OrderByDescending(o => o.Id).ToList();
                    break;
                case "GuestID":
                    bookingList = bookingList.OrderBy(o => o.GuestId).ToList();
                    break;
                case "GuestID_descending":
                    bookingList = bookingList.OrderByDescending(o => o.GuestId).ToList();
                    break;
                case "StartDate":
                    bookingList = bookingList.OrderBy(o => o.StartDate).ToList();
                    break;
                case "StartDate_descending":
                    bookingList = bookingList.OrderByDescending(o => o.StartDate).ToList();
                    break;
                case "EndDate":
                    bookingList = bookingList.OrderBy(o => o.EndDate).ToList();
                    break;
                case "EndDate_descending":
                    bookingList = bookingList.OrderByDescending(o => o.EndDate).ToList();
                    break;
                case "RoomType":
                    bookingList = bookingList.OrderBy(o => o.CatDescription).ToList();
                    break;
                case "RoomType_descending":
                    bookingList = bookingList.OrderByDescending(o => o.CatDescription).ToList();
                    break;
                case "RoomID":
                    bookingList = bookingList.OrderBy(o => o.RoomId).ToList();
                    break;
                case "RoomID_descending":
                    bookingList = bookingList.OrderByDescending(o => o.RoomId).ToList();
                    break;
                default:
                    bookingList = bookingList.OrderBy(o => o.Id).ToList();
                    break;
            }

            return bookingList;
        }
    }
}
