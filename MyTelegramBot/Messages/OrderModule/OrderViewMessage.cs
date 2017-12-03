using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types.InlineKeyboardButtons;
using System.Security.Cryptography;
using System.Text;

namespace MyTelegramBot.Messages
{
    /// <summary>
    /// Сообщение с описание заказа
    /// </summary>
    public class OrderViewMessage: Bot.BotMessage
    {
        private int OrderId { get; set; }

        private Orders Order { get; set; }

        private InlineKeyboardCallbackButton ChekPayBtn { get; set; }

        private InlineKeyboardCallbackButton AddFeedbackBtn { get; set; }

        private InlineKeyboardCallbackButton RemoveOrderBtn { get; set; }


        private const int QiwiPayMethodId = 2;

        private const int PaymentOnReceiptId = 1;

        public OrderViewMessage (int OrderId)
        {
            this.OrderId = OrderId;
        }

        public OrderViewMessage (Orders order)
        {
            this.Order = order;
        }

        public OrderViewMessage BuildMessage()
        {
            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                if (this.OrderId > 0) // если в конструктор был передан айди заявки
                    Order = db.Orders.Where(o => o.Id == OrderId).
                        Include(o => o.OrderConfirm).
                        Include(o => o.OrderDeleted).
                        Include(o => o.OrderDone).
                        Include(o => o.FeedBack).
                        Include(o => o.OrderProduct).
                        Include(o => o.OrderPayment).
                        Include(o => o.OrderAddress).Include(o=>o.BotInfo)
                        .Include(o=>o.OrderProduct).FirstOrDefault();

                if(Order!=null && Order.OrderProduct.Count==0)
                    Order.OrderProduct = db.OrderProduct.Where(op => op.OrderId == Order.Id).ToList();

                if (Order != null)
                {
                    string Position = "";

                    string done = "";

                    string feedback = "-";

                    string paid = "";

                    string PaymentFaq = "";

                    double total = 0.0; // общая строисоить заказа

                    var Address = db.Address.Where(a => a.Id == Order.OrderAddress.FirstOrDefault().AdressId).
                        Include(a => a.House).
                        Include(a => a.House.Street).
                        Include(a => a.House.Street.City).
                        FirstOrDefault();


                    try
                    {
                        int counter = 0; // счетчки цикла
                    

                        foreach (OrderProduct p in Order.OrderProduct)
                        {
                            counter++;
                            p.Product = db.Product.Where(x => x.Id == p.ProductId).Include(x => x.ProductPrice).FirstOrDefault();
                            p.Price = db.ProductPrice.Where(price => price.Id == p.PriceId).FirstOrDefault();
                            Position += counter.ToString() + ") " + p.ToString() + NewLine();
                            total += p.Price.Value * p.Count;
                        }

                        if (Order.BotInfo == null)
                            Order.BotInfo = db.BotInfo.Where(o => o.Id == Order.BotInfoId).FirstOrDefault();

                        if (Order.OrderDone != null && Order.OrderDone.Count > 0) //Заказ выполнен
                            done = "Да";

                        else // ЗАказ не выполен
                            done = "Нет";

                        if (Order.OrderDone != null && Order.OrderDone.Count > 0 && Order.FeedBack != null && Order.FeedBack.Text != null) // Есть отзыв к заказ
                            feedback = Order.FeedBack.Text + " | " + Order.FeedBack.DateAdd.ToString();

                        if (Order.Paid == false && Order.OrderDone.Count == 0 && Order.PaymentTypeId== QiwiPayMethodId) 
                            // заказ еще не оплачен. Метод оплаты Киви
                        {
                            paid = "Нет";
                            PaymentFaq = QiwiNoPaid(db.QiwiApi.Where(q=>q.Enable==true).OrderByDescending(q=>q.Id).FirstOrDefault().Telephone, total);
                        }

                        if (Order.Paid == false && Order.PaymentTypeId == PaymentOnReceiptId) // Заказ не выполнен и способ оплаты "При получении"
                            paid = "Нет";


                        if (Order.Paid == true) // Заказ оплачен
                            paid = "Да";

                        if (Order.OrderDone != null && Order.OrderDone.Count > 0 && Order.FeedBack == null) // Отзыва нет, Добавляем кнопку
                        {
                            feedback = "Нет";
                             NonFeedBack();
                        }

                        base.TextMessage = Bold("Номер заказа: ") + Order.Number.ToString() + NewLine()
                                    + Position + NewLine()
                                    + Bold("Общая стоимость: ") + total.ToString() + " руб." + NewLine()
                                    + Bold("Комментарий: ") + Order.Text + NewLine()
                                    + Bold("Адрес доставки: ") + Address.House.Street.City.Name + ", " + Address.House.Street.Name + ", " + Address.House.Number + NewLine()
                                    + Bold("Время: ") + Order.DateAdd.ToString() + NewLine()
                                    + Bold("Оплачено: ") + paid
                                    + NewLine() + Bold("Выполнено: ") + done
                                    + NewLine()+Bold("Оформлен через:") +"@"+ Order.BotInfo.Name
                                    + NewLine() + Bold("Отзыв: ") + feedback
                                    + NewLine() + PaymentFaq;
                    }
                    catch (Exception exp)
                    {

                    }
                    base.CallBackTitleText = "Номер заказа:" + Order.Number.ToString();
                }


            }

