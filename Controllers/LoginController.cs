using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using Newtonsoft.Json;
using Seasharpbooking.Models;
using Microsoft.Extensions.Logging;

namespace Seasharpbooking.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILogger<LoginController> _logger;

        public LoginController(ILogger<LoginController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(AdminLogin admin)
        {
            try
            {
                AdminModel loginOk = null;// = new User();
                using (var httpClient = new HttpClient())
                {
                    StringContent content = new StringContent(JsonConvert.SerializeObject(admin), Encoding.UTF8, "application/json");

                    using (var response = await httpClient.PostAsync("http://informatik10.ei.hv.se/UserService/Login", content))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        loginOk = JsonConvert.DeserializeObject<AdminModel>(apiResponse);
                    }
                }

                if (loginOk.Status == true && loginOk.Role.First() == "RoomAdmin")
                {
                    await SetUserAuthenticated(loginOk);

                    //Den ska inte vara med. Bara för att visa att det fungerar
                    return Redirect("~/Booking/Index/");
                }
                else
                {
                    ViewData["failedlogin"] = "Inloggningen misslyckades";
                    return View();
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Login failed", ex);
                throw;
            }
        }

        private async Task SetUserAuthenticated(AdminModel loginOk)
        {
            try
            {
                //Inloggningsuppgifter stämmer, admin loggas in
                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                identity.AddClaim(new Claim(ClaimTypes.Name, loginOk.Status.ToString()));

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(identity));
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Could not authenticate user", ex);
                throw;
            }
            
        }

        public async Task<IActionResult> SignOut(AdminLogin admin)
        {
            try
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogCritical("Could not sign out user", ex);
                throw;
            }
            
        }
    }
}
