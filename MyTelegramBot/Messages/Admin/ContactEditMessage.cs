using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.EntityFrameworkCore;
using MyTelegramBot.Bot;

namespace MyTelegramBot.Messages.Admin
{
    public class ContactEditMessage:Bot.BotMessage
    {
        private InlineKeyboardCallbackButton VkEditBtn { get; set; }

        private InlineKeyboardCallbackButton InstagramEditBtn { get; set; }

        private InlineKeyboardCallbackButton ChatEditBtn { get; set; }

        private InlineKeyboardCallbackButton ChannelEditBtn { get; set; }

        public ContactEditMessage BuildMessage()
        {
            base.TextMessage = "Выберите действие";
            VkEditBtn = new InlineKeyboardCallbackButton("VK.COM", BuildCallData("VkEdit"));
            InstagramEditBtn = new InlineKeyboardCallbackButton("Instagram", BuildCallData("InstagramEdit"));
            ChatEditBtn = new InlineKeyboardCallbackButton("Чат в телеграм", BuildCallData("ChatEdit"));
            ChannelEditBtn = new InlineKeyboardCallbackButton("Канал в телеграм", BuildCallData("ChannelEdit"));
            BackBtn = new InlineKeyboardCallbackButton("Назад", BuildCallData("BackToAdminPanel"));
            SetInlineKeyBoard();
            return this;
        }

        private void SetInlineKeyBoard()
        {

            base.MessageReplyMarkup = new InlineKeyboardMarkup(
                new[]{
                new[]
                        {
                            VkEditBtn
                        },
                new[]
                        {
                            InstagramEditBtn
                        },
                new[]
                        {
                            ChatEditBtn
                        },
                new[]
                        {
                            ChannelEditBtn
                        },
                new[]
                        {
                            BackBtn
                        }

                 });
        }
    }
}
