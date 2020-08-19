using BotCreator.Core;
using System.Collections.Generic;

namespace WebTelegramBotsBuilder.Models
{
    public class User
    {
        public User()
        {
            Bots = new List<TelegramBot>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string ApiToken { get; set; }
        public virtual IEnumerable<TelegramBot> Bots { get; set; }
    }
}
