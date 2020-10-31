using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebTelegramBotsBuilder.Models;
using WebTelegramBotsBuilder.Models.Helpers;
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
        [Route("api")]
        public IActionResult ApiIndex()
        {
            return View();
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

        [HttpGet]
        [Route("settings")]
        public  async Task<IActionResult> Settings()
        {

            User user = await db.Users.FirstOrDefaultAsync(x => x.Name == User.Identity.Name);
            if (user != null)
                return View(user);
            else return RedirectToAction("LogIn", "Home");
        }

        [HttpPost]
        [Route("settings")]
        public async Task<IActionResult> Settings(int id, string username, string password)
        {
            if(ModelState.IsValid)
            {
                User us = await db.Users.FirstOrDefaultAsync(x => x.Id == id);
                us.Name = username;
                if(!us.Password.Equals(password))
                {
                    us.Password = MD5Helper.ToMD5Hash(us.Password);
                }
                db.Users.Update(us);
                await db.SaveChangesAsync();
            }
            else
            {
                ModelState.AddModelError("", "Wrong user data");
                return View(ModelState);
            }
            return View("LogIn");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("login")]
        public async Task<IActionResult> LogIn(LoginModel model)
        {
            if(ModelState.IsValid)
            {
                User user = await db.Users.FirstOrDefaultAsync(u => u.Name == model.Name && u.Password == MD5Helper.ToMD5Hash(model.Password));
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
                    db.Users.Add(new User() { Name = model.Name, Password = MD5Helper.ToMD5Hash(model.Password), ApiToken = Guid.NewGuid().ToString() });
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
