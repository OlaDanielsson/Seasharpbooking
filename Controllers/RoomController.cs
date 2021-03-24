using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<RoomController> _logger;

        public RoomController(ILogger<RoomController> logger)
        {
            _logger = logger;
        }

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
                _logger.LogWarning("Could not send roomDescList", ex);
                return RedirectToAction("Index", "Home");
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
                _logger.LogWarning("Could not create room: Form", ex);
                return View();
            }

        }

        [HttpPost]
        public async Task<IActionResult> Create(RoomModel room)
        {
            try
            {
                if (room.Id == 0)
                {
                    var postTask = ApiConnection.ApiClient.PostAsJsonAsync<RoomModel>("RoomModels", room);
                    postTask.Wait();

                    var result = postTask.Result;
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Could not create room: HttpPost", ex);
                throw;
            }
            
        }


        //Editfunktion om ett rum behöver ändras
        public async Task<IActionResult> Edit(int id)
        {

            try
            {
                List<CategoryModel> categoryList = await ApiConnection.GetCategoryList();

                ViewData["CategoryId"] = new SelectList(categoryList, "Id", "Description");


                HttpResponseMessage responseRoom = ApiConnection.ApiClient.GetAsync("RoomModels/" + id.ToString()).Result;
                return View(responseRoom.Content.ReadAsAsync<RoomModel>().Result);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Could not edit room: Form", ex);
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(RoomModel room)
        {
            try
            {
                HttpResponseMessage response = ApiConnection.ApiClient.PutAsJsonAsync("RoomModels/" + room.Id, room).Result;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Could not edit room: HttpPost", ex);
                throw;
            }
           
        }


        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                HttpResponseMessage response = ApiConnection.ApiClient.DeleteAsync("RoomModels/" + id.ToString()).Result;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Could not delete room", ex);
                throw;
            }
            
        }
    }
}
