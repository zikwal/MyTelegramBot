using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types.ReplyMarkups;
using MyTelegramBot.Bot.AdminModule;

namespace MyTelegramBot.Messages.Admin
{
    /// <summary>
    /// Сообщение с названием товара и кнопками +/- 
    /// </summary>
    public class OrderPositionEditMessage:Bot.BotMessage
    {

        private InlineKeyboardCallbackButton AddBtn { get; set; }

        private InlineKeyboardCallbackButton RemoveBtn { get; set; }

        private int PositionId { get; set; }


        private OrderProduct orderProduct { get; set; }

        public OrderPositionEditMessage(int PositionId)
        {
            this.PositionId = PositionId;
        }

        public OrderPositionEditMessage BuildMessage ()
        {
            using(MarketBotDbContext db=new MarketBotDbContext())
                orderProduct=db.OrderProduct.Where(o => o.Id == PositionId).Include(o => o.Product).FirstOrDefault();            

            base.TextMessage = orderProduct.Product.Name + NewLine() + "Количество:" + orderProduct.Count.ToString();

            base.CallBackTitleText = orderProduct.Product.Name;

            AddBtn = new InlineKeyboardCallbackButton("+", BuildCallData(Bot.OrderPositionBot.AddToPositionCmd, PositionId));

            RemoveBtn = new InlineKeyboardCallbackButton("-", BuildCallData(Bot.OrderPositionBot.RemoveFromPositionCmd, PositionId));

            BackBtn = new InlineKeyboardCallbackButton("Назад", BuildCallData(OrderProccesingBot.CmdGetOrderAdmin, orderProduct.OrderId));

            SetInlineKeyBoard();

            return this;
        }

        private void SetInlineKeyBoard()
        {
            base.MessageReplyMarkup = new InlineKeyboardMarkup(
                new[]{
                new[]
                        {
                            RemoveBtn, AddBtn
                        },
                new[]
                        {
                            BackBtn
                        }

                 });
        }
    }
}