            return this;
        }

        private void NonFeedBack()
        {
            AddFeedbackBtn = new InlineKeyboardCallbackButton("Добавить отзыв", BuildCallData(Bot.OrderBot.CmdAddFeedBack, Order.Id));

            base.MessageReplyMarkup = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(
                new[]{
                                new[]
                                    {
                                            AddFeedbackBtn
                                    }, });

        }

        private string QiwiNoPaid(string telephone, double total)
        {
            base.MessageReplyMarkup = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(
                new[]{
                                new[]
                                    {
                                            ChekPayBtn=ChekPay(Order.Id)
                                    },

                });

            return NewLine() + Bold("Реквизиты:") +
            NewLine() + Bold("Телефон: ") + telephone +
            NewLine() + Bold("Сумма: ") + total +
            NewLine() + Bold("Комментарий: ") + Bot.GeneralFunction.BuildPaymentComment(Bot.GeneralFunction.GetBotName(), Order.Number.ToString()) +
            NewLine() + NewLine()+
            QiwiPaymentUrl(Convert.ToInt32(total), Order.Number, telephone);
        }

        private string QiwiPaymentUrl(int total, decimal? OrderNumber, string Telephone)
        {
            //https://qiwi.com/payment/form/<ID>?<parameter>=<value>
            //99 - Перевод на Visa QIWI Wallet
            //amountInteger - целая часть суммы платежа(рубли).
            //amountFraction - дробная часть суммы платежа(копейки).
            //currency - константа, 643.Обязательный параметр, если вы передаете в ссылке сумму платежа
            //extra['comment'] -комментарий(только для ID = 99).Имя параметра должно быть URL-закодировано.
            //extra['account'] - номер телефона / счета / карты пользователя.Формат совпадает с форматом параметра fields.account в соответствующем платежном запросе.Имя параметра должно быть URL - закодировано.

            string amountInteger = "amountInteger=" + total.ToString();
            string currency = "currency=643";
            string comment = "extra['comment']=" + Bot.GeneralFunction.BuildPaymentComment(Bot.GeneralFunction.GetBotName(),OrderNumber.ToString());
            string account = "extra['account']=" + Telephone;

            string url= "https://qiwi.com/payment/form/99?"+account+"&"+amountInteger+"&"+comment+"&"+currency;
            return HrefUrl(url, "Нажмите сюда, что бы открыть заполенную платежную форму");
        }

        private InlineKeyboardCallbackButton ChekPay(int OrderId)
        {
            InlineKeyboardCallbackButton button = new InlineKeyboardCallbackButton("Я оплатил", BuildCallData(Bot.OrderBot.CheckPayCmd, OrderId));
            return button;
        }

    }
}
