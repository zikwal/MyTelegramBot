using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;

namespace MyTelegramBot.Messages.Admin
{
    /// <summary>
    /// Сообщение со списком товаров в виде кнопок
    /// </summary>
    public class AdminProductListMessage:Bot.BotMessage
    {
        int CategoryId { get; set; }

        InlineKeyboardCallbackButton[][] ProductListBtn { get; set; }
        
        private List<Product> ProductList { get; set; }
        public AdminProductListMessage(int CategoryId)
        {
            this.CategoryId = CategoryId;
            base.BackBtn = new InlineKeyboardCallbackButton("Назад", BuildCallData(Bot.ProductEditBot.BackToAdminProductInCategoryCmd));
        }
        public AdminProductListMessage BildMessage()
        {
            base.TextMessage="Выберите товар";

            using(MarketBotDbContext db=new MarketBotDbContext())
                ProductList=db.Product.Where(p => p.CategoryId == CategoryId).ToList(); 

            if (ProductList != null && ProductList.Count() > 0)
            {
                ProductListBtn = new InlineKeyboardCallbackButton[ProductList.Count() + 1][];

                int counter = 0;

                foreach (Product product in ProductList)
                {
                    ProductListBtn[counter] = new InlineKeyboardCallbackButton[1];
                    ProductListBtn[counter][0] = new InlineKeyboardCallbackButton(product.Name, BuildCallData("SelectProduct", product.Id));
                    counter++;

                }

                ProductListBtn[ProductList.Count()] = new InlineKeyboardCallbackButton[1];
                ProductListBtn[ProductList.Count()][0] = BackBtn;
            }

            else
            {
                ProductListBtn = new InlineKeyboardCallbackButton[1][];
                ProductListBtn[0] = new InlineKeyboardCallbackButton[1];
                ProductListBtn[0][0] = BackBtn;
            }
            base.MessageReplyMarkup = new InlineKeyboardMarkup(ProductListBtn);
            return this;
        }
    }
}
