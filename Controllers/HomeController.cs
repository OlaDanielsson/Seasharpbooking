using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Seasharpbooking.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Seasharpbooking.Controllers
{
    public class HomeController : Controller//hej
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index() //Hej det är jag som är Simon
        {
            return View();
        }

        public IActionResult Privacy() // Forsättning
        {
            return View();//hampus bästa kommentar
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)] //Fortsättning 2
        public IActionResult Error() //Fortsättning 15miljoner
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
