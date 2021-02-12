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
    public class RoomController : Controller
    {
        public async Task<IActionResult> Index()
        {
            try
            {
                List<RoomModel> room = new List<RoomModel>();

                var response = await ApiConnection.ApiClient.GetAsync("RoomModels");
                string jsonresponse = await response.Content.ReadAsStringAsync();
                room = JsonConvert.DeserializeObject<List<RoomModel>>(jsonresponse);

                return View(room);
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
                    return View(new RoomModel());
                }

                else
                {
                    HttpResponseMessage response = ApiConnection.ApiClient.GetAsync("RoomModels/" + id.ToString()).Result;
                    return View(response.Content.ReadAsAsync<RoomModel>().Result);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return View();
            }

        }

        [HttpPost]
        public async Task<IActionResult> Create(RoomModel room)
        {
            // Om ID är 0 skickar man en ny film till API
            if (room.Id == 0)
            {
                var postTask = ApiConnection.ApiClient.PostAsJsonAsync<RoomModel>("RoomModels", room);
                postTask.Wait();

                var result = postTask.Result;
            }
            // Om ID inte är 0 uppdaterar man en redan befintlig film hos API med hjälp av PUT
            else
            {
                HttpResponseMessage response = ApiConnection.ApiClient.PutAsJsonAsync("RoomModels/" + room.Id, room).Result;
            }

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Delete(int id)
        {
            HttpResponseMessage response = ApiConnection.ApiClient.DeleteAsync("RoomModels/" + id.ToString()).Result;
            return RedirectToAction("Index");
        }
    }
}
