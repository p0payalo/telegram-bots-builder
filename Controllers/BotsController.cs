using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotCreator.Core;
using BotCreator.Core.BotQueries;
using BotCreator.Core.BotResponses;
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
            ViewData["name"] = User.Identity.Name;
            Models.User user = await db.Users.Include(x => x.Bots).ThenInclude(x => x.BotQueries)
                .ThenInclude(x => x.Response).FirstOrDefaultAsync(x => x.Name == User.Identity.Name);
            TelegramBot model;
            try
            {
                model = user.Bots.FirstOrDefault(x => x.TelegramBotId == Id);
            }
            catch
            {
                return View("Error", new ErrorModel("Bad bot index"));
            }
            return View("EditBot", model);
        }

        [HttpPost]
        [Route("/editbot")]
        public async Task<IActionResult> UpdateBot(int Id, string BotName, string BotToken)
        {
            Models.User user = await db.Users.Include(x => x.Bots).FirstOrDefaultAsync(x => x.Name == User.Identity.Name);
            TelegramBot bot;
            try
            {
                bot = user.Bots.First(x => x.TelegramBotId == Id);
                bot.BotName = BotName;
                bot.BotToken = BotToken;
                db.Bots.Update(bot);
                await db.SaveChangesAsync();
            }
            catch
            {
                return View("Error", new ErrorModel("Bad bot index"));
            }
            return RedirectToAction("EditBot", new { Id });
        }

        [HttpPost]
        [Route("/addquery")]
        public async Task<IActionResult> AddQuery(int Id, string Query, string Response)
        {
            Models.User user = await db.Users.Include(x => x.Bots).ThenInclude(x => x.BotQueries)
                .ThenInclude(x => x.Response).FirstOrDefaultAsync(x => x.Name == User.Identity.Name);
            TelegramBot bot;
            try
            {
                bot = user.Bots.First(x => x.TelegramBotId == Id);
                bot.BotQueries.Add(new BotMessageQuery(Query, new BotMessageResponse(Response)));
                db.Update(bot);
                await db.SaveChangesAsync();
            }
            catch
            {
                return View("Error", new ErrorModel("Bad bot index"));
            }
            return RedirectToAction("EditBot", new { Id });
        }

        [HttpPost]
        [Route("/removequery")]
        public async Task<IActionResult> RemoveQuery(int Id, int BotId)
        {
            Models.User user = await db.Users.Include(x => x.Bots).ThenInclude(x => x.BotQueries)
                .ThenInclude(x => x.Response).FirstOrDefaultAsync(x => x.Name == User.Identity.Name);
            TelegramBot bot = user.Bots.FirstOrDefault(x => x.TelegramBotId == BotId);
            try
            {
                db.BotQueries.Remove(bot.BotQueries.First(x => x.BotQueryId == Id));
            }
            catch
            {
                return View("Error", new ErrorModel("Bad query id"));
            }
            await db.SaveChangesAsync();
            return RedirectToAction("EditBot", new { Id = BotId });
        }

        [HttpPost]
        [Route("/updatequery")]
        public async Task<IActionResult> UpdateQuery(int Id, int BotId, string Query, string Response)
        {
            Models.User user = await db.Users.Include(x => x.Bots).ThenInclude(x => x.BotQueries)
                .ThenInclude(x => x.Response).FirstOrDefaultAsync(x => x.Name == User.Identity.Name);
            BotMessageQuery target = null;
            try
            {
                TelegramBot bot = user.Bots.FirstOrDefault(x => x.TelegramBotId == BotId);
                target = (BotMessageQuery)bot.BotQueries.First(x => x.BotQueryId == Id);
                target.Message = Query;
                (target.Response as BotMessageResponse).Message = Response;
            }
            catch
            {
                return View("Error", new ErrorModel("Bad query id"));
            }
            db.BotQueries.Update(target);
            await db.SaveChangesAsync();
            return RedirectToAction("EditBot", new { Id = BotId });
        }

        [HttpGet]
        [Route("/updatequery")]
        public async Task<IActionResult> UpdateQuery(int Id, int BotId)
        {
            ViewData["name"] = User.Identity.Name;
            Models.User user = await db.Users.Include(x => x.Bots).ThenInclude(x => x.BotQueries)
                .ThenInclude(x => x.Response).FirstOrDefaultAsync(x => x.Name == User.Identity.Name);
            BotMessageQuery target;
            try
            {
                TelegramBot bot = user.Bots.FirstOrDefault(x => x.TelegramBotId == BotId);
                target = (BotMessageQuery)bot.BotQueries.First(x => x.BotQueryId == Id);
            }
            catch
            {
                return View("Error", new ErrorModel("Bad query id"));
            }
            ViewBag.BotId = BotId;
            return View("EditQuery", target);
        }
    }
}
