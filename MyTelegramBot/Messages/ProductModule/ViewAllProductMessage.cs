using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.EntityFrameworkCore;

namespace MyTelegramBot.Messages
{
    /// <summary>
    /// Весь ассортимент одним сообщением
    /// </summary>
    public class ViewAllProductMessage:Bot.BotMessage
    {
        private List<Product> Products { get; set; }

        private List<IGrouping<int,Category>> Categorys { get; set; }

        public ViewAllProductMessage()
        {
            BackBtn = new InlineKeyboardCallbackButton("Назад", BuildCallData("BackCategoryList"));
        }

        public ViewAllProductMessage BuildMessage()
        {
            try
            {
                using (MarketBotDbContext db = new MarketBotDbContext())
                {
                    Products = db.Product.Where(p => p.Enable == true).Include(p => p.ProductPrice).Include(p => p.Category).Include(p => p.Stock).ToList();
                    Categorys = db.Category.Where(c => c.Enable).Include(c => c.Product).GroupBy(c => c.Id).ToList();
                }
                string message = Bold("Ассортимент");

                foreach (var cat in Categorys)
                {
                    List<Category> list = cat.ToList();

                    foreach (Category category in list)
                    {
                        int counter = 1;
                        message += NewLine() + NewLine() + Italic(category.Name);
                        foreach (Product product in category.Product)
                        {

                            if (product.Enable==true) // провереряем указана ли цена
                            {
                                message += NewLine() + counter.ToString() + ") " + product.Name + "  " + product.ProductPrice.Where(p => p.Enabled).FirstOrDefault().Value + " руб.";

                                if (product.Stock != null && product.Stock.Count == 0 ||
                                product.Stock != null && product.Stock.Count > 0 && product.Stock.OrderByDescending(s => s.Id).FirstOrDefault().Balance == 0)
                                    message += NewLine() + "нет в наличии";

                                message += NewLine() + "Показать: /product" + product.Id.ToString() + NewLine();

                                counter++;
                            }
                        }

                    }

                }

                message += NewLine() + Italic("для поиска используйте Inline-режим. @НАЗВАНИЕБОТА запрос");
                base.TextMessage = message;

                base.MessageReplyMarkup = new InlineKeyboardMarkup(
                    new[]{
                new[]
                        {
                            BackBtn
                        },

                     });

                return this;
            }

            catch (Exception exp)
            {
                return null;
            }
        }



    }
}
