using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Seasharpbooking.Controllers
{
    public class FileUploadsController : Controller
    {
        private readonly ILogger<FileUploadsController> _logger;

        public FileUploadsController(ILogger<FileUploadsController> logger)
        {
            _logger = logger;
        }

        public IActionResult Post()
        {
            return View();
        }

        [HttpPost]
        public async Task<string> Post(IFormFile image)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogWarning("Could not post data(image)", ex);
                throw;
            }            
        }
    }
}
