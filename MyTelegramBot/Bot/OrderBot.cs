using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.EntityFrameworkCore;
using MyTelegramBot.Messages.Admin;
using MyTelegramBot.Messages;

namespace MyTelegramBot.Bot
{
    public partial class OrderBot:Bot.BotCore
    {
        /// <summary>
        /// Сообщение с просибой отправить свой номер телефона
        /// </summary>
        private RequestPhoneNumberMessage RequestPhoneNumberMsg { get; set; }

        /// <summary>
        /// Сообщение с предаварительным описанием заказа. Без номер и т.д
        /// </summary>
        private OrderTempMessage OrderPreviewMsg { get; set; }

        /// <summary>
        /// Сообщение с вариантами оплаты
        /// </summary>
        private PaymentsMethodsListMessage PaymentsMethodsListMsg { get; set; }

        /// <summary>
        /// Адреса пользователя
        /// </summary>
        private AddressListMessage ViewShipAddressMsg { get; set; }

        /// <summary>
        /// Сообщение с описанием заказа
        /// </summary>
        private OrderViewMessage OrderViewMsg { get; set; }

       
        private NewFeedBackAddedMessage NewFeedBackAddedMsg { get; set; }

        /// <summary>
        /// Сообщение с позициями заказа. Каждай позиция отдельная кнопка
        /// </summary>

        private MyOrdersMessage MyOrdersMsg { get; set; }

        private Messages.Admin.AdminOrderMessage OrderAdminMsg { get; set; }

        private int OrderId { get; set; }

        private Orders Order { get; set; }

        const string CmdAddDescToOrder = "Добавить комментарий к заказу";

        public const string CmdEnterDesc = "Введите комментарий";

        /// <summary>
        /// Пользователь выбрал адрес доставки
        /// </summary>
        public const string CmdGetAddress = "GetAddress";

        /// <summary>
        /// Изменить адрес доставки заказа
        /// </summary>
        public const string CmdAddressEditor = "AddressEditor";

        /// <summary>
        /// Добавить комментарий к заказу
        /// </summary>
        public const string CmdOrderDesc = "OrderDesc";

        /// <summary>
        /// Отправить заказ на рассмотрение
        /// </summary>
        public const string CmdOrderSave = "OrderSave";


        /// <summary>
        /// Показать адрес доставки на карте
        /// </summary>
        public const string CmdViewAddressOnMap = "ViewAddressOnMap";

        public const string CmdAddFeedBack = "AddFeedBack";

        public const string MyOrdersListCmd = "MyOrdersList";

        public const string MyOrder = "/myorder";

        /// <summary>
        /// Назад
        /// </summary>
        public const string CmdBackToOrder = "BackToOrder";

        private const string ForceReplyAddFeedBack = "Добавить отзыв к заказу:";

        private const string GetOrderCmd = "/order";

        /// <summary>
        /// Выбран один из варинатов оплаты
        /// </summary>
        public const string PaymentMethodCmd = "GetPaymentMethod";

        /// <summary>
        /// Список все доступных варинатов оплаты
        /// </summary>
        public const string GetPaymentMethodListCmd = "GetPaymentMethodList";

        /// <summary>
        ///Кнопка Я оплатил
        /// </summary>
        public const string CheckPayCmd = "CheckPay";

        private const int QiwiPayMethodId = 2;

        private const int PaymentOnReceipt = 1;

        int AddressId { get; set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="_update"></param>
        public OrderBot(Update _update) : base(_update)
        {            
         

        }

        protected override void Constructor()
        {
            if (Update.Message != null && Update.Message.ReplyToMessage != null)
                CommandName = Update.Message.ReplyToMessage.Text;

            try
            {
                PaymentsMethodsListMsg = new PaymentsMethodsListMessage();
                if (base.Argumetns.Count > 0)
                {
                    OrderId = Argumetns[0];
                    OrderViewMsg = new OrderViewMessage(this.OrderId);
                    MyOrdersMsg = new MyOrdersMessage(base.FollowerId,BotInfo.Id);
                    using (MarketBotDbContext db = new MarketBotDbContext())
                        Order = db.Orders.Where(o => o.Id == this.OrderId).Include(o => o.OrderConfirm).
                            Include(o => o.OrderDone).Include(o => o.OrderDeleted).
                            Include(o => o.OrderProduct).Include(o => o.Follower).Include(o => o.FeedBack).FirstOrDefault();

                }

                RequestPhoneNumberMsg = new RequestPhoneNumberMessage(base.FollowerId);
                ViewShipAddressMsg = new AddressListMessage(base.FollowerId);
                OrderPreviewMsg = new OrderTempMessage(base.FollowerId,BotInfo.Id);

            }

            catch
            {

            }

            finally
            {

            }
        }

        public async override Task<IActionResult> Response()
        {
            switch (base.CommandName)
            {
                ///Поользователь выбрал адрес доставки нажав на кнопку,
                ///далее появляется сообщение с выбором метода оплаты, если доступны больше 1ого метода,
                ///если доступен только один метод, то появляется сообщение с заказом
                case CmdGetAddress:
                   return await SendPaymentMethodsList();

                ///Польватель нажал изменить адрес доставки. 
                ///Сообщение с описание заказа редактируется на сообщение со списком его адресов
                case CmdAddressEditor:
                    return await SendAddressEditor();

                ///Пользвовательно нажал на кнопку "Комментарий к заказу"
                case CmdOrderDesc:
                    return await SendForceReplyAddDesc();

                /// Поользователь присал новый комментриай к заказу процитировав сообщение бота "Введите комментарий".
                /// Коммент сохрнаятеся в бд,а После этого бот присылает
                /// обновелнное описание заказа
                case CmdEnterDesc:
                    return await AddOrderTempDesc();

                //Пользователь нажал на кнопку "Отправить заказ"
                case CmdOrderSave:
                    return await OrderSave();


                //Пользователь нажал на один из доступных вариантов оплаты
                case PaymentMethodCmd:
                    return await GetPaymentMethod();

                //Пользователь нажал на кнопку Мои заказы
                case MyOrdersListCmd:
                    return await SendMyOrderList();

                //Пользователь записал свой ник в настройках, и нажал далее на картинке
                case "VerifyUserName":
                    return await UserNameCheck();
                default:
                    break;
                    
            }

            //Пользоватлеь отправил команду /myorder
            if (base.CommandName.Contains(MyOrder))
                return await SendMyOrder();

            //Пользователь нажал на кнопку "Я оплатил"
            if (base.CommandName == CheckPayCmd)
                return await CheckPay();

            //Пользователь нажал добавить отзыв, ему пришло форсе реплай сообещнеи "Оставить отзыв к заказу:N". Если отзыва еще нет
            if (base.CommandName == CmdAddFeedBack && Order != null && Order.FeedBack == null)
                return await ForceReplyBuilder(ForceReplyAddFeedBack + Order.Number);

            ///Добавить отзыв
            if (base.OriginalMessage.Contains(ForceReplyAddFeedBack))
                return await SaveNewFeedBack();


            else
                return null;
           
            
        }

       
    }

}
