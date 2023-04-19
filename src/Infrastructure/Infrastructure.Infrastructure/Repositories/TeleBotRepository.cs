using Application.Enums;
using Application.Interfaces.Repositories;

using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;

namespace Infrastructure.Infrastructure.Repositories
{
    public class TeleBotRepository<T> : ITeleBotRepository<T>
    {
        private readonly ILogger<TeleBotRepository<T>> _logger;
   
        public TeleBotRepository(ILogger<TeleBotRepository<T>> logger)
        {
            _logger = logger;
        }
        public async Task SendMess(T model,string text="")
        {
            try
            {
                //var bot = new TelegramBotClient("");
                //if (!string.IsNullOrEmpty(text))
                //{
                //    model.Content = text;
                //}
                //await bot.SendTextMessageAsync(model.ConfigRoomBot.ChatId, !string.IsNullOrEmpty(text)? text :model.Content, Telegram.Bot.Types.Enums.ParseMode.Html);

                _logger.LogInformation("RemindWork SendTextMessageAsync OK: " + JsonConvert.SerializeObject(model));

            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
       
            }
        }
    }
}
