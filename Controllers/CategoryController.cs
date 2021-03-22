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
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Seasharpbooking.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(ILogger<CategoryController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                List<CategoryModel> categoryList = await ApiConnection.GetCategoryList();
                return View(categoryList);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Could not show CategoryIndex", ex);
                return RedirectToAction("Index", "Home");
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
                _logger.LogWarning("Could not create category: Form", ex);
                return View();
            }
        }
        [HttpPost]
        public async Task<IActionResult> Create(CategoryModel category)
        {
            try
            {
                var postTask = ApiConnection.ApiClient.PostAsJsonAsync<CategoryModel>("CategoryModels", category);
                postTask.Wait();

                var result = postTask.Result;

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Could not create category: HttpPost", ex);
                throw;
            }
            
        }
        //Editfunktion om en kategori behöver ändras
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                HttpResponseMessage responseCategory = ApiConnection.ApiClient.GetAsync("CategoryModels/" + id.ToString()).Result;
                return View(responseCategory.Content.ReadAsAsync<CategoryModel>().Result);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Could not edit category: Form", ex);
                throw;
            }
            
        }
        [HttpPost]
        public async Task<IActionResult> Edit(CategoryModel category)
        {
            try
            {
                HttpResponseMessage response = ApiConnection.ApiClient.PutAsJsonAsync("CategoryModels/" + category.Id, category).Result;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Could not edit category: HttpPost", ex);
                throw;
            }
            
        }
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                HttpResponseMessage response = ApiConnection.ApiClient.DeleteAsync("CategoryModels/" + id.ToString()).Result;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Could not delete category", ex);
                throw;
            }            
        }
    }
}