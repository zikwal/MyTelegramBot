using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputMessageContents;
using System.Data.SqlClient;
using Newtonsoft.Json;
using MyTelegramBot.Bot;

namespace MyTelegramBot.Messages
{
    /// <summary>
    /// Результат поиска через встраиваемый режим
    /// </summary>
    public class ProductSearchInline
    {

        private string Query { get; set; }

        private InlineKeyboardCallbackButton OpenBtn { get; set; }

        public ProductSearchInline(string Query)
        {
            this.Query = Query;
        }

        public InlineQueryResult[] ProductInlineSearch()
        {
            List<Product> product = new List<MyTelegramBot.Product>();
            SqlParameter param = new SqlParameter("@name", "%" + Query + "%");
            using (MarketBotDbContext db = new MarketBotDbContext())
                product = db.Product.FromSql("SELECT * FROM Product WHERE Name LIKE @name and Enable=1", param).
                    Include(p=>p.ProductPrice).Include(p=>p.Stock).Include(p=>p.ProductPrice).Include(p=>p.Unit).ToList();


            InputTextMessageContent[] textcontent = new InputTextMessageContent[product.Count];
            InlineQueryResultArticle[] article = new InlineQueryResultArticle[product.Count];
            InlineQueryResult[] result = new InlineQueryResult[product.Count];
            

            for (int i = 0; i < product.Count; i++)
            {
                textcontent[i] = new InputTextMessageContent();
                textcontent[i].DisableWebPagePreview = true;
                textcontent[i].MessageText = product[i].ToString();
                

                article[i] = new InlineQueryResultArticle();
                article[i].HideUrl = true;
                article[i].Id = product[i].Id.ToString();
                article[i].Title = product[i].Name;
                article[i].Description = product[i].Name+" " +product[i].ProductPrice.Where(p=>p.Enabled).FirstOrDefault().Value.ToString() +" руб." + "\r\nНажмите сюда";
                article[i].ThumbUrl = product[i].PhotoUrl;
                article[i].Url = product[i].TelegraphUrl;
                article[i].InputMessageContent = textcontent[i];
                article[i].ReplyMarkup  = new InlineKeyboardMarkup(
                    new[]{
                    new[]
                    {
                        OpenBtn=new InlineKeyboardCallbackButton("Открыть",BuildCallData("GetProduct",product[i].Id))
                    }
                    });
                result[i] = new InlineQueryResult();
                result[i] = article[i];
            }
            return result;
        }

        private string BuildCallData(string CommandName, params int[] Argument)
        {
            BotCommand command = new BotCommand
            {
                Cmd = CommandName,
                Arg = new List<int>()
            };

            for (int i = 0; i < Argument.Length; i++)
                command.Arg.Add(Argument[i]);

            return JsonConvert.SerializeObject(command);
        }

    }
}
