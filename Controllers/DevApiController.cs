using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BotCreator.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebTelegramBotsBuilder.Models;
using WebTelegramBotsBuilder.Models.ApiResponses;

namespace WebTelegramBotsBuilder.Controllers
{
    [Route("/api")]
    [ApiController]
    public class DevApiController : ControllerBase
    {
        private MainContext db;

        public DevApiController(MainContext context)
        {
            db = context;
        }

        [NonAction]
        private string ToMD5Hash(string Input)
        {
            using (var md5 = MD5.Create())
            {
                byte[] result = md5.ComputeHash(Encoding.UTF8.GetBytes(Input));
                return Encoding.UTF8.GetString(result);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult<ApiResponse>> Auth(string Username, string Password)
        {

            Models.User user = await db.Users.FirstOrDefaultAsync(u => u.Name == Username && u.Password == ToMD5Hash(Password));
            if (user != null)
            {
                return new ApiAuthResponse()
                {
                    Status = true,
                    ApiToken = user.ApiToken
                };
            }
            else return new ApiRequestResponse() { Status = false, Message = "Wrong username or password" };
            
        }

        //bots methods
        
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<ApiResponse>> GetBots(string Token)
        {
            Models.User user = await db.Users.Include(b => b.Bots).ThenInclude(q => q.BotQueries).ThenInclude(r => r.Response).FirstOrDefaultAsync(u => u.ApiToken == Token);
            if (user != null) 
            {
                return new ApiBotsResponse()
                {
                    Status = true,
                    Bots = user.Bots
                };
            }
            else return new ApiRequestResponse() { Status = false, Message = "Wrong token" };
        }

        [HttpPost]
        [Route("[action]")]
#pragma warning disable CS8632
        public async Task<ActionResult<ApiResponse>> UpdateBot(string Token, int BotId, string? NewName, string? NewToken)
#pragma warning restore CS8632
        {
            Models.User user = await db.Users.Include(b => b.Bots).FirstOrDefaultAsync(u => u.ApiToken == Token);
            if (user != null)
            {
                TelegramBot bot = user.Bots.FirstOrDefault(b => b.Id == BotId);
                if (bot != null) 
                {
                    if(NewName != null)
                    {
                        bot.BotName = NewName;
                    }
                    if(NewToken != null)
                    {
                        bot.BotToken = NewToken;
                    }
                    db.Bots.Update(bot);
                    await db.SaveChangesAsync();
                    return new ApiRequestResponse() { Status = true, Message = "Changes apply" };
                }
                else return new ApiRequestResponse() { Status = false, Message = "Wrong bot id" };
            }
            else return new ApiRequestResponse() { Status = false, Message = "Wrong token" };
        }

        [HttpDelete]
        [Route("[action]")]
        public async Task<ActionResult<ApiResponse>> DeleteBot(string Token, int BotId)
        {
            Models.User user = await db.Users.Include(b => b.Bots).FirstOrDefaultAsync(u => u.ApiToken == Token);
            if(user != null)
            {
                TelegramBot bot = user.Bots.FirstOrDefault(b => b.Id == BotId);
                if(bot != null)
                {
                    db.Bots.Remove(bot);
                    await db.SaveChangesAsync();
                    return new ApiRequestResponse() { Status = true, Message = "Bot was deleted" };
                }
                else return new ApiRequestResponse() { Status = false, Message = "Wrong bot id" };
            }
            else return new ApiRequestResponse() { Status = false, Message = "Wrong token" };
        }

        [HttpPut]
        [Route("[action]")]
        public async Task<ActionResult<ApiResponse>> CreateBot(string Token, string BotName, string BotToken)
        {
            if(BotName == null || BotToken == null)
            {
                return new ApiRequestResponse() { Status = false, Message = "BotName and BotToken is required" };
            }
            Models.User user = await db.Users.Include(b => b.Bots).FirstOrDefaultAsync(u => u.ApiToken == Token);
            if (user != null)
            {
                if (BotName.Length > 12 || BotToken.Length > 32)
                {
                    return new ApiRequestResponse() { Status = false, Message = "Bot name or bot token too long" };
                }
                else
                {
                    TelegramBot bot = new TelegramBot(BotToken, BotName);
                    (user.Bots as List<TelegramBot>).Add(bot);
                    db.Users.Update(user);
                    await db.SaveChangesAsync();
                    return new ApiBotsResponse()
                    {
                        Status = true,
                        Bots = user.Bots
                    };
                }
            }
            else return new ApiRequestResponse() { Status = false, Message = "Wrong token" };
        }
    }
}
