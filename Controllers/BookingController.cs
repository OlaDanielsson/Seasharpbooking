using Microsoft.AspNetCore.Mvc;
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

        public ActionResult Create(int id = 0)
        {
            try
            {
                if (id == 0)
                {
                    return View(new BookingModel());
                }

                else
                {
                    HttpResponseMessage response = ApiConnection.ApiClient.GetAsync("BookingModels/" + id.ToString()).Result;
                    return View(response.Content.ReadAsAsync<BookingModel>().Result);
                }
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
            // Om ID är 0 skickar man en ny film till API
            if (booking.Id == 0)
            {
                var postTask = ApiConnection.ApiClient.PostAsJsonAsync<BookingModel>("BookingModels", booking);
                postTask.Wait();

                var result = postTask.Result;
            }
            // Om ID inte är 0 uppdaterar man en redan befintlig film hos API med hjälp av PUT
            else
            {
                HttpResponseMessage response = ApiConnection.ApiClient.PutAsJsonAsync("BookingModels/" + booking.Id, booking).Result;
            }

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Delete(int id)
        {
            HttpResponseMessage response = ApiConnection.ApiClient.DeleteAsync("BookingModels/" + id.ToString()).Result;
            return RedirectToAction("Index");
        }
    }
}
