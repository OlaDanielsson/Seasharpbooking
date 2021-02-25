using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Seasharpbooking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Seasharpbooking.Controllers
{
    public class BookingController : Controller
    {
        public async Task<IActionResult> Index()
        {
            try
            {
                List<BookingModel> bookingList = await ApiConnection.GetBookingList();
                List<CategoryModel> categoryList = await ApiConnection.GetCategoryList();
                List<RoomdescModel> roomdescList = await ApiConnection.GetRoomdescList();

                BookingHandler.PlaceCategoryInBooking(bookingList, categoryList, roomdescList); //placerar kategoribeskrivning i bokningslistan
                return View(bookingList);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                return RedirectToAction("Privacy", "Home");
            }
        }
    
        public async Task<IActionResult> Create()
        {
            try
            {
                List<CategoryModel> categoryList = await ApiConnection.GetCategoryList();

                ViewData["Desc"] = new SelectList(categoryList, "Id", "Description"); //för att fixa viewdata
                HttpResponseMessage responseRoom = ApiConnection.ApiClient.GetAsync("CategoryModels/").Result;

                return View(new BookingModel());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return View();
            }
        }
        [HttpPost]
        public async Task<IActionResult> Create(BookingModel booking)
        {

            try
            {
                List<CategoryModel> categoryList = await ApiConnection.GetCategoryList();
                List<RoomModel> roomList = await ApiConnection.GetRoomList();
                List<BookingModel> bookingList = await ApiConnection.GetBookingList();

                List<RoomModel> qualifiedrooms = new List<RoomModel>();
                List<RoomModel> corcatroom = new List<RoomModel>();

                int bookingstart = int.Parse(DateTime.Parse(booking.StartDate.ToString()).ToString().Remove(10, 9).Remove(4, 1).Remove(6, 1)); //parsar datetime till int
                int bookingend = int.Parse(DateTime.Parse(booking.EndDate.ToString()).ToString().Remove(10, 9).Remove(4, 1).Remove(6, 1)); //parsar datetime till int

                if (bookingstart < bookingend) //kollar så bokningens start datum inte är efter slutdatum
                {
                    corcatroom.AddRange(from item in roomList
                                        where item.CategoryId == booking.CategoryId
                                        select item);
                    List<RoomModel> compareList = new List<RoomModel>();

                    BookingHandler.RoomAvailableCheck(bookingList, qualifiedrooms, corcatroom, bookingstart, bookingend, compareList); //Kollar så rummen inte är bokade

                    if (qualifiedrooms.Count > 0) //kollar ifall det finns tillgängliga rum
                    {
                        var room = qualifiedrooms.First(); 
                        BookingModel finalBooking = BookingHandler.SetFinalBooking(booking, room);

                        var postTask = ApiConnection.ApiClient.PostAsJsonAsync<BookingModel>("BookingModels", finalBooking); 
                        postTask.Wait();

                        var result = postTask.Result;
                        return RedirectToAction("Confirmation", "Booking");
                    }
                    else
                    {
                        ViewData["norooms"] = "Det finns inga lediga rum av din preferenser";
                        return View();
                    }
                }
                else
                {
                    ViewData["wrongtime"] = "Vänligen fyll i en korrekt tid. Slutdatumet måste vara senare än startdatumet.";                   
                    return View();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return View();
            }        
        }

        public ActionResult Confirmation()
        {
            return View();
        }
        public async Task<IActionResult> Delete(int id)
        {
            HttpResponseMessage response = ApiConnection.ApiClient.DeleteAsync("BookingModels/" + id.ToString()).Result;
            return RedirectToAction("Index");
        }
    }
}