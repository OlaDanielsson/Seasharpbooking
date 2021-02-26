using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
                List<CategoryModel> categoryList = await ApiConnection.GetCategoryList();
                return View(categoryList);
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
                HttpResponseMessage responseCategory = ApiConnection.ApiClient.GetAsync("CategoryModels/").Result;
                return View(new CategoryModel());
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
            var postTask = ApiConnection.ApiClient.PostAsJsonAsync<CategoryModel>("CategoryModels", category);
            postTask.Wait();

            var result = postTask.Result;

            return RedirectToAction("Index");
        }
        //Editfunktion om en kategori behöver ändras
        public async Task<IActionResult> Edit(int id)
        {
            HttpResponseMessage responseCategory = ApiConnection.ApiClient.GetAsync("CategoryModels/" + id.ToString()).Result;
            return View(responseCategory.Content.ReadAsAsync<CategoryModel>().Result);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(CategoryModel category)
        {
            HttpResponseMessage response = ApiConnection.ApiClient.PutAsJsonAsync("CategoryModels/" + category.Id, category).Result;
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Delete(int id)
        {
            HttpResponseMessage response = ApiConnection.ApiClient.DeleteAsync("CategoryModels/" + id.ToString()).Result;
            return RedirectToAction("Index");
        }
    }
}