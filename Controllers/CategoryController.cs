using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Seasharpbooking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Seasharpbooking.Controllers
{
    public class CategoryController : Controller
    {
        public async Task<IActionResult> Index()
        {
            try
            {
                List<CategoryModel> Category = new List<CategoryModel>();

                var response = await ApiConnection.ApiClient.GetAsync("CategoryModels");
                string jsonresponse = await response.Content.ReadAsStringAsync();
                Category = JsonConvert.DeserializeObject<List<CategoryModel>>(jsonresponse);

                return View(Category);
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
                    return View(new CategoryModel());
                }

                else
                {
                    HttpResponseMessage response = ApiConnection.ApiClient.GetAsync("CategoryModels/" + id.ToString()).Result;
                    return View(response.Content.ReadAsAsync<CategoryModel>().Result);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return View();
            }

        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoryModel category)
        {
            // Om ID är 0 skickar man en ny film till API
            if (category.Id == 0)
            {
                var postTask = ApiConnection.ApiClient.PostAsJsonAsync<CategoryModel>("CategoryModels", category);
                postTask.Wait();

                var result = postTask.Result;
            }
            // Om ID inte är 0 uppdaterar man en redan befintlig film hos API med hjälp av PUT
            else
            {
                HttpResponseMessage response = ApiConnection.ApiClient.PutAsJsonAsync("CategoryModels/" + category.Id, category).Result;
            }

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Delete(int id)
        {
            HttpResponseMessage response = ApiConnection.ApiClient.DeleteAsync("CategoryModels/" + id.ToString()).Result;
            return RedirectToAction("Index");
        }
    }
}