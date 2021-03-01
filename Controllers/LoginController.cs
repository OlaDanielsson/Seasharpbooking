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

namespace Seasharpbooking.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(AdminLogin admin)
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

            if (loginOk.Status == true)
            {
                await SetUserAuthenticated(loginOk);

                //Den ska inte vara med. Bara för att visa att det fungerar
                return Redirect("~/Home/Index/");
            }
            else
            {
                ViewData["failedlogin"] = "Inloggningen misslyckades";
                return View();
            }
        }

        private async Task SetUserAuthenticated(AdminModel loginOk)
        {
            //Inloggningsuppgifter stämmer, admin loggas in
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.Name, loginOk.Status.ToString()));

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));
        }
    }
}
