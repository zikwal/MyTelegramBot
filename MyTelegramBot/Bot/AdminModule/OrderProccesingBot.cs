using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using System.IO;
using MyTelegramBot.Messages.Admin;
using MyTelegramBot.Messages;
using Microsoft.EntityFrameworkCore;

namespace MyTelegramBot.Bot.AdminModule
{
    public partial class OrderProccesingBot : Bot.BotCore
    {
        StockChangesMessage StockChangesMsg { get; set; }
       
        /// <summary>
        /// Сообщение с описание заказа и админскими кнопками
        /// </summary>
        private AdminOrderMessage OrderAdminMsg { get; set; }


        /// <summary>
        /// Сообщение с позициями заказа. Каждай позиция отдельная кнопка
        /// </summary>
        private OrderPositionListMessage OrderPositionListMsg { get; set; }


        /// <summary>
        /// Предложить отсвить отзыв
        /// </summary>
        private FeedBackOfferMessage FeedBackOfferMsg { get; set; }

        private int OrderId { get; set; }

        private Orders Order { get; set; }

        /// <summary>
        /// Админ. Показать номере телефона клиента
        /// </summary>
        public const string CmdGetTelephone = "GetTelephone";

        /// <summary>
        /// Админ. Показать заказ для Админа
        /// </summary>
        public const string CmdGetOrderAdmin = "GetOrderAdmin";

        /// <summary>
        /// Админ. Изменить позиции заказа
        /// </summary>
        public const string CmdEditOrderPosition = "EditOrderPosition";

        /// <summary>
        /// Обработать заказ. (Согласовать, Удалить и т.д)
        /// </summary>
        public const string CmdProccessOrder = "ProccessOrder";

        /// <summary>
        /// Показать адрес доставки на карте
        /// </summary>
        public const string CmdViewAddressOnMap = "ViewAddressOnMap";

        /// <summary>
        /// Удалить заказ
        /// </summary>
        public const string CmdOrderDelete = "OrderDelete";

        /// <summary>
        /// Заказа согласован
        /// </summary>
        public const string CmdConfirmOrder = "ConfirmOrder";

        /// <summary>
        /// Восстановить заказ (Если удален)
        /// </summary>
        public const string CmdRecoveryOrder = "RecoveryOrder";

        /// <summary>
        /// Заказ выполнен
        /// </summary>
        public const string CmdDoneOrder = "DoneOrder";


        /// <summary>
        /// Назад
        /// </summary>
        public const string CmdBackToOrder = "BackToOrder";

        private const string ForceReplyOrderDelete = "Удалить заказ:";

        private const string ForceReplyOrderDone = "Выполнить заказ:";

        private const string ForceReplyOrderConfirm = "Согласовать заказ:";

        private const string ForceReplyAddFeedBack = "Добавить отзыв к заказу:";

        private const string GetOrderCmd = "/order";

        private IProcessing Processing { get; set; }
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="_update"></param>
        public OrderProccesingBot(Update _update) : base(_update)
        {


        }

        protected override void Constructor()
        {
            Processing = new OrderProcess(Update);
            if (Update.Message != null && Update.Message.ReplyToMessage != null)
                CommandName = Update.Message.ReplyToMessage.Text;

            try
            {
                if (base.Argumetns.Count > 0)
                {
                    OrderId = Argumetns[0];
                    OrderAdminMsg = new AdminOrderMessage(this.OrderId,FollowerId);
                    OrderPositionListMsg = new OrderPositionListMessage(this.OrderId);
                    FeedBackOfferMsg = new FeedBackOfferMessage(this.OrderId);
                    using (MarketBotDbContext db = new MarketBotDbContext())
                        Order = db.Orders.Where(o => o.Id == this.OrderId).Include(o => o.OrderConfirm).
                            Include(o => o.OrderDone).Include(o => o.OrderDeleted).Include(o => o.OrderProduct).
                            Include(o => o.Follower).Include(o => o.FeedBack).Include(o=>o.OrderAddress).Include(o=>o.OrdersInWork).FirstOrDefault();

                }

            }

            catch
            {

            }

        }

