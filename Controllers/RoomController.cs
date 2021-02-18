using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Seasharpbooking.Models;
using System;
using System.Collections;
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
                List<RoomModel> roomlist = new List<RoomModel>();

                var roomresponse = await ApiConnection.ApiClient.GetAsync("RoomModels");
                string jsonroomresponse = await roomresponse.Content.ReadAsStringAsync();
                roomlist = JsonConvert.DeserializeObject<List<RoomModel>>(jsonroomresponse);

                return View(roomlist);
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
                List<CategoryModel> Category = new List<CategoryModel>();

                    var response = await ApiConnection.ApiClient.GetAsync("CategoryModels");
                    string jsonresponse = await response.Content.ReadAsStringAsync();
                    Category = JsonConvert.DeserializeObject<List<CategoryModel>>(jsonresponse);
                    //La till den raden här under
                    ViewData["CategoryId"] = new SelectList(Category, "Id", "Description");

                    HttpResponseMessage responseRoom = ApiConnection.ApiClient.GetAsync("RoomModels/").Result;

                return View(new RoomModel());

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
            if (room.Id == 0)
            {
                var postTask = ApiConnection.ApiClient.PostAsJsonAsync<RoomModel>("RoomModels", room);
                postTask.Wait();

                var result = postTask.Result;
            }

            return RedirectToAction("Index");
        }


        //Editfunktion om ett rum behöver ändras
        public async Task<IActionResult> Edit(int id)
        {
            List<CategoryModel> Category = new List<CategoryModel>();

            var response = await ApiConnection.ApiClient.GetAsync("CategoryModels");
            string jsonresponse = await response.Content.ReadAsStringAsync();
            Category = JsonConvert.DeserializeObject<List<CategoryModel>>(jsonresponse);
            ////La till den raden här under
            ViewData["CategoryId"] = new SelectList(Category, "Id", "Description");


            HttpResponseMessage responseRoom = ApiConnection.ApiClient.GetAsync("RoomModels/" + id.ToString()).Result;
            return View(responseRoom.Content.ReadAsAsync<RoomModel>().Result);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(RoomModel room)
        {
            HttpResponseMessage response = ApiConnection.ApiClient.PutAsJsonAsync("RoomModels/" + room.Id, room).Result;
            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Delete(int id)
        {
            HttpResponseMessage response = ApiConnection.ApiClient.DeleteAsync("RoomModels/" + id.ToString()).Result;
            return RedirectToAction("Index");
        }
    }
}
