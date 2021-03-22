using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Seasharpbooking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Seasharpbooking.Controllers
{
    [Authorize]
    public class BookingController : Controller
    {
        private readonly ILogger<BookingController> _logger;

        public BookingController(ILogger<BookingController> logger)
        {
            _logger = logger;
        }

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
                _logger.LogWarning("Could not fetch bookings", ex);
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
                _logger.LogWarning("Could not create booking: Form", ex);
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
                int dateToday = int.Parse(DateTime.Parse(DateTime.Today.ToString()).ToString().Remove(10, 9).Remove(4, 1).Remove(6, 1));

                if (bookingstart < bookingend && bookingstart >= dateToday) //kollar så bokningens start datum inte är efter slutdatum
                {
                    corcatroom.AddRange(from item in roomList
                                        where item.CategoryId == booking.CategoryId
                                        select item);

                    List<BookingModel> corcatbooking = new List<BookingModel>();

                    //Används inte än men kan tas tag i imorgon.
                    //BookingHandler.GetCorCatBookingList(bookingList, corcatbooking, booking.CategoryId); //Hämtar lista med enbart bokningar av korrekta kategori.
                    //---------------------------------------------------------------------------------------------------------------------------------------------

                    BookingHandler.RoomAvailableCheckV2(bookingList, corcatroom, bookingstart, bookingend);

                    if (corcatroom.Count > 0) //kollar ifall det finns tillgängliga rum
                    {
                        var room = corcatroom.First();
                        BookingModel finalBooking = BookingHandler.SetFinalBooking(booking, room);

                        var postTask = ApiConnection.ApiClient.PostAsJsonAsync<BookingModel>("BookingModels", finalBooking);
                        postTask.Wait();

                        var result = postTask.Result;
                        return RedirectToAction("Confirmation", "Booking");
                    }


                    else
                    {
                        ViewData["norooms"] = "Det finns inga lediga rum av din preferenser";
                        await ViewbagCategory();
                        return View(new BookingModel());
                    }
                }
                else
                {
                    ViewData["wrongtime"] = "Vänligen fyll i en korrekt tid. Slutdatumet måste vara senare än startdatumet.";
                    await ViewbagCategory();
                    return View(new BookingModel());
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Could not create booking: HttpPost", ex);
                return View();
            }
        }

        public ActionResult Confirmation()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Could not show confirmation screen", ex);
                throw;
            }
        }

        public async Task<IActionResult> SuperDelete(int? id)
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
                _logger.LogWarning("Could not show delete confirmation screen", ex);
                throw;
            }
            

            
        }
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                HttpResponseMessage response = ApiConnection.ApiClient.DeleteAsync("BookingModels/" + id.ToString()).Result;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Could not delete booking", ex);
                throw;
            }           
        }

        public async Task<IActionResult> Edit(int id, int GuestId)
        {
            try
            {
                await ViewbagCategory();
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Could not edit booking: Form", ex);
                throw;
            }           
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, int GuestId,[Bind("Id,CategoryId,StartDate,EndDate,GuestId")] BookingModel booking)
        {

            try
            {
                //först måste vi ta bort aktuell bokning från den lokala bokningslistan
                //anropa metod för att kolla om bokningen är möjlig
                List<RoomModel> roomList = await ApiConnection.GetRoomList();
                List<BookingModel> bookingList = await ApiConnection.GetBookingList();

                foreach (var item in bookingList)
                {
                    if (booking.Id == item.Id)
                    {
                        bookingList.Remove(item);
                        break;
                    }
                }

                int bookingstart = int.Parse(DateTime.Parse(booking.StartDate.ToString()).ToString().Remove(10, 9).Remove(4, 1).Remove(6, 1)); //parsar datetime till int
                int bookingend = int.Parse(DateTime.Parse(booking.EndDate.ToString()).ToString().Remove(10, 9).Remove(4, 1).Remove(6, 1)); //parsar datetime till int
                int dateToday = int.Parse(DateTime.Parse(DateTime.Today.ToString()).ToString().Remove(10, 9).Remove(4, 1).Remove(6, 1));

                List<RoomModel> corcatroom = new List<RoomModel>();

                if (bookingstart < bookingend && bookingstart >= dateToday) //kollar så bokningens start datum inte är efter slutdatum
                {
                    corcatroom.AddRange(from item in roomList
                                        where item.CategoryId == booking.CategoryId
                                        select item);

                    //---------------------------------------------------------------------------------------------------------------------------------------------

                    BookingHandler.RoomAvailableCheckV2(bookingList, corcatroom, bookingstart, bookingend);

                    RoomModel Besterummet = new RoomModel();

                    if (corcatroom.Count() > 0)
                    {
                        Besterummet = corcatroom.First();

                        BookingModel finalBooking = new BookingModel();
                        finalBooking.Id = booking.Id;
                        finalBooking.StartDate = booking.StartDate;
                        finalBooking.EndDate = booking.EndDate;
                        finalBooking.RoomId = Besterummet.Id;
                        finalBooking.GuestId = GuestId;

                        HttpResponseMessage response = ApiConnection.ApiClient.PutAsJsonAsync("BookingModels/" + finalBooking.Id, finalBooking).Result;
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ViewData["norooms"] = "Det finns inga lediga rum av din preferenser";
                        await ViewbagCategory();
                        return View();
                    }

                }
                else
                {
                    ViewData["wrongtime"] = "Denna ändring är inte möjlig";
                    await ViewbagCategory();
                    return RedirectToAction("Privacy", "Home");
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Could not edit booking: HttpPost", ex);
                throw;
            }
            
        }

        private async Task ViewbagCategory()
        {
            try
            {
                List<CategoryModel> categoryList = await ApiConnection.GetCategoryList();
                ViewData["Desc"] = new SelectList(categoryList, "Id", "Description"); //för att fixa viewdata
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Could not send categories with ViewBag", ex);
                throw;
            }
            
        }
    }
}