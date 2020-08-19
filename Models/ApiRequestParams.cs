using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebTelegramBotsBuilder.Models
{
    public class ApiRequestParams
    {
        public int BotId { get; set; }
        public int QueryId { get; set; }
        public string QueryText { get; set; }
        public string ResponseText { get; set; }
        public string BotName { get; set; }
        public string BotToken { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
