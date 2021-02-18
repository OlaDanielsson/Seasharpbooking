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
                List<BookingModel> booking = new List<BookingModel>();

                var response = await ApiConnection.ApiClient.GetAsync("BookingModels");
                string jsonresponse = await response.Content.ReadAsStringAsync();
                booking = JsonConvert.DeserializeObject<List<BookingModel>>(jsonresponse);

                return View(booking);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                return RedirectToAction("Privacy", "Home");
            }
        }
        public async Task<IActionResult> Create()
        {
            try
            {
                List<CategoryModel> Categorylist = new List<CategoryModel>();

                var categoryresponse = await ApiConnection.ApiClient.GetAsync("CategoryModels");
                string jsoncategoryresponse = await categoryresponse.Content.ReadAsStringAsync();
                Categorylist = JsonConvert.DeserializeObject<List<CategoryModel>>(jsoncategoryresponse);

                ViewData["Desc"] = new SelectList(Categorylist, "Id", "Description"); //för att fixa viewdata

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
                List<CategoryModel> Categorylist = new List<CategoryModel>();

                var categoryresponse = await ApiConnection.ApiClient.GetAsync("CategoryModels");
                string jsoncategoryresponse = await categoryresponse.Content.ReadAsStringAsync();
                Categorylist = JsonConvert.DeserializeObject<List<CategoryModel>>(jsoncategoryresponse);

                List<RoomModel> roomlist = new List<RoomModel>();

                var roomresponse = await ApiConnection.ApiClient.GetAsync("RoomModels");
                string jsonroomresponse = await roomresponse.Content.ReadAsStringAsync();
                roomlist = JsonConvert.DeserializeObject<List<RoomModel>>(jsonroomresponse);

                List<BookingModel> bookinglist = new List<BookingModel>();

                var bookingresponse = await ApiConnection.ApiClient.GetAsync("Bookingmodels");
                string jsonbookingresponse = await bookingresponse.Content.ReadAsStringAsync();
                bookinglist = JsonConvert.DeserializeObject<List<BookingModel>>(jsonbookingresponse);

                List<RoomModel> qualifiedrooms = new List<RoomModel>();
                List<RoomModel> corcatroom = new List<RoomModel>();

                foreach (var item in roomlist) //loopar igenom listan room
                {
                    if (item.CategoryId == booking.RoomId) //om något item i listan room har samma categori Id som användarinmatningen så går den vidare
                    {
                        corcatroom.Add(item);
                    }
                }
                List<RoomModel> CompareList = new List<RoomModel>();
                foreach (var item in corcatroom) //loopar igenom bokningslistan
                {
                    foreach (var element in bookinglist)
                    {
                        if (element.RoomId == item.Id)
                        {
                            int start = int.Parse(DateTime.Parse(element.StartDate.ToString()).ToString().Remove(10, 9).Remove(4, 1).Remove(6, 1));
                            int end = int.Parse(DateTime.Parse(element.EndDate.ToString()).ToString().Remove(10, 9).Remove(4, 1).Remove(6, 1));
                            int bookingstart = int.Parse(DateTime.Parse(booking.StartDate.ToString()).ToString().Remove(10, 9).Remove(4, 1).Remove(6, 1));
                            int bookingend = int.Parse(DateTime.Parse(booking.EndDate.ToString()).ToString().Remove(10, 9).Remove(4, 1).Remove(6, 1));


                            if ((start < bookingstart && end < bookingstart) || (start > bookingend && end > bookingend)) // testar tidsintervallet, finns ingen bokning lägg till rum i listan
                            {
                                qualifiedrooms.Add(item);
                                break;
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            CompareList.Add(item);
                        }
                        if (bookinglist.Count == CompareList.Count)
                        {
                            qualifiedrooms.Add(item);
                            break;
                        }
                    }
                    CompareList.Clear();
                }
                if (qualifiedrooms.Count > 0)
                {
                    var room = qualifiedrooms.First();
                    //HttpResponseMessage responseBooking = ApiConnection.ApiClient.GetAsync("BookingModels/" + room.ToString()).Result; 

                    var postTask = ApiConnection.ApiClient.PostAsJsonAsync<BookingModel>("BookingModels", booking);
                    postTask.Wait();

                    var result = postTask.Result;
                    return RedirectToAction("Confirmation", "Booking");

                }
                else
                {
                    ViewBag.norooms = ("inga lediga rum");
                    //det finns inga lediga rum 
                    return View("Index", "Booking");
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

        //public async Task<IActionResult> Create(BookingModel booking) //detta är för bokning av rum för kunder
        //{
        //    List<RoomModel> room = new List<RoomModel>();

        //    var response = await ApiConnection.ApiClient.GetAsync("RoomModels");
        //    string jsonresponse = await response.Content.ReadAsStringAsync();
        //    room = JsonConvert.DeserializeObject<List<RoomModel>>(jsonresponse);

        //    List<BookingModel> bookinglist = new List<BookingModel>();

        //    var response1 = await ApiConnection.ApiClient.GetAsync("Bookingmodels");
        //    string jsonresponse1 = await response.Content.ReadAsStringAsync();
        //    bookinglist = JsonConvert.DeserializeObject<List<BookingModel>>(jsonresponse);

        //    List<RoomModel> qualifiedrooms = new List<RoomModel>();
        //    foreach (var item in room) //Detta skapar en ny lista med bokningsbara rum
        //    { 
        //        if (item.CategoryId == booking.CategoryId)
        //        {
        //            foreach (var element in bookinglist)
        //            {
        //                int start = int.Parse(DateTime.Parse(element.StartDate.ToString()).ToString().Remove(10, 9).Remove(4, 1).Remove(6, 1));
        //                int end = int.Parse(DateTime.Parse(element.StartDate.ToString()).ToString().Remove(10, 9).Remove(4, 1).Remove(6, 1));
        //                int bookingstart = int.Parse(DateTime.Parse(booking.StartDate.ToString()).ToString().Remove(10, 9).Remove(4, 1).Remove(6, 1));
        //                int bookingend = int.Parse(DateTime.Parse(booking.StartDate.ToString()).ToString().Remove(10, 9).Remove(4, 1).Remove(6, 1));


        //                if (start >= bookingstart || end >= bookingstart)
        //                {
        //                    if (start <= bookingend || end <= bookingend)
        //                    {

        //                        qualifiedrooms.Add(item);

        //                    }                  
        //                }
        //            }
        //        }
        //    }
        //    // skapa bokning av rum som ligger först i listan Qualifiedrooms
        //    int roomid = qualifiedrooms.First().Id;

        //    HttpResponseMessage responseBooking = ApiConnection.ApiClient.GetAsync("BookingModels/" + roomid.ToString()).Result;
        //    return View(new BookingModel());
        //}
    }
}
