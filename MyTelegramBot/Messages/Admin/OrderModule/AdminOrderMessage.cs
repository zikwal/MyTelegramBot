using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.EntityFrameworkCore;
using MyTelegramBot.Messages.Admin;
using MyTelegramBot.Messages;
using MyTelegramBot.Bot.AdminModule;

namespace MyTelegramBot.Messages.Admin
{
    /// <summary>
    /// Сообщение с описанием заказа. Админка
    /// </summary>
    public class AdminOrderMessage:Bot.BotMessage
    {
        private InlineKeyboardCallbackButton EditOrderPositionBtn { get; set; }

        private InlineKeyboardCallbackButton ViewTelephoneNumberBtn { get; set; }

        /// <summary>
        /// Кнопка взять заказ в обработку
        /// </summary>
        private InlineKeyboardCallbackButton TakeOrderBtn { get; set; }

        private InlineKeyboardCallbackButton ViewAddressOnMapBtn { get; set; }

        private InlineKeyboardCallbackButton DoneBtn { get; set; }

        private InlineKeyboardCallbackButton DeleteBtn { get; set; }

        private InlineKeyboardCallbackButton RecoveryBtn { get; set; }

        private InlineKeyboardCallbackButton ConfirmBtn { get; set; }

        private InlineKeyboardCallbackButton ViewPaymentBtn { get; set; }

        /// <summary>
        /// Освободить заявку
        /// </summary>
        private InlineKeyboardCallbackButton FreeOrderBtn { get; set; }
      
        private List<Payment> Payments { get; set; }

        private Orders Order { get; set; }

        private int OrderId { get; set; }

        private Follower Follower { get; set; }

        private string PaymentMethodName { get; set; }

        /// <summary>
        /// Какому пользователю будет отсылать сообщение
        /// </summary>
        private int FollowerId { get; set; }

        /// <summary>
        /// У какого пользователя заявка сейчас находится в обработке
        /// </summary>
        private int InWorkFollowerId { get; set; }

        public AdminOrderMessage(int OrderId, int FollowerId = 0)
        {
            this.OrderId = OrderId;
            this.FollowerId = FollowerId;
        }

        public AdminOrderMessage (Orders order, int FollowerId=0)
        {
            this.Order = order;
            this.FollowerId = FollowerId;
        }

