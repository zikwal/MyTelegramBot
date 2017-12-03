using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.EntityFrameworkCore;

namespace MyTelegramBot.Messages.Admin
{
    public class OrderMiniViewMessage:Bot.BotMessage
    {
        private int OrderId { get; set; }

        private InlineKeyboardCallbackButton OpenBtn { get; set; }

        public OrderMiniViewMessage(string Text, int OrderId)
        {
            this.OrderId = OrderId;

            base.TextMessage = Text;
        }

        public OrderMiniViewMessage BuildMessage()
        {
            OpenBtn = new InlineKeyboardCallbackButton("Открыть", BuildCallData("OpenOrder", OrderId));

            base.MessageReplyMarkup = new InlineKeyboardMarkup(new[] { new[] { OpenBtn } });

            return this;
        }
    }
}
