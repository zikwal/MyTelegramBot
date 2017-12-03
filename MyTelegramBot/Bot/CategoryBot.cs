using System;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot.Types;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyTelegramBot.Messages;

namespace MyTelegramBot.Bot
{
    public class CategoryBot:BotCore
    {
        private int CategoryId { get; set; }

        private Category Category { get; set; }

        ProductViewMessage ProductViewMsg { get; set; }

        ViewAllProductMessage ViewAllProductMsg { get; set; }

        public CategoryBot(Update _update) : base(_update)
        {
           
        }

        protected override void Constructor()
        {
            try
            {
                if (this.Argumetns.Count > 0)
                {
                    CategoryId = Argumetns[0];
                    using (MarketBotDbContext db = new MarketBotDbContext())
                        Category = db.Category.Where(c => c.Id == this.CategoryId).FirstOrDefault();

                    ProductViewMsg = new ProductViewMessage(Category,BotInfo.Id);
                }

                ViewAllProductMsg = new ViewAllProductMessage();
            }

            catch
            {

            }
        }
        public async override Task<IActionResult> Response()
        {
        
            switch (base.CommandName)
            {
                case "Menu":
                    return await GetCategoryList();

                case "ReturnToCatalogList":
                    return await GetCategoryList();

                case "ProductInCategory":
                     return await GetProduct();

                case "ViewAllProduct":
                    return await GetAllProduct();

                case "BackCategoryList":
                    return await GetCategoryList(base.MessageId);

                default:
                    return null;
                    
            }
               
               
        }


        private async Task<IActionResult> GetCategoryList(int MessageId=0)
        {
            CategoryListMessage categoryListMessage = new CategoryListMessage();
            if (await SendMessage(categoryListMessage.BuildMessage(),MessageId) != null)
                return base.OkResult;

            else
                return base.NotFoundResult;

        }

        private async Task<IActionResult> GetProduct()
        {
            var message = ProductViewMsg.BuildMessage();

            if (await SendPhoto(message) != null)
                return base.OkResult;

            else
                return base.NotFoundResult;
        }

        private async Task<IActionResult> GetAllProduct()
        {

            if (await EditMessage(ViewAllProductMsg.BuildMessage()) != null)
                return base.OkResult;

            else
                return base.NotFoundResult;
        }
    }
}
