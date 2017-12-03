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

    public class AdminPanelCmdMessage:Bot.BotMessage
    {
        private InlineKeyboardCallbackButton EditProductBtn { get; set; }

        private InlineKeyboardCallbackButton EditCategoryBtn { get; set; }

        private InlineKeyboardCallbackButton NoConfirmOrdersBtn { get; set; }

        private InlineKeyboardCallbackButton ContactEditPanelBtn { get; set; }

        private InlineKeyboardCallbackButton PaymentsEnableListBtn { get; set; }

        private MyTelegramBot.Admin Admin { get; set; }

        private int FollowerId { get; set; }

        private int AdminId { get; set; }
        public AdminPanelCmdMessage(int FollowerId)
        {
            this.FollowerId = FollowerId;
        }

        public AdminPanelCmdMessage BuildMessage()
        {


                EditProductBtn = new InlineKeyboardCallbackButton("Изменить товар"+ " \ud83d\udd8a", BuildCallData(AdminBot.ProductEditCmd));
                EditCategoryBtn = new InlineKeyboardCallbackButton("Изменить категорию"+ " \ud83d\udd8a", BuildCallData(AdminBot.CategoryEditCmd));
                ContactEditPanelBtn= new InlineKeyboardCallbackButton("Изменить контактные данные"+ " \ud83d\udd8a", BuildCallData(AdminBot.ContactEditCmd));
                NoConfirmOrdersBtn = new InlineKeyboardCallbackButton("Показать необработанные заказы" + " \ud83d\udcd2", BuildCallData(AdminBot.NoConfirmOrderCmd));
                PaymentsEnableListBtn = new InlineKeyboardCallbackButton("Выбрать доступные методы оплаты" + " \ud83d\udcb0", BuildCallData(AdminBot.PayMethodsListCmd));

            base.TextMessage = Bold("Панель администратора") + NewLine() +
                               "1) Показать текущие остатки /currentstock" + NewLine() +
                               "2) Показать все товары одним сообщением /allprod" + NewLine() +
                               "3) Импорт новых товаров из CSV файла /import" + NewLine() +
                               "4) Экспорт всех заказов в CSV файл /export" + NewLine() +
                               "5) Экспорт истории изменения остатков /stockexport" + NewLine() +
                               "6) Добавить новый товар /newprod" + NewLine() +
                               "7) Создать новую категорию /newcategory" + NewLine() +
                               "8) Настроить QIWI платежи /qiwi" + NewLine() +
                               "9) Выбрать доступные способы оплаты /paymethods" + NewLine() +
                               "10) Статистика /stat" + NewLine() +
                               "11) Список операторов / Добавить нового / Удалить /operators" + NewLine() +
                               "12) Бот рассылает уведомления в ЛС. Что бы выключить нажмите /off , что бы включить нажмите /on";

                SetInlineKeyBoard();
                return this;

        }

        private void SetInlineKeyBoard()
        {
            
                base.MessageReplyMarkup = new InlineKeyboardMarkup(
                    new[]{
                new[]
                        {
                            EditProductBtn
                        },
                new[]
                        {
                            EditCategoryBtn,
                        },

                new[]
                        {
                            NoConfirmOrdersBtn
                        },
                new[]
                        {
                            ContactEditPanelBtn
                        },

                     });
        }
    }


}