        public async override Task<IActionResult> Response()
        {
            if (IsOperator() || IsOwner())
            {
                switch (base.CommandName)
                {
                    ///Сообщение с деталями заказа для администратора системы
                    case CmdGetOrderAdmin :
                        return await GetOrderAdmin();

                    case "OpenOrder":
                        return await GetOrderAdmin();

                    ///Администратор нажал на кнопку "показать номер телефона"
                    case CmdGetTelephone:
                        return await GetContact();

                    ///Адмнистратор нажал на кнопку "Показать на карте"
                    case CmdViewAddressOnMap:
                        return await SendOrderAddressOnMap();

                    ///Администратор нажал на кнопку "Восстановить заказ"
                    case CmdRecoveryOrder:
                        return await OrderRecovery();

                    ///Администратор нажал на кнопку "Заказ выполнен"
                    case CmdDoneOrder:
                        return await OrderDone();

                    ///Пользователь нажал на кнопку "Назад"
                    case CmdBackToOrder:
                        return await BackToOrder();

                    case CmdEditOrderPosition:
                        return await SendEditorOrderPositionList();

                    case "TakeOrder":
                        return await TakeOrder();

                    case "FreeOrder":
                        return await FreeOrder();

                    default:
                        break;

                }

                //Администратор прислал ответ на сообщение "Удалить заказ:"
                if (OriginalMessage.Contains(ForceReplyOrderDelete))
                    return await OrderDelete();

                //Администратор прислал ответ на сообщение "Согласовать заказ:"
                if (OriginalMessage.Contains(ForceReplyOrderConfirm))
                    return await OrderConfirm();


                //Администратор нажал на кнопку "Удалить заказ"
                if (base.CommandName == CmdOrderDelete && Order != null)
                    return await ForceReplyBuilder(ForceReplyOrderDelete + Order.Number.ToString());

                //Администратор нажал на кнопку "Заказ согласован"
                if (base.CommandName == CmdConfirmOrder && Order != null)
                    return await ForceReplyBuilder(ForceReplyOrderConfirm + Order.Number.ToString());

                /// /order показать заказ для админа
                if (base.CommandName.Contains(GetOrderCmd))
                    return await GetOrder();

                else
                    return null;
            }


            else
                return null;


        }


        private async Task<IActionResult> GetOrder()
        {
            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                try
                {
                    int number = Convert.ToInt32(base.CommandName.Substring(GetOrderCmd.Length));

                    int id = db.Orders.Where(o => o.Number == number).FirstOrDefault().Id;

                    OrderAdminMsg = new AdminOrderMessage(id);
                    await SendMessage(OrderAdminMsg.BuildMessage());

                    return OkResult;
                }

                catch
                {
                    return OkResult;
                }
            }

        }

        /// <summary>
        /// Освободить заказ.
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> FreeOrder()
        {
            try
            {
                if (Order != null && await Processing.CheckInWork(Order))
                {
                    using (MarketBotDbContext db = new MarketBotDbContext())
                    {
                        OrdersInWork ordersInWork = new OrdersInWork
                        {
                            Timestamp = DateTime.Now,
                            InWork = false,
                            OrderId = Order.Id,
                            FollowerId = FollowerId
                        };

                        db.OrdersInWork.Add(ordersInWork);

                        if (db.SaveChanges() > 0)
                        {
                            Order.OrdersInWork.Add(ordersInWork);
                            OrderAdminMsg = new AdminOrderMessage(Order, FollowerId);
                            await base.EditMessage(OrderAdminMsg.BuildMessage());
                            await Processing.NotifyChanges("Пользователь " + GeneralFunction.FollowerFullName(FollowerId) + " освободил заказ №" + Order.Number.ToString(), Order.Id);
                        }
                        return OkResult;
                    }
                }

                else
                    return OkResult;
            }

            catch
            {
                return OkResult;
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

                if (order.Follower != null && order.Follower.UserName != null && order.Follower.UserName != "")
                {
                    string url = Bot.BotMessage.HrefUrl("https://t.me/" + order.Follower.UserName, order.Follower.UserName);
                    await SendMessage(new BotMessage { TextMessage = url });
                    return OkResult;
                }

                else
                    return base.OkResult;
            }
        }

