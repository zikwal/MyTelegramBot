using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types.InlineQueryResults;
using MyTelegramBot.Bot;
using Newtonsoft.Json;

namespace MyTelegramBot.Messages.Admin
{
    public class CurrencyListMessage:Bot.BotMessage
    {
        private InlineKeyboardCallbackButton [][] CurrencyBtns { get; set; }

        private int ProductId;
        public CurrencyListMessage(int ProductId)
        {
            this.ProductId = ProductId;
            BackBtn = new InlineKeyboardCallbackButton("Назад", BuildCallData("BackToProductEditor", ProductId));
        }

        public CurrencyListMessage BuildMessage()
        {
            using (MarketBotDbContext db=new MarketBotDbContext())
            {
                var cur = db.Currency.ToList();

                CurrencyBtns = new InlineKeyboardCallbackButton[cur.Count+1][];

                int counter = 0;
                foreach (Currency c in cur)
                {
                    CurrencyBtns[counter] = new InlineKeyboardCallbackButton[1];
                    CurrencyBtns[counter][0] = new InlineKeyboardCallbackButton(c.Name + "-" + c.ShortName, BuildCallData("UpdateProductCurrency", ProductId, c.Id));
                    counter++;
                }

                CurrencyBtns[cur.Count] = new InlineKeyboardCallbackButton[1];
                CurrencyBtns[cur.Count][0] = BackBtn;

                base.MessageReplyMarkup = new InlineKeyboardMarkup(CurrencyBtns);

                base.TextMessage = "Выберите валюту для цены";

                return this;
            }
        }
    }
}
