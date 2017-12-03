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
    public partial class OrderBot
    {
        /// <summary>
        /// Показать заказ пользователя. Команда /myorder
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> SendMyOrder()
        {
            try
            {
                int number = Convert.ToInt32(base.CommandName.Substring(MyOrder.Length));
                using (MarketBotDbContext db = new MarketBotDbContext())
                {
                    Orders order = new Orders();

                    // Пользователь будет видеть заказы оформелнные через других ботов
                    order = db.Orders.Where(o => o.Number == number && o.FollowerId == FollowerId).Include(o => o.OrderConfirm)
                       .Include(o => o.OrderDeleted).Include(o => o.OrderDone).Include(o => o.FeedBack).
                        Include(o => o.OrderProduct).Include(o => o.OrderAddress).Include(o=>o.BotInfo).FirstOrDefault();


                    if (order != null)
                    {
                        OrderViewMsg = new OrderViewMessage(order);
                        await SendMessage(OrderViewMsg.BuildMessage());
                        return base.OkResult;
                    }

                    else
                        return base.OkResult;
                }
            }

            catch
            {
                return base.NotFoundResult;
            }
        }

        /// <summary>
        /// Отправить пользователю список его заказов
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> SendMyOrderList()
        {
            try
            {
                if (MyOrdersMsg != null)
                {
                    await SendMessage(MyOrdersMsg.BuildMessage());
                    return OkResult;
                }

                else
                {
                    MyOrdersMsg = new MyOrdersMessage(base.FollowerId,BotInfo.Id);
                    await SendMessage(MyOrdersMsg.BuildMessage());
                    return OkResult;
                }
            }

            catch
            {
                return NotFoundResult;
            }
        }

        /// <summary>
        /// Пользователь выбрал методо оплаты
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> GetPaymentMethod()
        {
            int id= 0;
            if (Argumetns.Count > 0)
                id = Argumetns[0];

            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                var method = db.PaymentType.Where(p => p.Enable == true).FirstOrDefault();
                base.ConfigurationBot = GetConfigurationBot(BotInfo.Id);
                //если включена верификация номера телефона, то проверяем есть ли БД номер телефона текущего пользователя
                if (method != null && ConfigurationBot.VerifyTelephone)
                    return await TelephoneCheck(id);

                else // Если верификации номера телефона нет, то проверяем указан ли у пользователя UserName
                    return await UserNameCheck(base.ConfigurationBot, id);

            }

        }

        /// <summary>
        /// После того, как пользователь выбрал адрес доставки, Отправляем сообщение с выбором варианта оплаты
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> SendPaymentMethodsList()
        {
            int AddressId = 0;

            if (Argumetns.Count > 0)
            { // сохраняем адрес
                AddressId = Argumetns[0];

                AddAddressToOrderTemp(AddressId);
            }

            var message = PaymentsMethodsListMsg.BuildMessage();

            if (message != null && await EditMessage(message)!=null)            
                return OkResult;
            

            else // если сообщение с вариантами оплаты пустое, значит досутпен только один метод. Его выбираем и записываем в БД.
            // и отправляем сообщение с описание заказа, но перед эти проверяем номер телефон пользователя или его UserName в завизимости от настроек бота
            {
                using (MarketBotDbContext db = new MarketBotDbContext())
                {
                    var method = db.PaymentType.Where(p => p.Enable == true).FirstOrDefault();
                    base.ConfigurationBot = GetConfigurationBot(BotInfo.Id);
                    //если включена верификация номера телефона, то проверяем есть ли БД номер телефона текущего пользователя
                    if (method != null && ConfigurationBot.VerifyTelephone)
                        await TelephoneCheck(method.Id);

                    else // Если верификации номера телефона нет, то проверяем указан ли у пользователя UserName
                        await UserNameCheck(base.ConfigurationBot, method.Id);
                    
                }

                return OkResult;
            }

           
        }


        /// <summary>
        /// ПОльзователь выбрал вариант оплты из списка. Далее ему отправялется  сообщение с просьбой указать свой номер телефона,
        /// если номер уже есть в базе, то отсылается сообщение с превью заказа.
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> TelephoneCheck(int PaymentType=0)
        {
            int PaymentTypeId = PaymentType;

            if (Argumetns.Count > 0 && PaymentType==0)
                PaymentTypeId = Argumetns[0];

            if (PaymentTypeId > 0)
                AddPaymentMethodToOrderTemp(PaymentTypeId);

            //Сообщение с просьбой отрпвить свой номер телефон. Если сообщение пустое, значит номер телефона уже есть в БД
            var message = RequestPhoneNumberMsg.BuildMessage(); // 


            if (message != null) // Номера телефона нет в базе
            {
                if (await SendMessage(message) != null)
                    return base.OkResult;

                else
                    return base.NotFoundResult;
            }

            else // Номер телефона есть в базе. ПОказываем превью заказа
                return await SendOrderTemp();

        }

        /// <summary>
        /// Проверка юзер нейм пользователя в телеграме. 
        /// Эта функция работает если выключена верификация телефона. Это нужно для того что бы с пользователем можно было выйти на связь
        /// </summary>
        /// <param name="configuration">Конфигурация бота. Может быть пустым в случае когда пользователь нажал далее</param>
        /// <param name="PaymentType">Тип оплаты</param>
        /// <returns></returns>
        private async Task<IActionResult> UserNameCheck(Configuration configuration=null, int PaymentType = 0)
        {

            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                string name;

                int PaymentTypeId = PaymentType;
                
                if (Argumetns.Count > 0 && PaymentType == 0)
                    PaymentTypeId = Argumetns[0];

                if (PaymentTypeId > 0)
                    AddPaymentMethodToOrderTemp(PaymentTypeId);

                if (configuration==null)
                    configuration = db.Configuration.Where(c => c.BotInfoId == BotInfo.Id).FirstOrDefault();

                // Проверяем указан ли у пользователя UserName
                if (Update.CallbackQuery != null && Update.CallbackQuery.Message != null
                    && Update.CallbackQuery.Message.Chat != null && Update.CallbackQuery.Message.Chat.Username != null)
                {
                    name = Update.CallbackQuery.Message.Chat.Username;
                    var follower = db.Follower.Where(f => f.Id == FollowerId).FirstOrDefault();
                    follower.UserName = name;

                    if(name!=follower.UserName)
                        db.SaveChanges();

                    await SendOrderTemp();
                }

                else // Если не указан Просим указать
                {
                    UserNameImageMessage userNameImage = new UserNameImageMessage(configuration);
                    var message = userNameImage.BuilMessage();
                    var PhotoSend= await SendPhoto(message);
                    // добавляем ID файла в бд, что бы потом не отправлять сам файл,а только ID на сервере телегарм
                    if (configuration != null && configuration.UserNameFaqFileId == null && PhotoSend != null)
                    {
                        configuration.UserNameFaqFileId = PhotoSend.Photo[PhotoSend.Photo.Length - 1].FileId;
                        db.SaveChanges();
                    }
                }

                
            }

            return OkResult;
        }


        /// <summary>
        /// Сообщение с деталями Заказа из таблицы OrderTemp
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> SendOrderTemp()
        {
            if (OrderPreviewMsg == null)
                OrderPreviewMsg = new OrderTempMessage(base.FollowerId,BotInfo.Id);

            var message = OrderPreviewMsg.BuildMessage();

            if (message != null && await EditMessage(message) != null)
                return base.OkResult;

            else
                return base.NotFoundResult;
        }

        /// <summary>
        /// Forcre Reply сообщение с просьбой отправить сообщение с комментарием для заказа
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> SendForceReplyAddDesc()
        {
            ForceReply forceReply = new ForceReply
            {
                Force = true,

                Selective = true
            };

            if (await SendMessage(new BotMessage { TextMessage = CmdEnterDesc, MessageReplyMarkup = forceReply }) != null)
                return base.OkResult;

            else
                return base.NotFoundResult;
        }

        /// <summary>
        /// Добавить комментарий к заказу. БД
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> AddOrderTempDesc()
        {
            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                var OrderTmp = db.OrderTemp.Where(o => o.FollowerId == FollowerId).FirstOrDefault();
                if (OrderTmp != null)
                {
                    OrderTmp.Text = Update.Message.Text;

                    if (db.SaveChanges() > 0 && await SendMessage(OrderPreviewMsg.BuildMessage()) != null)
                        return base.OkResult;

                    else
                        return base.NotFoundResult;
                }

                else
                    return base.OkResult;
            }

        }

        /// <summary>
        /// Показать номер телефона покупателя
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> GetContact()
        {
            using (MarketBotDbContext db = new MarketBotDbContext())
            {

                var order = db.Orders.Where(o => o.Id == OrderId).Include(o => o.Follower).FirstOrDefault();

                if (order.Follower != null && order.Follower.Telephone != null && order.Follower.Telephone != "")
                {
                    Contact contact = new Contact
                    {
                        FirstName = order.Follower.FirstName,
                        PhoneNumber = order.Follower.Telephone

                    };

                    await SendContact(contact);
                   
                }

                if(order.Follower != null && order.Follower.UserName != null && order.Follower.UserName != "")
                {
                   string url= Bot.BotMessage.HrefUrl("https://t.me/" + order.Follower.UserName, order.Follower.UserName);
                   await SendMessage(new BotMessage { TextMessage = url });
                   return OkResult;
                }

                else
                    return base.OkResult;
            }
        }



        /// <summary>
        /// Изменить адрес доставки для заказка
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> SendAddressEditor()
        {

            if (await EditMessage(ViewShipAddressMsg.BuildMessage()) != null)
                return base.OkResult;

            else
                return base.NotFoundResult;
        }

        /// <summary>
        /// Сохрнанить
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> OrderSave()
        {
            var new_order = AddOrder();

            OrderViewMsg = new OrderViewMessage(new_order);

            // Если тип платежа "при получении", то отправляем уведомление о новом заказке Админам
            if (new_order!=null && OrderViewMsg !=null && await EditMessage(OrderViewMsg.BuildMessage()) != null && new_order.PaymentTypeId==PaymentOnReceipt)
               return await OrderRedirectToAdmins(new_order.Id);

            

            else
                return base.NotFoundResult;
        }

        /// <summary>
        /// После того как пользователь нажал на кнопку "Отправить", информация о заказе пересылается Операторам в личку и в общий чат (если он есть)
        /// </summary>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        private async Task<IActionResult> OrderRedirectToAdmins(int OrderId, Orders order=null)
        {
            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                try
                {
                    var admins = db.Admin.Include(a => a.Follower).ToList();

                    if(order==null)
                        OrderAdminMsg = new AdminOrderMessage(OrderId);

                    else
                        OrderAdminMsg = new AdminOrderMessage(order);

                    var message = OrderAdminMsg.BuildMessage();

                    await SendMessageAllBotEmployeess(message);

                    return base.OkResult;
                }

                catch
                {
                    return base.NotFoundResult;
                }
            }

        }


        /// <summary>
        /// Добавить информацию о заказе в таблицу Order
        /// </summary>
        /// <returns>Id заказа</returns>
        private Orders AddOrder()
        {
            List<IGrouping<int, Basket>> basket = new List<IGrouping<int, Basket>>();
            OrderTemp OrderTmp = new OrderTemp();

            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                basket = db.Basket.Where(b => b.FollowerId == FollowerId && b.Enable && b.BotInfoId==BotInfo.Id).GroupBy(b => b.ProductId).ToList();
                OrderTmp = db.OrderTemp.Where(o => o.FollowerId == FollowerId && o.BotInfoId==BotInfo.Id).FirstOrDefault();
                decimal? Number = 0;
                var LastOrder = db.Orders.OrderByDescending(o => o.Id).FirstOrDefault();

                if (LastOrder != null)
                    Number = LastOrder.Number;

                if (OrderTmp != null && OrderTmp.PaymentTypeId != null)
                {
                    Orders NewOrder = new Orders
                    {
                        DateAdd = DateTime.Now,
                        FollowerId = FollowerId,
                        Text = OrderTmp.Text,
                        Number = Number + 1,
                        PaymentTypeId = OrderTmp.PaymentTypeId,
                        Paid = false,
                        BotInfoId=BotInfo.Id
                    };

                    db.Orders.Add(NewOrder);
                    db.SaveChanges();
                    db.OrderAddress.Add(
                        new OrderAddress
                        {
                            OrderId = NewOrder.Id,
                            AdressId = Convert.ToInt32(OrderTmp.AddressId)
                        });

                    foreach (var group in basket)
                       FromBasketToOrderPosition(group.ElementAt(0).ProductId, NewOrder.Id, group);

                    

                    db.OrderTemp.Remove(OrderTmp);
                    db.SaveChanges();

                    return NewOrder;
                }

                else
                    return null;
            }
        }

        /// <summary>
        /// Добавить отзыв к заказу
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> SaveNewFeedBack()
        {
            int orderid = 0;
            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                try
                {
                    int number = Convert.ToInt32(base.OriginalMessage.Substring(ForceReplyAddFeedBack.Length));

                    var order = db.Orders.Where(o => o.Number == number).FirstOrDefault();

                    if (order != null)
                        orderid = order.Id;

                    var feed = db.FeedBack.Where(f => f.OrderId == orderid);

                    string text = base.ReplyToMessageText;

                    if (feed != null && feed.Count() == 0) // если отзывов еще нет
                    {
                        FeedBack feedBack = new FeedBack
                        {
                            DateAdd = DateTime.Now,
                            OrderId = orderid,
                            Text = text
                        };

                        db.FeedBack.Add(feedBack);

                        if (db.SaveChanges() > 0)
                        {
                            await SendMessage(new BotMessage { TextMessage = "Ваш отзыв добавлен. Спасибо!" });

                            var Admins = db.Admin.Include(a => a.Follower).ToList();

                            NewFeedBackAddedMsg = new NewFeedBackAddedMessage(order); // Уведомляем всех о новом отзыве
                            NewFeedBackAddedMsg.BuildMessage();
                            await SendMessageAllBotEmployeess(NewFeedBackAddedMsg);

                        }

                        return base.OkResult;
                    }

                    else // если отзыв уже есть
                        return base.OkResult;
                }

                catch
                {
                    return base.NotFoundResult;
                }
            }
        }

        /// <summary>
        /// Перенести данные из таблицы Basket в таблицу ORderProducr
        /// </summary>
        /// <param name="ProductId"></param>
        /// <param name="OrderId"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        private OrderProduct FromBasketToOrderPosition(int ProductId, int OrderId, IGrouping<int, Basket> group)
        {
            var Summa = group.Sum(p => p.Amount);

            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                ProductPrice price = db.ProductPrice.Where(p => p.ProductId == ProductId && p.Enabled).FirstOrDefault();


                if (price != null)
                {
                    OrderProduct orderProduct = new OrderProduct
                    {
                        ProductId = ProductId,
                        OrderId = OrderId,
                        DateAdd = DateTime.Now,
                        Count = Summa,
                        PriceId = price.Id,

                    };

                    foreach (var product in group)
                        db.Basket.Remove(product);

                    db.OrderProduct.Add(orderProduct);

                    db.SaveChanges();

                    return orderProduct;

                }

                else
                    return null;
            }
        }

        /// <summary>
        /// Добавить адрес доставки к заказу (в таблицу OrderTemp !!!)
        /// </summary>
        private void AddAddressToOrderTemp(int AddressId)
        {
            OrderTemp OrderTmp = new OrderTemp();

            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                OrderTmp = db.OrderTemp.Where(o => o.FollowerId == FollowerId && o.BotInfoId==BotInfo.Id).FirstOrDefault();
                if (OrderTmp != null && AddressId > 0)
                {
                    OrderTmp.AddressId = AddressId;
                    db.SaveChanges();
                }

                if (OrderTmp == null && AddressId > 0)
                {
                    db.OrderTemp.Add(new OrderTemp { AddressId = Argumetns[0], FollowerId = base.FollowerId , BotInfoId=BotInfo.Id});
                    db.SaveChanges();
                }
            }

        }

        /// <summary>
        /// Добавить метод оплаты к заказу (в таблицу OrderTemp !!!)
        /// </summary>
        /// <param name="PaymentTypeId"></param>
        /// <returns></returns>
        private int AddPaymentMethodToOrderTemp (int PaymentTypeId)
        {
            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                var order = db.OrderTemp.Where(o => o.FollowerId == FollowerId && o.BotInfoId==BotInfo.Id).FirstOrDefault();

                if (order != null && PaymentTypeId > 0)
                {
                    order.PaymentTypeId = PaymentTypeId;
                    return db.SaveChanges();
                }

                else
                    return 0;

            }
        }


        /// <summary>
        /// Проверяем поступил ли платеж на киви кошелек
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> CheckPay()
        {
            try
            {
                using (MarketBotDbContext db = new MarketBotDbContext())
                {
                    var order = db.Orders.Where(o => o.Id == OrderId && o.FollowerId == FollowerId).
                        Include(o => o.OrderConfirm).
                        Include(o => o.OrderDeleted).
                        Include(o => o.OrderDone).
                        Include(o => o.FeedBack).
                        Include(o => o.OrderProduct).
                        Include(o => o.OrderPayment).
                        Include(o => o.OrderAddress).FirstOrDefault();

                    double total = GeneralFunction.OrderTotalPrice(order.OrderProduct.ToList());
                    var QiwiPayment = await Services.Qiwi.QiwiFunction.SearchPayment(Convert.ToInt32(order.Number), db.QiwiApi.Where(q => q.Enable == true).FirstOrDefault().Token, db.QiwiApi.Where(q => q.Enable == true).FirstOrDefault().Telephone);

                    var payments = db.OrderPayment.Where(o => o.OrderId == order.Id).Include(o => o.Payment).ToList();

                    double debit = 0.0;

                    foreach (OrderPayment pay in payments)
                        debit += pay.Payment.Summ;


                    if (QiwiPayment != null && total <= debit + QiwiPayment.sum.amount && QiwiPayment.status == "SUCCESS")
                    {
                        AddPaymentToDb(order.Id, QiwiPayment.txnId, QiwiPayment.sum.amount,QiwiPayMethodId,true);
                        OrderViewMsg = new Messages.OrderViewMessage(order.Id);
                        await base.EditMessage(new BotMessage { TextMessage = "Платеж подтвержден!" , CallBackTitleText="" });
                        await OrderRedirectToAdmins(order.Id,order);

                    }

                    if (QiwiPayment != null && total <= debit + QiwiPayment.sum.amount && QiwiPayment.status != "SUCCESS")
                        await base.SendMessage(ChatId, new BotMessage { TextMessage = "Платеж еще не подтвержден!", CallBackTitleText = "" });


                    if (QiwiPayment != null && total > debit + QiwiPayment.sum.amount && QiwiPayment.status == "SUCCESS")
                        await base.SendMessage(ChatId, new BotMessage { TextMessage = "Необходимо доплатить еще " + (total - QiwiPayment.sum.amount).ToString() + " руб.", CallBackTitleText = "" });

                    if (QiwiPayment == null)
                        await base.SendMessage(ChatId, new BotMessage { TextMessage = "Платеж не найден!", CallBackTitleText = "" });

                    //if (QiwiPayment == null)
                    //{
                    //    AddPaymentToDb(order.Id, 1111, 123,QiwiPayMethodId,true);
                    //    OrderViewMsg = new Messages.OrderViewMessage(order);
                    //    await base.EditMessage(OrderViewMsg.BuildMessage());
                    //    await OrderRedirectToAdmins(order.Id, order);
                    //}

                    return OkResult;
                }
            }

            catch
            {
                return NotFoundResult;

            }
        }

        /// <summary>
        /// Заносим инф о платеже в БЖ
        /// </summary>
        /// <param name="orderID">id заказа</param>
        /// <param name="TxId">id транзакции в киви</param>
        /// <param name="Summ">сумма</param>
        /// <returns></returns>
        private OrderPayment AddPaymentToDb(int orderID, long TxId, double Summ,int PaymentTypeId=2, bool Paid=true)
        {
            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                var order = db.Orders.Where(o => o.Id == orderID).FirstOrDefault();

                order.Paid = Paid;

                Payment payment = new Payment
                {
                    DataAdd = DateTime.Now,
                    Comment = Bot.GeneralFunction.BuildPaymentComment(Bot.GeneralFunction.GetBotName(), order.Number.ToString()),
                    PaymentTypeId = PaymentTypeId,
                    TxId = TxId.ToString(),
                    Summ = Summ

                };

                db.Payment.Add(payment);

                db.SaveChanges();

                OrderPayment orderPayment = new OrderPayment
                {
                    OrderId = order.Id,
                    PaymentId = payment.Id
                };

                db.OrderPayment.Add(orderPayment);

                db.SaveChanges();

                return orderPayment;
            }
        }
    }
}
