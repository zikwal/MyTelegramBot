using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Telegram.Bot;
using Telegram.Bot.Types;
using System.Threading.Tasks;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputMessageContents;
using Telegram.Bot.Types.ReplyMarkups;
using Newtonsoft.Json;
using System.Web;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Microsoft.EntityFrameworkCore;

namespace MyTelegramBot.Messages
{
    public class MyOrdersMessage:Bot.BotMessage
    {
        private int FollowerId { get; set; }

        private int BotId { get; set; }
        public MyOrdersMessage(int FollowerId, int BotId)
        {
            this.FollowerId = FollowerId;
            this.BotId = BotId;
        }

        public MyOrdersMessage BuildMessage()
        {
            using(MarketBotDbContext db=new MarketBotDbContext())
            {
                List<Orders> orders = new List<Orders>();

                orders = db.Orders.Where(o => o.FollowerId == FollowerId && o.BotInfoId==BotId).Include(o=>o.OrderProduct).Include(o=>o.OrderDone).Include(o=>o.OrderConfirm).Include(o=>o.OrderDeleted).OrderBy(o=>o.Id).ToList();

                if (orders != null && orders.Count>0)
                {
                    base.TextMessage = Bold("Мои заказы") + NewLine();

                    int Counter = 1;

                    foreach (Orders order in orders)
                    {
                        if (order.OrderDeleted != null && order.OrderDeleted.Count == 0)
                        {
                            base.TextMessage += Counter.ToString() + ") Заказ №" + order.Number.ToString() + " от " + order.DateAdd.ToString() +
                                    NewLine() + "открыть /myorder" + order.Number.ToString() + NewLine();
                            Counter++;
                        }
                    }

                    base.TextMessage += NewLine() + "Главная /start";
                }

                else
                    base.CallBackTitleText = "У вас еще нет заказов";


                return this;
            }
        }
    }
}
