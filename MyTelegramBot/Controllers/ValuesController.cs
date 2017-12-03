using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using MyTelegramBot.Bot;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using MyTelegramBot.Bot.AdminModule;
using Microsoft.Extensions.Configuration;

namespace MyTelegramBot.Controllers
{
    [Produces("application/json")]
    [Route("api/Values")]
    public class ValuesController : Controller
    {
        private CategoryBot Category { get; set; }

        private BotCommand Command { get; set; }

        private ProductBot Product { get; set; }

        private BasketBot Basket { get; set; }

        private AddressBot Address { get; set; }

        private OrderBot OrderBot { get; set; }

        private FollowerBot FollowerBot { get; set; }

        private OrderPositionBot PositionBot { get; set; }

        private ProductEditBot ProductEditBot { get; set; }

        private CategoryEditBot CategoryEditBot { get; set; }

        private MainMenuBot MainMenuBot { get; set; }

        private AdminBot AdminBot { get; set; }

        private ImportCSVBot ImportCSVBot { get; set; }

        private HelpDeskBot HelpDeskBot { get; set; }

        private Bot.AdminModule.OrderProccesingBot OrderProccesingBot { get; set; }

        private HelpDeskProccessingBot HelpDeskProccessingBot { get; set; }

        protected OkResult OkResult { get; set; }

        protected NotFoundResult NotFoundResult { get; set; }

        IActionResult Result { get; set; }
        // POST api/values
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Update update)
        {
            OkResult = this.Ok();
            NotFoundResult = this.NotFound();

            if (Result == null)
            {
                HelpDeskProccessingBot = new HelpDeskProccessingBot(update);
                Result = await HelpDeskProccessingBot.Response();
            }

            if (Result == null)
            {
                OrderProccesingBot = new OrderProccesingBot(update);
                Result = await OrderProccesingBot.Response();
            }

            if (Result == null)
            {
                Category = new CategoryBot(update);
                Result = await Category.Response();
            }

            if (Result == null)
            {
                Product = new ProductBot(update);
                Result = await Product.Response();
            }

            if (update.CallbackQuery != null && Result != OkResult)
            {
                Basket = new BasketBot(update);
                Result = await Basket.Response();
            }

            if (update.CallbackQuery != null && Result == null || update.Message != null && Result == null)
            {
                Address = new AddressBot(update);
                Result = await Address.Response();
            }

            if (update.CallbackQuery != null && Result == null || update.Message != null && Result == null)
            {
                OrderBot = new OrderBot(update);
                Result = await OrderBot.Response();
            }

            if (update.Message != null && Result == null)
            {
                FollowerBot = new FollowerBot(update);
                Result = await FollowerBot.Response();
            }

            if (update.CallbackQuery != null && Result == null)
            {
                PositionBot = new OrderPositionBot(update);
                Result = await PositionBot.Response();
            }

            if (Result == null)
            {
                AdminBot = new AdminBot(update);
                Result = await AdminBot.Response();
            }

            if (Result == null)
            {
                ProductEditBot = new ProductEditBot(update);
                Result = await ProductEditBot.Response();
            }

            if (Result == null)
            {
                CategoryEditBot = new CategoryEditBot(update);
                Result = await CategoryEditBot.Response();
            }

            if (Result == null)
            {
                MainMenuBot = new MainMenuBot(update);
                Result = await MainMenuBot.Response();
            }

            if (Result == null)
            {
                ImportCSVBot = new ImportCSVBot(update);
                Result = await ImportCSVBot.Response();
            }

            if (Result == null)
            {
                HelpDeskBot = new HelpDeskBot(update);
                Result = await HelpDeskBot.Response();
            }

            await AddUpdateMsgToDb(update);

            if (update.Message != null)
              await  UpdateFollowerInfo(update.Message.Chat);

            if(update.CallbackQuery!=null && update.CallbackQuery.Message!=null)
                await UpdateFollowerInfo(update.CallbackQuery.Message.Chat);


            if (Result == null || Result!=null)
                Result = Ok();


            return Result;
        }

         async Task<int> AddUpdateMsgToDb(Update update)
        {
            Follower follower = new Follower();
            string MessageId = "";
            int BotId = 0;
            try
            {
                using (MarketBotDbContext db = new MarketBotDbContext())
                {
                    var builder = new ConfigurationBuilder()
                    .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");
                    string name = builder.Build().GetSection("BotName").Value;

                    if (update.CallbackQuery != null && update.InlineQuery == null)
                    {
                        follower = await db.Follower.Where(f => f.ChatId == update.CallbackQuery.From.Id).FirstOrDefaultAsync();
                        MessageId = update.CallbackQuery.Message.MessageId.ToString();
                        
                 
                    }

                    if (update.Message != null && update.InlineQuery == null)
                    {
                        follower = await db.Follower.Where(f => f.ChatId == update.Message.From.Id).FirstOrDefaultAsync();
                        MessageId = update.Message.MessageId.ToString();

                    }

                    BotId = db.BotInfo.Where(b => b.Name == name).FirstOrDefault().Id;

                    if (follower!=null && db.TelegramMessage.Where(t => t.UpdateId == update.Id).FirstOrDefault() == null && BotId>0 && update.InlineQuery==null)
                    {
                        
                        TelegramMessage telegramMessage = new TelegramMessage
                        {
                            FollowerId = follower.Id,
                            MessageId = MessageId,
                            UpdateId = update.Id,
                            UpdateJson = JsonConvert.SerializeObject(update),
                            DateAdd=DateTime.Now,
                            BotInfoId=BotId
                        };

                        db.TelegramMessage.Add(telegramMessage);
                        return await db.SaveChangesAsync();
                    }

                    else
                        return -1;

                }
            }

            catch (Exception exp)
            {
                return -1;
            }
        }

        /// <summary>
        /// Проверяем информацию о пользователе. Если есть изменения, то вносим их в БД
        /// </summary>
        /// <returns></returns>
        protected async Task<int> UpdateFollowerInfo(Chat ChatInfo)
        {
            try
            {
                using (MarketBotDbContext db = new MarketBotDbContext())
                {
                    var follower = db.Follower.Where(f => f.ChatId == ChatInfo.Id).FirstOrDefault();

                    if (follower != null && follower.FirstName != ChatInfo.FirstName)
                    {
                        follower.FirstName = ChatInfo.FirstName;
                        return await db.SaveChangesAsync();
                    }

                    if (follower != null && follower.LastName != ChatInfo.LastName)
                    {
                        follower.LastName = ChatInfo.LastName;
                        return await db.SaveChangesAsync();
                    }

                    if (follower != null && follower.UserName != ChatInfo.Username)
                    {
                        follower.UserName = ChatInfo.Username;
                        return await db.SaveChangesAsync();
                    }

                    else
                        return -1;
                }
            }

            catch
            {
                return -1;
            }
        }
    }
}