        public AdminOrderMessage BuildMessage()
        {
            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                if(this.Order==null && this.OrderId>0)
                Order = db.Orders.Where(o => o.Id == OrderId).
                    Include(o => o.OrderConfirm).
                    Include(o => o.OrderDeleted).
                    Include(o => o.OrderDone).
                    Include(o=>o.FeedBack).
                    Include(o => o.OrderProduct).
                    Include(o => o.OrderAddress).
                    Include(o=>o.OrdersInWork).
                    Include(o=>o.PaymentType).
                    Include(o=>o.OrderPayment).FirstOrDefault();
               
                var Address = db.Address.Where(a => a.Id == Order.OrderAddress.FirstOrDefault().AdressId).Include(a => a.House).Include(a => a.House.Street).Include(a => a.House.Street.City).FirstOrDefault();

                double total = 0.0;
                string Position = "";
                int counter = 0;
                string Paid = "";

                ///////////Провереряем какой метод оплаты и наличие платежей////////////
                if (Order.PaymentType!= null)
                {
                    PaymentMethodName = Order.PaymentType.Name;
                }

                else
                {
                    PaymentMethodName = db.PaymentType.Where(p=>p.Id==Order.PaymentTypeId).FirstOrDefault().Name;
                }

                if (Order.BotInfo == null)
                    Order.BotInfo = db.BotInfo.Where(b => b.Id == Order.BotInfoId).FirstOrDefault();

                if (Order.Paid == true)
                    Paid = "Оплачено";

                else
                    Paid = "Не оплачено";
              

                foreach (OrderProduct p in Order.OrderProduct) // Вытаскиваем все товары из заказа
                {
                    counter++;
                    p.Product = db.Product.Where(x => x.Id == p.ProductId).Include(x => x.ProductPrice).FirstOrDefault();
                    if(p.Price==null)
                        p.Price = p.Product.ProductPrice.FirstOrDefault();

                    Position += counter.ToString() + ") " + p.AdminText() + NewLine();
                    total += p.Price.Value * p.Count;
                }

                /////////Формируем основную часть сообщения
                base.TextMessage = Bold("Номер заказа: ") + Order.Number.ToString() + NewLine()
                            + Position + NewLine()
                            + Bold("Общая стоимость: ") + total.ToString() + NewLine()
                            + Bold("Комментарий: ") + Order.Text + NewLine()
                            + Bold("Адрес доставки: ") + Address.House.Street.City.Name + ", " + Address.House.Street.Name + ", " + Address.House.Number + NewLine()
                            + Bold("Время: ") + Order.DateAdd.ToString() +NewLine()
                            + Bold("Способ оплаты: ") + PaymentMethodName + NewLine() 
                            +Bold("Оформлено через: ")+"@" + Order.BotInfo.Name +NewLine()
                            + Bold("Статус платежа: ") + Paid;

                //Детали согласования заказа
                if (Order != null && Order.OrderConfirm != null && Order.OrderConfirm.Count > 0
                    && Order.OrderDeleted.Count == 0)
                    base.TextMessage += NewLine() + NewLine() + Bold("Заказ согласован:") + NewLine() + Italic("Комментарий: " + Order.OrderConfirm.OrderByDescending(o => o.Id).FirstOrDefault().Text
                        + " |Время: " + Order.OrderConfirm.OrderByDescending(o => o.Id).FirstOrDefault().DateAdd.ToString() 
                        + " |Пользователь: " + Bot.GeneralFunction.FollowerFullName(Order.OrderConfirm.OrderByDescending(o => o.Id).FirstOrDefault().FollowerId));

                ///Детали удаления заказа
                if (Order != null && Order.OrderDeleted != null && Order.OrderDeleted.Count > 0)
                    base.TextMessage += NewLine() + NewLine() + Bold("Заказ удален:") + NewLine() + Italic("Комментарий: " + Order.OrderDeleted.OrderByDescending(o => o.Id).FirstOrDefault().Text
                        + " |Время: " + Order.OrderDeleted.OrderByDescending(o => o.Id).FirstOrDefault().DateAdd.ToString()
                        + " |Пользователь: " + Bot.GeneralFunction.FollowerFullName(Order.OrderDeleted.OrderByDescending(o => o.Id).FirstOrDefault().FollowerId));

                ///Детали выполнения заказа
                if (Order != null && Order.OrderDone != null && Order.OrderDone.Count > 0)
                    base.TextMessage += NewLine() + NewLine() + Bold("Заказ выполнен:") + Italic(Order.OrderDone.OrderByDescending(o => o.Id).FirstOrDefault().DateAdd.ToString())
                        + " |Пользователь: " + Bot.GeneralFunction.FollowerFullName(Order.OrderDone.OrderByDescending(o => o.Id).FirstOrDefault().FollowerId);

                //Детали Отзыва к заказу
                if (Order != null && Order.FeedBack != null && Order.FeedBack.Text != null && Order.FeedBack.Text != "")
                    base.TextMessage += NewLine() + NewLine() + Bold("Отзыв к заказу:") + NewLine() + Italic(Order.FeedBack.Text + " | Время: " + Order.FeedBack.DateAdd.ToString());

                InWorkFollowerId = WhoInWork(Order);

                CreateBtns();

                SetInlineKeyBoard();

                return this; }
        }

        private int WhoInWork(Orders order)
        {
            if (order != null && order.OrdersInWork.Count > 0)
            {
                var in_work = order.OrdersInWork.OrderByDescending(o => o.Id).FirstOrDefault();

                if (in_work != null && in_work.InWork == true)
                    return Convert.ToInt32(in_work.FollowerId);

                else
                    return 0;
            }

            else
                return 0;
        }

