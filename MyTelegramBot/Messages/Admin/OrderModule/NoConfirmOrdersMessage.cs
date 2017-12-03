using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.EntityFrameworkCore;
using MyTelegramBot.Bot;
using System.Data.SqlClient;

namespace MyTelegramBot.Messages.Admin
{
    public class NoConfirmOrdersMessage:BotMessage
    {

        public NoConfirmOrdersMessage()
        {
            BackBtn = new InlineKeyboardCallbackButton("Назад", BuildCallData("BackToAdminPanel"));
        }


        public NoConfirmOrdersMessage BuildMessage()
        {
            List<Orders> orders = new List<Orders>();
            using (MarketBotDbContext db=new MarketBotDbContext())
            {
                orders = db.Orders.FromSql("SELECT Orders.* FROM Orders LEFT JOIN OrderConfirm ON Orders.Id=OrderConfirm.OrderId LEFT JOIN OrderDeleted ON OrderDeleted.OrderId=Orders.Id WHERE OrderConfirm.OrderId IS NULL and OrderDeleted.OrderId is NULL").ToList();

            }

            string message = "";
            int counter = 1;
            foreach (Orders order in orders)
            {
                message += counter.ToString() + ") Заказ № " + order.Number.ToString() + " /order" + order.Number + NewLine();
            }

            base.MessageReplyMarkup = new InlineKeyboardMarkup(
                new[]{
                new[]
                        {
                            BackBtn
                        },
                 });

            base.TextMessage = "Необработанные заказы:"+NewLine()+message;
            return this;
        }
    }
}
