using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.EntityFrameworkCore;
using MyTelegramBot.Bot;
using MyTelegramBot.Messages.Admin;
using MyTelegramBot.Messages;
using MyTelegramBot.Bot.AdminModule;

namespace MyTelegramBot.Messages.Admin
{
    public class AdminPayMethodsSettings:Bot.BotMessage
    {
        private InlineKeyboardCallbackButton [][] MethodsBtns { get; set; }

        const string CheckEmodji = "\u2714\ufe0f";

        const string UnCheckEmodji = "\ud83d\udd32";

        public AdminPayMethodsSettings BuildMessage()
        {
            using (MarketBotDbContext db=new MarketBotDbContext())
            {
                var methods = db.PaymentType.ToList();

                MethodsBtns = new InlineKeyboardCallbackButton[methods.Count][];

                base.TextMessage = "Выберите доступые методы оплаты." + NewLine() +
                                   Italic("Должен быть доступен, как минимум один метод оплаты")+NewLine()
                                   +"Вернуться в панель администратора /admin";

                int counter = 0;
                foreach (PaymentType pt in methods)
                {
                    MethodsBtns[counter]= new InlineKeyboardCallbackButton[1];

                    if (pt.Enable==true)
                        MethodsBtns[counter][0] = new InlineKeyboardCallbackButton(pt.Name+" "+CheckEmodji, BuildCallData(AdminBot.PaymentTypeEnableCmd, pt.Id));

                    else
                        MethodsBtns[counter][0] = new InlineKeyboardCallbackButton(pt.Name + " "+ UnCheckEmodji, BuildCallData(AdminBot.PaymentTypeEnableCmd, pt.Id));

                    counter++;
                }

                base.MessageReplyMarkup = new InlineKeyboardMarkup(MethodsBtns);
            }

            return this;
        }
    }
}