        private void CreateBtns()
        {
            EditOrderPositionBtn = new InlineKeyboardCallbackButton("Изменить содержание заказа"+ " \ud83d\udd8a", BuildCallData(OrderProccesingBot.CmdEditOrderPosition, Order.Id));

            ViewTelephoneNumberBtn = new InlineKeyboardCallbackButton("Контактные данные"+ " \ud83d\udcde", BuildCallData(OrderProccesingBot.CmdGetTelephone, Order.Id));

            ViewAddressOnMapBtn = new InlineKeyboardCallbackButton("Показать на карте"+ " \ud83c\udfd8", BuildCallData(OrderProccesingBot.CmdViewAddressOnMap, Order.Id));

            DoneBtn = new InlineKeyboardCallbackButton("Выполнено"+" \ud83d\udc4c\ud83c\udffb", BuildCallData(OrderProccesingBot.CmdDoneOrder, Order.Id));

            DeleteBtn = new InlineKeyboardCallbackButton("Удалить"+ " \u2702\ufe0f", BuildCallData(OrderProccesingBot.CmdOrderDelete, Order.Id));

            RecoveryBtn = new InlineKeyboardCallbackButton("Восстановить", BuildCallData(OrderProccesingBot.CmdRecoveryOrder, Order.Id));

            ConfirmBtn = new InlineKeyboardCallbackButton("Согласован"+ " \ud83e\udd1d", BuildCallData(OrderProccesingBot.CmdConfirmOrder, Order.Id));

            ViewPaymentBtn = new InlineKeyboardCallbackButton("Посмотреть платеж" + " \ud83d\udcb5", BuildCallData("ViewPayment", Order.Id));

            TakeOrderBtn = new InlineKeyboardCallbackButton("Взять в работу", BuildCallData("TakeOrder", Order.Id));

            FreeOrderBtn = new InlineKeyboardCallbackButton("Освободить", BuildCallData("FreeOrder", Order.Id));
        }

        private void SetInlineKeyBoard()
        {
            //Заявка еще ни кем не взята в обрабоку или Неизвстно кому мы отрпавляем это сообщение т.е переменная FollowerId=0
            if (InWorkFollowerId == 0  || FollowerId==0 )
                base.MessageReplyMarkup = new InlineKeyboardMarkup(
                new[]{
                    new[]
                    {
                        TakeOrderBtn
                    }
                });

            ///Заявка взять в обработку пользователем. Рисуем основные кнопки
            if (Order.OrderDeleted.Count == 0 && Order.OrderConfirm.Count == 0 && FollowerId==InWorkFollowerId && InWorkFollowerId!=0)
                base.MessageReplyMarkup = new InlineKeyboardMarkup(
                new[]{
                new[]
                        {
                            ViewPaymentBtn, FreeOrderBtn
                        },
                new[]
                        {
                            EditOrderPositionBtn
                        },
                new[]
                        {
                            ConfirmBtn,DeleteBtn
                        },
                new[]
                        {
                            ViewTelephoneNumberBtn,
                            ViewAddressOnMapBtn
                        },

                 });

            ///Заявка взять в обработку пользователем. Но заказ удален.
            if (Order.OrderDeleted.Count > 0 && FollowerId == InWorkFollowerId && InWorkFollowerId != 0)
                base.MessageReplyMarkup = new InlineKeyboardMarkup(
                new[]{
                new[]
                        {
                            ViewPaymentBtn,FreeOrderBtn
                        },
                new[]
                        {
                            EditOrderPositionBtn
                        },
                new[]
                        {
                            RecoveryBtn
                        },
                new[]
                        {
                            ViewTelephoneNumberBtn,
                            ViewAddressOnMapBtn
                        },

                });
            ///Заявка взять в обработку пользователем. Зазакз уже согласован
            if (Order.OrderConfirm.Count > 0 && Order.OrderDeleted.Count == 0 && FollowerId == InWorkFollowerId && InWorkFollowerId != 0)
                    base.MessageReplyMarkup = new InlineKeyboardMarkup(
                    new[]{
                 new[]
                        {
                            ViewPaymentBtn,FreeOrderBtn
                        },
                new[]
                        {
                            EditOrderPositionBtn
                        },
                new[]
                        {
                            DoneBtn,DeleteBtn
                        },
                new[]
                        {
                            ViewTelephoneNumberBtn,
                            ViewAddressOnMapBtn
                        },

                    });

            ///Заявка взять в обработку пользователем или может быть просто открыта любым т.к она уже выполнена
            if (Order.OrderDone.Count > 0)
                base.MessageReplyMarkup = new InlineKeyboardMarkup(
                new[]{
                new[]
                        {
                            ViewPaymentBtn
                        },
                new[]
                        {
                            ViewTelephoneNumberBtn,
                            ViewAddressOnMapBtn
                        },

                });
        }

    }
}