        /// <summary>
        /// Показать адрес доставки на карте
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> SendOrderAddressOnMap()
        {
            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                var Address = db.OrderAddress.Where(o => o.OrderId == OrderId).Include(o => o.Adress.House).FirstOrDefault();


                Location location = new Location
                {
                    Latitude = Convert.ToSingle(Address.Adress.House.Latitude),
                    Longitude = Convert.ToSingle(Address.Adress.House.Longitude)
                };

                if (await SendLocation(location) != null)
                    return OkResult;

                else
                    return NotFoundResult;
            }

        }


        private async Task<IActionResult> GetOrderAdmin()
        {
            if (OrderAdminMsg == null)
                OrderAdminMsg = new AdminOrderMessage(OrderId,FollowerId);

            if (await SendMessage(OrderAdminMsg.BuildMessage()) != null)
                return base.OkResult;


            else
                return base.NotFoundResult;
        }

        /// <summary>
        /// Позиции заказа. Кнопками
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> SendEditorOrderPositionList()
        {
            if (await EditMessage(OrderPositionListMsg.BuildMessage()) != null)
                return base.OkResult;

            else
                return base.NotFoundResult;
        }

        private async Task<IActionResult> TakeOrder()
        {
            try
            {
                using(MarketBotDbContext db=new MarketBotDbContext())
                {
                    OrdersInWork inWork = new OrdersInWork { FollowerId = FollowerId, Timestamp = DateTime.Now, OrderId = Order.Id, InWork = true };
                    db.OrdersInWork.Add(inWork);

                    var InWorkNow = Order.OrdersInWork.OrderByDescending(o => o.Id).FirstOrDefault();

                    if (Order!=null  && InWorkNow==null && db.SaveChanges() > 0 ||
                        Order != null && InWorkNow != null&&
                        InWorkNow.FollowerId!=FollowerId && InWorkNow.InWork==false && db.SaveChanges() > 0)
                    {
                        Order.OrdersInWork.Add(inWork);
                        OrderAdminMsg = new AdminOrderMessage(Order,FollowerId);
                        await EditMessage(OrderAdminMsg.BuildMessage());
                        string notify = "Заказ №" + this.Order.Number.ToString() + " взят в работу. Пользователь " + GeneralFunction.FollowerFullName(base.FollowerId);
                        await Processing.NotifyChanges(notify, this.Order.Id);
                    }

                    if (InWorkNow != null && InWorkNow.FollowerId != FollowerId && InWorkNow.InWork==true)
                        await SendMessage(new BotMessage { TextMessage = "Заявка в обработке у " + GeneralFunction.FollowerFullName(InWorkNow.FollowerId) });

                    //заявка уже в обработке у пользователя
                    if (InWorkNow != null && InWorkNow.FollowerId == FollowerId && InWorkNow.InWork == true)
                    {
                        OrderAdminMsg = new AdminOrderMessage(Order, FollowerId);
                        await EditMessage(OrderAdminMsg.BuildMessage());
                    }

                        return OkResult;
                }
            }

            catch
            {
                return NotFoundResult;
            }
        }

        /// <summary>
        /// Удалить заказ
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> OrderDelete()
        {
            int number = 0;
            int id = 0;
            try
            {
                number = Convert.ToInt32(base.OriginalMessage.Substring(ForceReplyOrderDelete.Length));

                using (MarketBotDbContext db = new MarketBotDbContext())
                {
                    Order = db.Orders.Where(o => o.Number == number).Include(o => o.OrderDeleted).Include(o=>o.OrdersInWork).FirstOrDefault();

                    string text = base.ReplyToMessageText;
                    if (Order != null && Order.OrderDeleted != null && Order.OrderDeleted.Count == 0 
                        && await Processing.CheckInWork(Order) && !await Processing.CheckIsDone(Order))
                    {
                        id = Order.Id;
                        OrderDeleted orderDeleted = new OrderDeleted
                        {
                            DateAdd = DateTime.Now,
                            Deleted = true,
                            FollowerId = FollowerId,
                            OrderId = id,
                            Text = text

                        };

                        db.OrderDeleted.Add(orderDeleted);
                        db.SaveChanges();
                    }
                }

                OrderAdminMsg = new AdminOrderMessage(id,FollowerId);
                var message = OrderAdminMsg.BuildMessage();
                await SendMessage(message);
                //Уведомляем других сотрудников об изменениях
                string notify = "Заказ №" + Order.Number.ToString() + " Удален. Пользователь " + GeneralFunction.FollowerFullName(FollowerId);
                await Processing.NotifyChanges(notify, Order.Id);
                return base.OkResult;
            }

            catch
            {
                return OkResult;
            }


        }

        /// <summary>
        /// Согласовать заказ
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> OrderConfirm()
        {
            try
            {
                int number = Convert.ToInt32(base.OriginalMessage.Substring(ForceReplyOrderConfirm.Length));

                int id = 0;

                using (MarketBotDbContext db = new MarketBotDbContext())
                {
                    Order = db.Orders.Where(o => o.Number == number).Include(o => o.OrderConfirm).Include(o=>o.OrdersInWork).FirstOrDefault();

                    if (Order != null && Order.OrderConfirm != null && Order.OrderConfirm.Count == 0
                        && await Processing.CheckInWork(Order) && await Processing.CheckIsDone(Order) ==false) // Если уже есть записи о том что заказ соглосован, то больще записей не делаем
                    {
                        string text = base.ReplyToMessageText;
                        id = Order.Id;

                        OrderConfirm orderConfirm = new OrderConfirm
                        {
                            DateAdd = DateTime.Now,
                            Confirmed = true,
                            FollowerId = FollowerId,
                            OrderId = id,
                            Text = text
                        };

                        db.OrderConfirm.Add(orderConfirm);
                        db.SaveChanges();

                    }
                }
                
                OrderAdminMsg = new AdminOrderMessage(id,FollowerId);
                var message = OrderAdminMsg.BuildMessage();
                await SendMessage(message);
                string notify = "Заказ №" + this.Order.Number.ToString() + " согласован. Пользователь " + GeneralFunction.FollowerFullName(base.FollowerId);
                await Processing.NotifyChanges(notify, this.Order.Id);

                return base.OkResult;



            }
            catch (Exception exp)
            {
                return base.NotFoundResult;
            }
        }

        /// <summary>
        /// Предложение оставить отзыв
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> SendFeedBackOffer()
        {
            if (Order != null && Order.Follower != null && await SendMessage(Order.Follower.ChatId, FeedBackOfferMsg.BuildMessage()) != null)
                return OkResult;

            else
                return NotFoundResult;


        }

        /// <summary>
        /// Восстановить заказ
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> OrderRecovery()
        {
            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                try
                {
                    var deleted = db.OrderDeleted.Where(o => o.OrderId == OrderId);

                    if (deleted != null && deleted.Count() > 0)
                    {
                        foreach (var value in deleted)
                            db.OrderDeleted.Remove(value);

                        db.SaveChanges();
                    }

                    OrderAdminMsg = new AdminOrderMessage(OrderId, FollowerId);
                    await EditMessage(OrderAdminMsg.BuildMessage());
                    string notify = "Заказ №" + Order.Number.ToString() + " восстановлен. Пользователь " + GeneralFunction.FollowerFullName(FollowerId);
                    await Processing.NotifyChanges(notify, Order.Id);
                    return base.OkResult;
                }

                catch (Exception e)
                {
                    return base.NotFoundResult;
                }
            }

               
        }

        /// <summary>
        /// Выполнить заказ
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> OrderDone()
        {
            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                //Проверяем согласован ли заказ и не удален ли он и не был ли выполнен ранее
                if (this.Order != null && this.Order.OrderDeleted.Count == 0 && this.Order.OrderConfirm.Count > 0 && this.Order.OrderDone.Count == 0
                   && await Processing.CheckInWork(this.Order) && !await Processing.CheckIsDone(this.Order))
                {
                    OrderDone orderDone = new OrderDone
                    {
                        DateAdd = DateTime.Now,
                        FollowerId = FollowerId,
                        Done = true,
                        OrderId = OrderId
                    };

                    OrdersInWork inWork = new OrdersInWork
                    {
                        FollowerId = FollowerId,
                        Timestamp = DateTime.Now,
                        InWork = false,
                        OrderId = this.Order.Id
                    };


                    db.OrdersInWork.Add(inWork);
                    db.OrderDone.Add(orderDone);
                    db.SaveChanges();
                    StockChangesMsg=new StockChangesMessage(UpdateStock(this.Order));

                }
            }

            if (OrderAdminMsg!= null)
            {
     
                var message = OrderAdminMsg.BuildMessage();
                await EditMessage(message); // Редакатруем текущее сообщение на новое
                string notify = "Заказ №" + this.Order.Number.ToString() + " выполнен. Пользователь " +GeneralFunction.FollowerFullName(base.FollowerId);
                await Processing.NotifyChanges(notify, this.Order.Id); // уведомляем сотрудников о выполненом заказе
                await SendMessageAllBotEmployeess(StockChangesMsg.BuildMessage()); //уведомляем сотрудников об изменениях остатков
                return await SendFeedBackOffer(); // предлагаем пользователю оставить отзыв
            }
            else
                return base.NotFoundResult;
        }

        /// <summary>
        /// Назад
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> BackToOrder()
        {
            if (OrderAdminMsg != null && await EditMessage(OrderAdminMsg.BuildMessage()) != null)
                return base.OkResult;

            if (OrderAdminMsg == null && this.OrderId > 0)
            {
                OrderAdminMsg = new AdminOrderMessage(this.OrderId);
                await EditMessage(OrderAdminMsg.BuildMessage());
                return OkResult;
            }

            else
            {
                OrderId = Argumetns[0];
                OrderAdminMsg = new AdminOrderMessage(this.OrderId);
                await EditMessage(OrderAdminMsg.BuildMessage());
                return OkResult;
            }

        }


        /// <summary>
        /// После того как заказ выполнен, обновяем данные на складе
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        private List<Stock> UpdateStock(Orders order)
        {
            List<Stock> list = new List<Stock>();

            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                foreach (OrderProduct p in order.OrderProduct)
                {
                    var stock = db.Stock.Where(s => s.ProductId == p.ProductId).Include(s=>s.Product).OrderByDescending(s => s.Id).FirstOrDefault();

                    if (stock != null)
                    {
                        int Count = 0;

                        if (stock.Balance - p.Count < 0)
                            Count = Convert.ToInt32(stock.Balance);

                        else
                            Count = p.Count;

                        Stock newstock = new Stock
                        {
                            DateAdd = DateTime.Now,
                            ProductId = stock.ProductId,
                            Quantity = -1 * Count,
                            Balance = stock.Balance - Count,
                            Text = "Заказ:" + order.Number
                        };

                        db.Stock.Add(newstock);
                        db.SaveChanges();
                        list.Add(newstock);

                    }
                }

                return list;
                
            }
        }


    }
}
