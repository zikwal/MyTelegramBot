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
using MyTelegramBot.Bot.AdminModule;

namespace MyTelegramBot.Messages
{
    /// <summary>
    /// Сообщение с категориями товаров в виде кнопок
    /// </summary>
    public class CategoryListMessage:Bot.BotMessage
    {
        private string Cmd { get; set; }

        private string BackCmd { get; set; }

        /// <summary>
        /// id товара которому нужно поменять категорию
        /// </summary>
        private int EditProductId { get; set; }

        private InlineKeyboardCallbackButton [][] CategoryListBtn { get; set; }

        private List<Category> Categorys { get; set; }

        private InlineKeyboardCallbackButton ViewAllBtn { get; set; }

        /// <summary>
        /// Для меню
        /// </summary>
        public CategoryListMessage()
        {
            Cmd = "ProductInCategory";
            BackCmd = "MainMenu";
            BackBtn = new InlineKeyboardCallbackButton("Назад", BuildCallData(BackCmd));
            
        }

        /// <summary>
        /// Другое действие. Например для панели администратора
        /// </summary>
        /// <param name="Cmd"></param>
        public CategoryListMessage(string Cmd)
        {
            this.Cmd = Cmd;
            this.BackCmd =AdminBot.BackToAdminPanelCmd;
            BackBtn= new InlineKeyboardCallbackButton("Назад", BuildCallData(BackCmd));
        }

        /// <summary>
        /// редактируем категорию в которой находится товар
        /// </summary>
        /// <param name="EditProductId"></param>
        /// <param name="Cmd"></param>
        public CategoryListMessage (int EditProductId, string Cmd = "UpdateCategory")
        {
            //
            this.EditProductId = EditProductId;
            this.Cmd = Cmd;
            this.BackCmd = "SelectProduct";
            BackBtn = new InlineKeyboardCallbackButton("Назад", BuildCallData(BackCmd,this.EditProductId));
        }

        public CategoryListMessage BuildMessage()
        {           
            using (MarketBotDbContext db=new MarketBotDbContext())
                Categorys=db.Category.ToList();
            
            CategoryListBtn = new InlineKeyboardCallbackButton[Categorys.Count+2][];

            ViewAllBtn = new InlineKeyboardCallbackButton("Показать весь ассортимент", BuildCallData("ViewAllProduct"));

            int count = 0;
            if (Categorys.Count > 0)
            {
                foreach (Category cat in Categorys)
                {
                    if (EditProductId > 0) // Если меняем категорию в которой находится товар. Для админа
                    {
                        InlineKeyboardCallbackButton button = new InlineKeyboardCallbackButton(cat.Name, base.BuildCallData(Cmd, EditProductId, cat.Id));
                        CategoryListBtn[count] = new InlineKeyboardCallbackButton[1];
                        CategoryListBtn[count][0] = button;
                    }

                    else // Для меню
                    {
                        InlineKeyboardCallbackButton button = new InlineKeyboardCallbackButton(cat.Name, base.BuildCallData(Cmd, cat.Id));
                        CategoryListBtn[count] = new InlineKeyboardCallbackButton[1];
                        CategoryListBtn[count][0] = button;
                    }
                    count++;

                }

                base.TextMessage = "Выберите категорию";
                CategoryListBtn[Categorys.Count+1] = new InlineKeyboardCallbackButton[1];
                CategoryListBtn[Categorys.Count+1][0] = BackBtn;


                CategoryListBtn[Categorys.Count] = new InlineKeyboardCallbackButton[1];
                CategoryListBtn[Categorys.Count][0] = ViewAllBtn;

                base.MessageReplyMarkup = new InlineKeyboardMarkup(CategoryListBtn);
            }

            else
                base.TextMessage = "Данные отсутствуют";
            
            return this;
        }


    }
}
