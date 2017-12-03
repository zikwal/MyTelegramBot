using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;

namespace MyTelegramBot.Messages
{
    /// <summary>
    /// Сообещние с содержание корзины
    /// </summary>
    public class ViewBasketMessage:Bot.BotMessage
    {
        private InlineKeyboardCallbackButton ClearBasketBtn { get; set; }

        private InlineKeyboardCallbackButton ToAddressList { get; set; }

        private InlineKeyboardCallbackButton ReturnToCatalogBtn { get; set; }

        private InlineKeyboardCallbackButton BasketEditBtn { get; set; }

        private Follower follower { get; set; }

        private List<Basket> Basket { get; set; }
        private int FollowerId { get; set; }

        public ViewBasketMessage(int FollowerId)
        {
            this.FollowerId = FollowerId;
        }

        public ViewBasketMessage BuildMessage ()
        {
            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                follower = db.Follower.Where(f => f.Id == FollowerId).FirstOrDefault();
                Basket = db.Basket.Where(b => b.FollowerId == follower.Id && b.Enable).ToList();
            }

            if (Basket!=null && Basket.Count()>0)
            {
                ClearBasketBtn = ClearBasket(follower.Id);
                ToAddressList = ToCheckOut(follower.Id);
                BasketEditBtn = BasketEdit(follower.Id);
                SetInlineKeyBoard();
                string Info = BasketPositionInfo.GetPositionInfo(follower.Id);
                base.TextMessage = Bold("Ваша корзина:") + NewLine() + Info;
            }

            else
            {
                base.CallBackTitleText = "Ваша Корзина пуста";
            }


            return this;
        }

        private void SetInlineKeyBoard()
        {
            base.MessageReplyMarkup = new InlineKeyboardMarkup(
                new[]{
                new[]
                        {
                            BasketEditBtn
                        },
                new[]
                        {
                            ToAddressList
                        },
                new[]
                        {
                            ClearBasketBtn
                        },

                 });
        }

        private InlineKeyboardCallbackButton ClearBasket (int FollowerId)
        {
            string data = BuildCallData(Bot.BasketBot.ClearBasketCmd, FollowerId);
            InlineKeyboardCallbackButton button = new InlineKeyboardCallbackButton("Очистить корзину", data);
            return button;
        }

        private InlineKeyboardCallbackButton ToCheckOut(int FollowerId)
        {
            string data = BuildCallData(Bot.AddressBot.CmdGetAddressList, FollowerId);
            InlineKeyboardCallbackButton button = new InlineKeyboardCallbackButton("Перейти к оформлению", data);
            return button;
        }

        private InlineKeyboardCallbackButton BasketEdit(int FollowerId)
        {
            string data = BuildCallData(Bot.BasketBot.BasketEditCmd, FollowerId);
            InlineKeyboardCallbackButton button = new InlineKeyboardCallbackButton("Изменить", data);
            return button;
        }

    }
}
