using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.EntityFrameworkCore;

namespace MyTelegramBot.Messages.Admin
{
    public class AdminAllProductsViewMessage:Bot.BotMessage
    {
        private List<Product> products { get; set; }

        private List<IGrouping<int, Category>> Categorys { get; set; }

        public AdminAllProductsViewMessage BuildMessage()
        {
            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                products = db.Product.Where(p => p.Enable == true).Include(p => p.ProductPrice).Include(p => p.Category).Include(p => p.Stock).ToList();
                Categorys = db.Category.Where(c => c.Enable).Include(c => c.Product).GroupBy(c => c.Id).ToList();
            }
            string message = Bold("Список всех товаров");

            foreach (var cat in Categorys)
            {
                List<Category> list = cat.ToList();

                foreach (Category category in list)
                {
               
                    int counter = 1;
                    message += NewLine() + NewLine() + Italic(category.Name);
                    foreach (Product product in category.Product)
                    {
                        ProductPrice price = product.ProductPrice.Where(p => p.Enabled).FirstOrDefault();

                        if (product.Enable == true && price!=null)
                        {
                            message += NewLine() + 
                                       counter.ToString() + ") " + product.Name + "  " + price.Value + " руб." + NewLine()+
                                       "Изменить " + Bot.ProductEditBot.SetProductCmd + product.Id.ToString() + NewLine();

                            counter++;
                        }
                    }


                }

            }

            base.TextMessage = message + NewLine() + "Панель администратора /admin";


            return this;
        }
    }
}
