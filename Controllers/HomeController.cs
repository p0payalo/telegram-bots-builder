using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebTelegramBotsBuilder.Models;
using WebTelegramBotsBuilder.Models.ViewModels;

namespace WebTelegramBotsBuilder.Controllers
{
    public class HomeController : Controller
    {
        private MainContext db;

        public HomeController(MainContext context)
        {
            db = context;
        }

        [HttpGet]
        [Route("/")]
        [Route("home")]
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return View("Index", User.Identity.Name);
            }
            else
            {
                return View("Index", "");
            }
        }

        [HttpGet]
        [Route("login")]
        public IActionResult LogIn()
        {
            return View();
        }

        [HttpGet]
        [Route("register")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("login")]
        public async Task<IActionResult> LogIn(LoginModel model)
        {
            if(ModelState.IsValid)
            {
                User user = await db.Users.FirstOrDefaultAsync(u => u.Name == model.Name && u.Password == model.Password);
                if (user != null)
                {
                    await Authenticate(user.Name);
                    return RedirectToAction("Index");
                }
                else ModelState.AddModelError("", "Wrong password or username");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("register")]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await db.Users.FirstOrDefaultAsync(u => u.Name == model.Name);
                if (user == null)
                {
                    db.Users.Add(new User() { Name = model.Name, Password = model.Password });
                    await db.SaveChangesAsync();
                    Debug.WriteLine(db.Users.Count());
                    await Authenticate(model.Name);
                    return RedirectToAction("Index");
                }
                else ModelState.AddModelError("", "Name is already exist");
            }
            return View(model);
        }

        [Authorize]
        [HttpGet]
        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index");
        }

        [NonAction]
        private async Task Authenticate(string userName)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName)
            };
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }
    }
}
