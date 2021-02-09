﻿using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Seasharpbooking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
                HttpClient client = new HttpClient();
                var response = await client.GetAsync("http://localhost:31328/Actor%22"); //denna ska ändras till rätt adress
                string jsonresponse = await response.Content.ReadAsStringAsync();
                Category = JsonConvert.DeserializeObject<List<CategoryModel>>(jsonresponse);

                return View(Category);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
