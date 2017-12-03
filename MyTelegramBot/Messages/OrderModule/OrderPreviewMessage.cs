using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.EntityFrameworkCore;

namespace MyTelegramBot.Messages
{
    /// <summary>
    /// Сообщение с заказом, из таблицы OrderTemp
    /// </summary>
    public class OrderTempMessage:Bot.BotMessage
    {
        InlineKeyboardCallbackButton SendBtn { get; set; }

        InlineKeyboardCallbackButton DescEditorBtn { get; set; }

        InlineKeyboardCallbackButton AddressEditor { get; set; }

        InlineKeyboardCallbackButton PaymentMethodEditor { get; set; }

        private int FollowerId { get; set; }

        private OrderTemp OrderTemp { get; set; }

        private int BotId { get; set; }

        public OrderTempMessage(int FollowerId, int BotId)
        {
            this.FollowerId = FollowerId;
            this.BotId = BotId;
        }

        public OrderTempMessage BuildMessage()
        {
            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                OrderTemp = db.OrderTemp.Where(o => o.FollowerId == FollowerId && o.BotInfoId==BotId).Include(o=>o.PaymentType).FirstOrDefault();

                if (OrderTemp != null)
                {
                    var Address = db.Address.Where(a => a.Id == OrderTemp.AddressId).Include(a => a.House).Include(a => a.House.Street).Include(a => a.House.Street.City).FirstOrDefault();

                    string PositionInfo = BasketPositionInfo.GetPositionInfo(FollowerId);

                    string Desc = "-";

                    string PaymentMethod = "-";

                    if (OrderTemp.PaymentType != null && OrderTemp.PaymentType.Name != null)
                        PaymentMethod = OrderTemp.PaymentType.Name;

                    if (PositionInfo != null)
                    {

                        if (OrderTemp.Text != null)
                            Desc = OrderTemp.Text;

                        base.TextMessage = "Информация о заказе:" +
                                    NewLine() + PositionInfo +
                                    NewLine() + Bold("Адрес доставки: ") + Address.House.Street.City.Name + ", " + Address.House.Street.Name + ", " + Address.House.Number +
                                    NewLine()+  Bold("Способ оплаты:")+PaymentMethod+
                                    NewLine() + Bold("Кoмментарий к заказу: ") + Desc;

                        SendBtn = new InlineKeyboardCallbackButton("Сохрнаить" + " \ud83d\udcbe", BuildCallData(Bot.OrderBot.CmdOrderSave));
                        DescEditorBtn = new InlineKeyboardCallbackButton("Комментарий к заказу" + " \ud83d\udccb", BuildCallData(Bot.OrderBot.CmdOrderDesc));
                        AddressEditor = new InlineKeyboardCallbackButton("Изменить адрес"+ " \ud83d\udd8a", BuildCallData(Bot.OrderBot.CmdAddressEditor));
                        PaymentMethodEditor = new InlineKeyboardCallbackButton("Изменить способ оплаты" + " \ud83d\udd8a", BuildCallData("PaymentMethodEditor"));

                        SetInlineKeyBoard();
                        return this;
                    }

                    else
                        return null;
                }

                else
                    return null;
            }
        }

        private void SetInlineKeyBoard()
        {
            base.MessageReplyMarkup = new InlineKeyboardMarkup(
                new[]{
                new[]
                        {
                            DescEditorBtn
                        },
                new[]
                        {
                            AddressEditor
                        },
                new[]
                        {
                            SendBtn
                        }

                 });
        }
    }


    public static class BasketPositionInfo
    {
        public static string GetPositionInfo(int FollowerID)
        {
            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                var basket = db.Basket.Where(b => b.FollowerId == FollowerID && b.Enable);

                var IdList = basket.Select(b => b.ProductId).Distinct().AsEnumerable();

                int counter = 1;

                double total = 0.0;

                string message = String.Empty;

                if (IdList.Count() > 0)
                {
                    foreach (int id in IdList)
                    {
                        string name = db.Product.Where(p => p.Id == id).FirstOrDefault().Name;
                        int count = basket.Where(p => p.ProductId == id).Count();
                        double price = db.ProductPrice.Where(p => p.ProductId == id && p.Enabled).FirstOrDefault().Value;
                        message += counter.ToString() + ") " + name + " " + count.ToString() + " x " + price.ToString() + " руб. " + " = " + (count * price).ToString() + " руб." + Bot.BotMessage.NewLine();
                        total += price * count;
                        counter++;

                    }

                    return message + Bot.BotMessage.NewLine() + Bot.BotMessage.Bold("Общая стоимость: ") + total.ToString() + " руб.";
                }

                else
                    return null;
            }
        }
    }
}
