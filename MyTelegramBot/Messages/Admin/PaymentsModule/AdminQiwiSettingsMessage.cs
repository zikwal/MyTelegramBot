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
    public class AdminQiwiSettingsMessage:Bot.BotMessage
    {
        private InlineKeyboardCallbackButton AddBtn { get; set; }

        private InlineKeyboardCallbackButton EditBtn { get; set; }

        public AdminQiwiSettingsMessage BuildMessage()
        {
            string mess = "";

            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                var api = db.QiwiApi.Where(q => q.Enable == true).OrderByDescending(q=>q.Id).FirstOrDefault();

                if (api != null)
                {
                    mess =Bold("Номер телефона: ") + api.Telephone + NewLine() + Bold("Ключ: ") + api.Token + 
                        NewLine() +Bold("Дата добавления: ") + api.DateAdd.ToString() +
                        NewLine()+ Italic("Что такое QIWI API и где взять ключ ? ")+ "/whatisqiwiapi "+
                        NewLine() + "Вернуться в панель администратора /admin";

                    EditBtn = new InlineKeyboardCallbackButton("Изменить", BuildCallData(AdminBot.QiwiEditCmd));

                    base.MessageReplyMarkup = new InlineKeyboardMarkup(
                        new[]{
                        new[]
                        {
                            EditBtn
                        } });
                   
                 }

                else
                {
                    mess = "Данные отсутствуют Нажмите кнопку добавить" +
                        NewLine() + Italic("Что такое QIWI API и где взять ключ ? ") + "/whatisqiwiapi " +
                        NewLine() + "Вернуться в панель администратора /admin";
                    AddBtn = new InlineKeyboardCallbackButton("Добавить", BuildCallData(AdminBot.QiwiAddEdit));

                    base.MessageReplyMarkup = new InlineKeyboardMarkup(
                        new[]{
                        new[]
                        {
                            AddBtn
                        } });
                }

            }

            TextMessage = mess;

            return this;
           
        }
    }
}
