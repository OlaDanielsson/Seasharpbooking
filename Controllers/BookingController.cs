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
                List<BookingModel> Booking = new List<BookingModel>();
                HttpClient client = new HttpClient();
                var response = await client.GetAsync("http://localhost:31328/Actor%22"); //denna ska ändras till rätt adress
                string jsonresponse = await response.Content.ReadAsStringAsync();
                Booking = JsonConvert.DeserializeObject<List<BookingModel>>(jsonresponse);

                return View(Booking);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
