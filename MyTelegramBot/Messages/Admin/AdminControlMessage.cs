﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.EntityFrameworkCore;
using MyTelegramBot.Bot;
using MyTelegramBot.Messages.Admin;
using MyTelegramBot.Messages;

namespace MyTelegramBot.Messages.Admin
{
    /// <summary>
    /// Сообщение со списком операторов
    /// </summary>
    public class AdminControlMessage:BotMessage
    {
        private InlineKeyboardCallbackButton NewOperatorBtn { get; set; }

        public AdminControlMessage BuildMessage()
        {
            base.TextMessage =Bold("Список операторов системы:");
            string OperatorsList = "";
            using (MarketBotDbContext db=new MarketBotDbContext())
            {
               var operators= db.Admin.Where(a => a.Enable).Include(a=>a.Follower).ToList();            

                if (operators != null)
                {
                    int counter = 1;

                    foreach(var op in operators)
                    {
                        OperatorsList+=NewLine()+ counter.ToString() + ") " + op.Follower.FirstName + " | телефон: " + op.Follower.Telephone+
                            NewLine()+ "Отстранить: /removeoperator"+op.Id.ToString()+NewLine();
                        counter++;
                    }
                }
            }

            base.TextMessage += OperatorsList+ NewLine() + Italic("Оператор системы имеет следующие права доступа: Обрабатывать заказы, Обрабатывать заявки технической поддержки.");
            NewOperatorBtn = new InlineKeyboardCallbackButton("Создать оператора", BuildCallData("GenerateKey"));
            base.MessageReplyMarkup = new InlineKeyboardMarkup(
                new[]{
                new[]
                        {
                            NewOperatorBtn
                        },


                 });

            return this;
        }
    }
}
