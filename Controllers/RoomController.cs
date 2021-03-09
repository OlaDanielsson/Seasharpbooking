using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    public class RoomController : Controller
    {

        public async Task<IActionResult> Index()
        {
            try
            {
                List<RoomdescModel> roomdescList = await ApiConnection.GetRoomdescList();
                List<CategoryModel> categoryList = await ApiConnection.GetCategoryList();

                foreach (var item in roomdescList)
                {
                    foreach (var element in categoryList)
                    {
                        if (item.CategoryId == element.Id)
                        {
                            item.Description = element.Description;
                            break;
                        }
                    }
                }
                return View(roomdescList);
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

                //La till den raden här under
                ViewData["CategoryId"] = new SelectList(categoryList, "Id", "Description");

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
            List<CategoryModel> categoryList = await ApiConnection.GetCategoryList();

            ViewData["CategoryId"] = new SelectList(categoryList, "Id", "Description");


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
