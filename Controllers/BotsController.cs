using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotCreator.Core;
using BotCreator.Core.BotQueries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebTelegramBotsBuilder.Models;
using WebTelegramBotsBuilder.Models.ViewModels;

namespace WebTelegramBotsBuilder.Controllers
{
    [Route("/bot")]
    [Authorize]
    public class BotsController : Controller
    {
        MainContext db;
        
        public BotsController(MainContext context)
        {
            db = context;
        }

        [HttpGet]
        [Route("/edit")]
        public async Task<IActionResult> EditBot(int Id)
        {
            try
            {
                ViewData["name"] = User.Identity.Name;
                Models.User user = await db.Users.Include(x => x.Bots).ThenInclude(x => x.BotQueries)
                    .ThenInclude(x => x.Response).FirstOrDefaultAsync(x => x.Name == User.Identity.Name);
                TelegramBot model;
                model = user.Bots.First(x => x.Id == Id);
                return View("EditBot", model);
            }
            catch
            {
                return View("Error", new ErrorModel("Bad bot index"));
            }
        }

        [HttpPost]
        [Route("/editbot")]
        public async Task<IActionResult> UpdateBot(int Id, string BotName, string BotToken)
        {
            try
            {
                Models.User user = await db.Users.Include(x => x.Bots).FirstOrDefaultAsync(x => x.Name == User.Identity.Name);
                TelegramBot bot;
                bot = user.Bots.First(x => x.Id == Id);
                bot.BotName = BotName;
                bot.BotToken = BotToken;
                db.Bots.Update(bot);
                await db.SaveChangesAsync();
                return RedirectToAction("EditBot", new { Id });
            }
            catch
            {
                return View("Error", new ErrorModel("Bad bot index"));
            }
        }

        [HttpPost]
        [Route("/addquery")]
        public async Task<IActionResult> AddQuery(int Id, string Query, string Response)
        {
            try
            {
                Models.User user = await db.Users.Include(x => x.Bots).ThenInclude(x => x.BotQueries)
                .ThenInclude(x => x.Response).FirstOrDefaultAsync(x => x.Name == User.Identity.Name);
                TelegramBot bot;
                bot = user.Bots.First(x => x.Id == Id);
                bot.BotQueries.Add(new BotQuery(Query, new BotResponse(Response, MessageType.Text), MessageType.Text));
                db.Update(bot);
                await db.SaveChangesAsync();
                return RedirectToAction("EditBot", new { Id });
            }
            catch
            {
                return View("Error", new ErrorModel("Bad bot index"));
            }
        }

        [HttpPost]
        [Route("/removequery")]
        public async Task<IActionResult> RemoveQuery(int Id, int BotId)
        {
            try
            {
                Models.User user = await db.Users.Include(x => x.Bots).ThenInclude(x => x.BotQueries)
                .ThenInclude(x => x.Response).FirstOrDefaultAsync(x => x.Name == User.Identity.Name);
                TelegramBot bot = user.Bots.FirstOrDefault(x => x.Id == BotId);
                db.BotQueries.Remove(bot.BotQueries.First(x => x.Id == Id));
                await db.SaveChangesAsync();
                return RedirectToAction("EditBot", new { Id = BotId });
            }
            catch
            {
                return View("Error", new ErrorModel("Bad query id"));
            }
        }

        [HttpPost]
        [Route("/updatequery")]
        public async Task<IActionResult> UpdateQuery(int Id, int BotId, string Query, string Response)
        {
            try
            {
                Models.User user = await db.Users.Include(x => x.Bots).ThenInclude(x => x.BotQueries)
                .ThenInclude(x => x.Response).FirstOrDefaultAsync(x => x.Name == User.Identity.Name);
                BotQuery target = null;

                TelegramBot bot = user.Bots.FirstOrDefault(x => x.Id == BotId);
                target = bot.BotQueries.First(x => x.Id == Id);
                target.Value = Query;
                target.Response.Value = Response;
                db.BotQueries.Update(target);
                await db.SaveChangesAsync();
                return RedirectToAction("EditBot", new { Id = BotId });
            }
            catch
            {
                return View("Error", new ErrorModel("Bad query id"));
            }
        }

        [HttpGet]
        [Route("/updatequery")]
        public async Task<IActionResult> UpdateQuery(int Id, int BotId)
        {
            try
            {
                ViewData["name"] = User.Identity.Name;
                Models.User user = await db.Users.Include(x => x.Bots).ThenInclude(x => x.BotQueries)
                    .ThenInclude(x => x.Response).FirstOrDefaultAsync(x => x.Name == User.Identity.Name);
                BotQuery target;

                TelegramBot bot = user.Bots.FirstOrDefault(x => x.Id == BotId);
                target = bot.BotQueries.First(x => x.Id == Id);
                ViewBag.BotId = BotId;
                return View("EditQuery", target);
            }
            catch
            {
                return View("Error", new ErrorModel("Bad query id"));
            }
        }
    }
}
