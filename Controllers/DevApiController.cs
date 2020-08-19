using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
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

        /*[HttpPost]
        public JsonResult ApiRequest(string Token, string MethodName, ApiRequestParams requestParams)
        {
            return new JsonResult("");
        }*/

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult<ApiResponse>> Auth(string Username, string Password)
        {

            Models.User user = await db.Users.FirstOrDefaultAsync(u => u.Name == Username && u.Password == ToMD5Hash(Password));
            if (user != null)
            {
                return new ApiAuthResponse()
                {
                    Status = "ok",
                    ApiToken = user.ApiToken
                };
            }
            else return new ApiResponse() { Status = "bad" };
            
        }
    }
}
