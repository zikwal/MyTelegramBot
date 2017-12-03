using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.EntityFrameworkCore;
using MyTelegramBot.Bot.AdminModule;
namespace MyTelegramBot.Messages.Admin
{
    public class NewFeedBackAddedMessage:Bot.BotMessage
    {
        private Orders Order { get; set; }

        private InlineKeyboardCallbackButton OpenOrderBtn { get; set; }

        public NewFeedBackAddedMessage(Orders order)
        {
            this.Order = order;
        }

        public NewFeedBackAddedMessage BuildMessage()
        {
            if (Order!=null)
            {
                base.TextMessage = Bold("Новый отзыв:") + NewLine() + "Добавлен отзыв к заказу №" + Order.Number.ToString();
                OpenOrderBtn = new InlineKeyboardCallbackButton("Посмотреть детали заказа", BuildCallData(OrderProccesingBot.CmdGetOrderAdmin, Order.Id));

                base.MessageReplyMarkup = new InlineKeyboardMarkup(
                new[]{
                new[]
                        {
                            OpenOrderBtn
                        },


                });

            }

            return this;
        }
    }
}
