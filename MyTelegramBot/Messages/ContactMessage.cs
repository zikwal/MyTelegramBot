﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;

namespace MyTelegramBot.Messages
{
    public class ContactMessage:Bot.BotMessage
    {
        private InlineKeyboardUrlButton VkBtn { get; set; }

        private InlineKeyboardUrlButton InstagramBtn { get; set; }

        private InlineKeyboardUrlButton ChatBtn { get; set; }

        private InlineKeyboardUrlButton ChannelBtn { get; set; }

        private Company Company { get; set; }

        public ContactMessage()
        {
            using (MarketBotDbContext db=new MarketBotDbContext())
                Company = db.Company.Where(c => c.Enable == true).OrderByDescending(o => o.Id).FirstOrDefault();            
            
        }

        public ContactMessage BuildMessage()
        {
            if(Company.Vk!="")
                VkBtn = new InlineKeyboardUrlButton("Vk.com", Company.Vk);

            else
                VkBtn = new InlineKeyboardUrlButton("Vk.com", "https://vk.com/");

            if(Company.Instagram!="")
                InstagramBtn = new InlineKeyboardUrlButton("Instagram", Company.Instagram );

            else
                InstagramBtn = new InlineKeyboardUrlButton("Instagram", "https://www.instagram.com/");

            if(Company.Chanel!="")
                ChannelBtn = new InlineKeyboardUrlButton("Канал в телеграм", Company.Chanel);

            else
                ChannelBtn = new InlineKeyboardUrlButton("Канал в телеграм", "https://t.me/");

            if(Company.Chat!="")
                ChatBtn = new InlineKeyboardUrlButton("Чат в телеграм", Company.Chat);

            else
                ChatBtn = new InlineKeyboardUrlButton("Чат в телеграм", "https://t.me/");

            base.TextMessage = "Контакты";

            SetInlineKeyBoard();

            return this;
        }

        private void SetInlineKeyBoard()
        {
            if(Company.Chanel!="" && Company.Chat!="" && Company.Instagram!="" && Company.Vk!="")
            base.MessageReplyMarkup = new InlineKeyboardMarkup(
                new[]{
                new[]
                        {
                            VkBtn
                        },
                new[]
                        {
                            InstagramBtn
                        },
                new[]
                        {
                            ChannelBtn
                        },
                new[]
                        {
                            ChatBtn
                        },

                 });

            if (Company.Chanel == "" && Company.Chat != "" && Company.Instagram != "" && Company.Vk != "")
                base.MessageReplyMarkup = new InlineKeyboardMarkup(
                    new[]{
                new[]
                        {
                            VkBtn
                        },
                new[]
                        {
                            InstagramBtn
                        },

                new[]
                        {
                            ChatBtn
                        },

                     });

            if (Company.Chanel != "" && Company.Chat == "" && Company.Instagram != "" && Company.Vk != "")
                base.MessageReplyMarkup = new InlineKeyboardMarkup(
                    new[]{
                new[]
                        {
                            VkBtn
                        },
                new[]
                        {
                            InstagramBtn
                        },

                     });
        }
    }
}
