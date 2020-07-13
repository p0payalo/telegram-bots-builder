using BotCreator.Core;
using BotCreator.Core.BotQueries;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace WebTelegramBotsBuilder.Models.Helpers
{
    public static class BotHandler
    {
        private static Dictionary<TelegramBotClient, TelegramBot> Bots = new Dictionary<TelegramBotClient, TelegramBot>();

        private static Dictionary<string, string> patterns = new Dictionary<string, string>()
        {
            ["username"] = "[username]",
            ["firstname"] = "[firstname]",
            ["lastname"] = "[lastname]",
            ["date"] = @"\[date=((\D\D-\D\D|\D\D),|)(.+?)\]",
            ["time"] = @"\[time=((.*),|)(.+?)\]",
            ["random"] = @"\[random=(\d{1,9})-(\d{1,9})\]",
            ["or"] = @"\[or=(.+?)\]"
        };

        public static void StartHandle(TelegramBot bot)
        {
            TelegramBotClient client;
            try
            {
                client = new TelegramBotClient(bot.BotToken);
                client.OnMessage += OnMessage;
                Bots.Add(client, bot);
                client.StartReceiving();
            }
            catch(ArgumentException e)
            {
                client = null;
            }
        }

        public static async void StartHandleAsync(TelegramBot bot)
        {
            await Task.Run(() => StartHandle(bot));
        }

        public static void StopHandle(TelegramBot bot)
        {
            TelegramBotClient target = Bots.FirstOrDefault(x => x.Value.BotToken == bot.BotToken).Key;
            if (target != null)
            {
                target.StopReceiving();
                Bots.Remove(target);
            }
        }

        private static void OnMessage(object sender, MessageEventArgs e)
        {
            TelegramBot invoker = Bots[sender as TelegramBotClient];
            if (e.Message.Type == Telegram.Bot.Types.Enums.MessageType.Text)
            {
                List<BotQuery> botQueries = invoker.BotQueries.Where(x => x.QueryType == MessageType.Text).ToList();
                BotQuery target = null;
                foreach (var query in botQueries)
                {
                    if (query.Value.Equals(e.Message.Text))
                    {
                        target = query;
                        break;
                    }
                }
                if (target != null)
                {
                    (sender as TelegramBotClient).SendTextMessageAsync(e.Message.Chat.Id, FormatString(target.Response.Value, e.Message.From));
                }
            }
        }

        private static string FormatString(string source, Telegram.Bot.Types.User data)
        {
            source = StringFormatter.FormatString(patterns["username"], source, data.Username == null ? "username" : data.Username);
            source = StringFormatter.FormatString(patterns["firstname"], source, data.Username == null ? "First Name" : data.FirstName);
            source = StringFormatter.FormatString(patterns["lastname"], source, data.Username == null ? "Last Name" : data.LastName);
            source = StringFormatter.FormatStringWithRandom(patterns["random"], source);
            source = StringFormatter.FormatStringWithOr(patterns["or"], source);

            string culture = Regex.Match(source, patterns["date"]).Groups[2].Value;
            CultureInfo dateCulture;
            try
            {
                dateCulture = new CultureInfo(culture);
            }
            catch
            {
                dateCulture = new CultureInfo("en-US");
            }

            DateTime now = DateTime.Now;

            Dictionary<string, string> datePatternParams = new Dictionary<string, string>()
            {
                //order is important because when dd will be init above dddd, dddd will replace at dd value
                //i'm so lazy to use regexp 
                //probably i will fix it later
                ["dddd"] = now.ToString("dddd", dateCulture),
                ["ddd"] = now.ToString("ddd", dateCulture),
                ["dd"] = now.ToString("dd", dateCulture),
                ["MMMM"] = now.ToString("MMMM", dateCulture),
                ["MMM"] = now.ToString("MMM", dateCulture),
                ["MM"] = now.ToString("MM", dateCulture),
                ["yyyy"] = now.ToString("yyyy", dateCulture),
                ["yy"] = now.ToString("yy", dateCulture),
            };
            source = StringFormatter.FormatStringWithParams(patterns["date"], source, datePatternParams);

            Match timeReg = Regex.Match(source, patterns["time"]);
            now = DateTime.UtcNow;
            if (timeReg.Success)
            {
                try
                {
                    TimeZoneInfo timezone = TimeZoneInfo.FindSystemTimeZoneById(timeReg.Groups[2].Value);
                    now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timezone);
                }
                catch
                {
                    now = DateTime.UtcNow;
                }
            }
            Dictionary<string, string> timePatternParams = new Dictionary<string, string>()
            {
                ["HH"] = now.ToString("HH"),
                ["mm"] = now.ToString("mm"),
                ["ss"] = now.ToString("ss")
            };
            source = StringFormatter.FormatStringWithParams(patterns["time"], source, timePatternParams);

            return source;
        }
    }
}
