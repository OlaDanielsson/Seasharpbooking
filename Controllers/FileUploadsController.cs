using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Seasharpbooking.Controllers
{
    public class FileUploadsController : Controller
    {
        public IActionResult Post()
        {
            return View();
        }
        [HttpPost]
        public async Task<string> Post(IFormFile image)
        {

            HttpContent fileStreamContent = new StreamContent(image.OpenReadStream());
            var multiContent = new MultipartFormDataContent
            {
                    { fileStreamContent, "image", image.FileName }
            };

            var response = await ApiConnection.ApiClient.PostAsync("FileUploads", multiContent);
            var data = await response.Content.ReadAsStringAsync(); ;
            return data;
        }
    }
}
