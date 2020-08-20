using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebTelegramBotsBuilder.Models.ApiResponses
{
    public class ApiBotsResponse : ApiResponse
    {
        public IEnumerable<BotCreator.Core.TelegramBot> Bots { get; set; }
    }
}
