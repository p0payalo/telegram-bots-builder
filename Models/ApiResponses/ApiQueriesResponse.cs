using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebTelegramBotsBuilder.Models.ApiResponses
{
    public class ApiQueriesResponse : ApiResponse
    {
        public IEnumerable<BotCreator.Core.BotQueries.BotQuery> Queries { get; set; }
    }
}
