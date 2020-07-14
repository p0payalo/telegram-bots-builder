using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BotCreator.Core;
using WebTelegramBotsBuilder.Models;
using Microsoft.EntityFrameworkCore;
using WebTelegramBotsBuilder.Models.ViewModels;
using WebTelegramBotsBuilder.Models.Helpers;
using System;
using System.IO;
using System.IO.Compression;
using Microsoft.AspNetCore.Hosting;
using System.Runtime.Serialization.Formatters.Binary;

namespace WebTelegramBotsBuilder.Controllers
{
    [Route("/bots")]
    [Authorize]
    public class MainController : Controller
    {
        public MainContext db;
        private readonly IWebHostEnvironment appEnvironment;
        public MainController(MainContext context, IWebHostEnvironment hostEnvironment)
        {
            db = context;
            appEnvironment = hostEnvironment;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Bots()
        {
            Models.User user = await db.Users.Include(u => u.Bots).FirstAsync(x => x.Name == User.Identity.Name);
            if (user != null)
            {
                return View("Bots", user);
            }
            return RedirectToAction("LogIn", "Home");
        }

        [HttpPost]
        [Route("addbot")]
        public async Task<IActionResult> AddBot(string BotName, string BotToken)
        {
            try
            {
                Models.User user = await db.Users.FirstAsync(x => x.Name == User.Identity.Name);
                (user.Bots as List<TelegramBot>).Add(new TelegramBot(BotToken, BotName));
                db.Update(user);
                db.SaveChanges();
            }
            catch
            {
                return View("Error", new ErrorModel("Undefined error"));
            }
            return RedirectToAction("Bots");
        }

        [HttpPost]
        [Route("removebot")]
        public async Task<IActionResult> RemoveBot(int Id)
        {
            try
            {
                Models.User user = await db.Users.Include(x => x.Bots).FirstAsync(x => x.Name == User.Identity.Name);
                (user.Bots as List<TelegramBot>).Remove(user.Bots.First(x => x.Id == Id));
                db.Update(user);
                await db.SaveChangesAsync();
                return RedirectToAction("Bots");
            }
            catch (Exception e)
            {
                return View("Error", new ErrorModel("Bad bot index"));
            }
        }

        [HttpPost]
        [Route("download")]
        public async Task<IActionResult> DownloadBot(int Id)
        {
            try
            {
                Models.User user = await db.Users.Include(x => x.Bots).ThenInclude(x => x.BotQueries)
                                        .ThenInclude(x => x.Response).FirstAsync(x => x.Name == User.Identity.Name);
                string files = appEnvironment.ContentRootPath + @"\UserFiles";
                string userPath = Directory.CreateDirectory(files + @"\" + User.Identity.Name).FullName;
                TelegramBot bot = user.Bots.First(x => x.Id == Id);
                string botPath = Directory.CreateDirectory(userPath + @"\" + bot.BotName).FullName;
                using (FileStream fs = System.IO.File.Create(botPath + @"\" + bot.BotName + ".bot"))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(fs, bot);
                }
                using (ZipArchive zip = ZipFile.Open(botPath + @"\bot.zip", ZipArchiveMode.Create))
                {
                    zip.CreateEntryFromFile(botPath + @"\" + bot.BotName + ".bot", "root/" + bot.BotName + ".bot");
                    string[] botHandlerFiles = Directory.GetFiles(appEnvironment.ContentRootPath + @"\UserFiles\BotHandler");
                    foreach (var i in botHandlerFiles)
                    {
                        zip.CreateEntryFromFile(i, "root/" + Path.GetFileName(i));
                    }
                }
                System.IO.File.Delete(botPath + @"\" + bot.BotName + ".bot");
                FileStream file = new FileStream(botPath + @"\bot.zip", FileMode.Open, FileAccess.Read, FileShare.None, 4096, FileOptions.DeleteOnClose);
                return File(file, "application/zip", "bot.zip");

            }
            catch (Exception e)
            {
                return View("Error", new ErrorModel(e.Message));
            }
        }

        [HttpGet]
        [Route("start")]
        public async Task<IActionResult> StartBot(int Id)
        {
            try
            {
                Models.User user = await db.Users.Include(x => x.Bots).ThenInclude(x => x.BotQueries)
                                                                  .ThenInclude(x => x.Response).FirstAsync(x => x.Name == User.Identity.Name);
                TelegramBot model;
                model = user.Bots.First(x => x.Id == Id);
                model.IsStarted = true;
                db.Bots.Update(model);
                await db.SaveChangesAsync();
                BotHandler.StartHandleAsync(model);
                return new NoContentResult();
            }
            catch (ArgumentException e)
            {
                return View("Error", new ErrorModel("Bad bot token"));
            }
        }

        [HttpGet]
        [Route("stop")]
        public async Task<IActionResult> StopBot(int Id)
        {
            try
            {
                Models.User user = await db.Users.Include(x => x.Bots).ThenInclude(x => x.BotQueries)
                                                                  .ThenInclude(x => x.Response).FirstAsync(x => x.Name == User.Identity.Name);
                TelegramBot model;
                model = user.Bots.First(x => x.Id == Id);
                model.IsStarted = false;
                db.Bots.Update(model);
                await db.SaveChangesAsync();
                BotHandler.StopHandle(model);
                return new NoContentResult();
            }
            catch
            {
                return View("Error", new ErrorModel("Bad bot index"));
            }
        }
    }
}
