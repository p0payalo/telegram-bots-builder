#pragma warning disable CS8632
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BotCreator.Core;
using BotCreator.Core.BotQueries;
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

        [NonAction]
        private ApiRequestResponse Error(string message)
        {
            return new ApiRequestResponse() { Status = false, Message = message };
        }

        [NonAction]
        private ApiRequestResponse Success(string message)
        {
            return new ApiRequestResponse() { Status = true, Message = message };
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
            else return Error("Wrong username or password");

        }

        //bots methods

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<ApiResponse>> GetBots(string Token)
        {
            Models.User user = await db.Users.Include(b => b.Bots).FirstOrDefaultAsync(u => u.ApiToken == Token);
            if (user != null)
            {
                return new ApiBotsResponse()
                {
                    Status = true,
                    Bots = user.Bots
                };
            }
            else return Error("Wrong token");
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult<ApiResponse>> UpdateBot(string Token, int BotId, string? NewName, string? NewToken)
        {
            Models.User user = await db.Users.Include(b => b.Bots).FirstOrDefaultAsync(u => u.ApiToken == Token);
            if (user != null)
            {
                TelegramBot bot = user.Bots.FirstOrDefault(b => b.Id == BotId);
                if (bot != null)
                {
                    if (NewName != null)
                    {
                        bot.BotName = NewName;
                    }
                    if (NewToken != null)
                    {
                        bot.BotToken = NewToken;
                    }
                    db.Bots.Update(bot);
                    await db.SaveChangesAsync();
                    return Success("Changes apply");
                }
                else return Error("Wrong bot id");
            }
            else return Error("Wrong token");
        }

        [HttpDelete]
        [Route("[action]")]
        public async Task<ActionResult<ApiResponse>> DeleteBot(string Token, int BotId)
        {
            Models.User user = await db.Users.Include(b => b.Bots).FirstOrDefaultAsync(u => u.ApiToken == Token);
            if (user != null)
            {
                TelegramBot bot = user.Bots.FirstOrDefault(b => b.Id == BotId);
                if (bot != null)
                {
                    db.Bots.Remove(bot);
                    await db.SaveChangesAsync();
                    return Success("Bot was deleted");
                }
                else return Error("Wrong bot id");
            }
            else return Error("Wrong token");
        }

        [HttpPut]
        [Route("[action]")]
        public async Task<ActionResult<ApiResponse>> CreateBot(string Token, string BotName, string BotToken)
        {
            if (BotName == null || BotToken == null)
            {
                return Error("BotName and BotToken is required");
            }
            if (BotName.Length > 12 || BotToken.Length > 32)
            {
                return Error("Bot name or bot token is too long");
            }
            Models.User user = await db.Users.Include(b => b.Bots).FirstOrDefaultAsync(u => u.ApiToken == Token);
            if (user != null)
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
            else return Error("Wrong token");
        }

        //queries methods

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<ApiResponse>> GetQueries(string Token, int BotId)
        {
            Models.User user = await db.Users.Include(b => b.Bots).FirstOrDefaultAsync(u => u.ApiToken == Token);
            if (user != null)
            {
                TelegramBot bot = user.Bots.FirstOrDefault(b => b.Id == BotId);
                if (bot != null)
                {
                    return new ApiQueriesResponse() { Status = true, Queries = bot.BotQueries };
                }
                else return Error("Wrong bot id");
            }
            else return Error("Wrong token");
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult<ApiResponse>> UpdateQuery(string Token, int BotId, int QueryId, string? Query, string? Response)
        {
            Models.User user = await db.Users.Include(b => b.Bots).FirstOrDefaultAsync(u => u.ApiToken == Token);
            if (user != null)
            {
                TelegramBot bot = user.Bots.FirstOrDefault(b => b.Id == BotId);
                if (bot != null)
                {
                    BotCreator.Core.BotQueries.BotQuery query = bot.BotQueries.FirstOrDefault(q => q.Id == QueryId);
                    if (query != null)
                    {
                        if (Query != null)
                        {
                            query.Value = Query;
                        }
                        if (Response != null)
                        {
                            query.Response.Value = Response;
                        }
                        db.BotQueries.Update(query);
                        await db.SaveChangesAsync();
                        return Success("Changes apply.");
                    }
                    else return Error("Wrong quuery id");
                }
                else return Error("Wrong bot id");
            }
            else return Error("Wrong token");
        }

        [HttpDelete]
        [Route("[action]")]
        public async Task<ActionResult<ApiResponse>> DeleteQuery(string Token, int BotId, int QueryId)
        {
            Models.User user = await db.Users.Include(b => b.Bots).FirstOrDefaultAsync(u => u.ApiToken == Token);
            if (user != null)
            {
                TelegramBot bot = user.Bots.FirstOrDefault(b => b.Id == BotId);
                if (bot != null)
                {
                    BotCreator.Core.BotQueries.BotQuery query = bot.BotQueries.FirstOrDefault(q => q.Id == QueryId);
                    if (query != null)
                    {
                        db.BotQueries.Remove(query);
                        await db.SaveChangesAsync();
                        return Success("Query was deleted");
                    }
                    else return Error("Wrong query id");
                }
                else return Error("Wrong bot id");
            }
            else return Error("Wrong token");
        }

        [HttpPut]
        [Route("[action]")]
        public async Task<ActionResult<ApiResponse>> CreateQuery(string Token, int BotId, string Query, string Response)
        {
            Models.User user = await db.Users.Include(b => b.Bots).FirstOrDefaultAsync(u => u.ApiToken == Token);
            if (user != null)
            {
                TelegramBot bot = user.Bots.FirstOrDefault(b => b.Id == BotId);
                if (bot != null)
                {
                    bot.BotQueries.Add(new BotQuery(Query, new BotResponse(Response, MessageType.Text), MessageType.Text));
                    db.Bots.Update(bot);
                    await db.SaveChangesAsync();
                    return new ApiQueriesResponse()
                    {
                        Status = true,
                        Queries = bot.BotQueries
                    };
                }
                else return Error("Wrong bot id");
            }
            else return Error("Wrong token");
        }
    }
}
