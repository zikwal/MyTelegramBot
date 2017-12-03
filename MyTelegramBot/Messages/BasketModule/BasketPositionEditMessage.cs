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
    /// Сообщение с редактированием позиции Корзины
    /// </summary>
    public class BasketPositionEditMessage:Bot.BotMessage
    {
        private int FollowerId { get; set; }

        private int ProductId { get; set; }

        private InlineKeyboardCallbackButton AddBtn { get; set; }

        private InlineKeyboardCallbackButton RemoveBtn { get; set; }

        private InlineKeyboardCallbackButton BackToBasketBtn { get; set; }

        private List<Basket> Positions { get; set; }

        Product Product { get; set; }

        public BasketPositionEditMessage(int FollowerId, int ProductId)
        {
            this.FollowerId = FollowerId;
            this.ProductId = ProductId;
            BackBtn = new InlineKeyboardCallbackButton("Назад", BuildCallData(Bot.BasketBot.BackToBasketPositionCmd, this.FollowerId));
            BackToBasketBtn = new InlineKeyboardCallbackButton("Вернуться в корзину", BuildCallData(Bot.BasketBot.BackToBasketCmd));
        }

        public BasketPositionEditMessage BuildMessage()
        {
            using (MarketBotDbContext db=new MarketBotDbContext())
            {
                Positions= db.Basket.Where(b => b.FollowerId == FollowerId && b.ProductId == ProductId).Include(b => b.Product).ToList();
                Product = db.Product.Where(p => p.Id == ProductId).FirstOrDefault();
            }

            if (Positions != null && Positions.Count()>0)
            {
                base.TextMessage = Positions.FirstOrDefault().Product.Name + " " + Positions.Count() + " шт.";
                AddBtn = new InlineKeyboardCallbackButton("+", BuildCallData(Bot.BasketBot.AddProductToBasketCmd, Positions.FirstOrDefault().ProductId));
                RemoveBtn = new InlineKeyboardCallbackButton("-", BuildCallData(Bot.BasketBot.RemoveProductFromBasketCmd, Positions.FirstOrDefault().ProductId));
                SetInlineKeyBoard();
            }

            else // Пользователь удалил последнюю еденицу этого товара. Значит теперь его 0 шт.
            {
                base.TextMessage =Product.Name+ " 0 шт.";
                AddBtn = new InlineKeyboardCallbackButton("+", BuildCallData(Bot.BasketBot.AddProductToBasketCmd, ProductId));
                RemoveBtn = new InlineKeyboardCallbackButton("-", BuildCallData(Bot.BasketBot.RemoveProductFromBasketCmd, ProductId));
                SetInlineKeyBoard();
            }

            return this;
        }

        private void SetInlineKeyBoard()
        {
            base.MessageReplyMarkup = new InlineKeyboardMarkup(
                new[]{
                new[]
                        {
                            RemoveBtn,AddBtn
                        },
                new[]
                        {
                            BackBtn
                        },
                new[]
                        {
                            BackToBasketBtn
                        }

                 });
        }
    }
}
