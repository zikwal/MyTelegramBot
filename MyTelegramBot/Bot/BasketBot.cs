﻿using System;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot.Types;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyTelegramBot.Messages;

namespace MyTelegramBot.Bot
{
    public class BasketBot:BotCore
    {

        public const string ViewBasketCmd = "ViewBasket";

        public const string BasketEditCmd = "EditBasket";

        public const string BackToBasketCmd = "BackToBasket";

        public const string AddProductToBasketCmd = "AddProductToBasket";

        public const string RemoveProductFromBasketCmd = "RemoveProductFromBasket";

        public const string BackToBasketPositionCmd = "BackToBasketPosition";

        public const string EditBasketProductCmd = "EditBasketProduct";

        public const string ClearBasketCmd = "ClearBasket";

        ViewBasketMessage ViewBasketMsg { get; set; }

        BasketPositionListMessage  BasketPositionListMsg { get; set; }

        BasketPositionEditMessage BasketPositionEditMsg { get; set; }

        private int ProductId { get; set; }

        public BasketBot(Update _update) : base(_update)
        {
           
        }

        protected override void Constructor()
        {
            try
            {
                ViewBasketMsg = new ViewBasketMessage(base.FollowerId);
                BasketPositionListMsg = new BasketPositionListMessage(base.FollowerId);


                if (base.Argumetns.Count > 0)
                {
                    this.ProductId = Argumetns[0];
                    BasketPositionEditMsg = new BasketPositionEditMessage(base.FollowerId, this.ProductId);
                }
            }

            catch
            {

            }
        }

        public async override Task<IActionResult> Response()
        {

            switch (base.CommandName)
            {
                case ViewBasketCmd:
                    return await ViewBasket();

                case ClearBasketCmd:
                    return await ClearBasket();

                case BasketEditCmd:
                    return await PositionList();

                case BackToBasketCmd:
                    return await ViewBasket(base.MessageId);

                case EditBasketProductCmd:
                    return await EditPositionMsg();

                case AddProductToBasketCmd:
                    return await AddToBasket();

                case RemoveProductFromBasketCmd:
                    return await RemoveFromBasket();

                case BackToBasketPositionCmd:
                    return await PositionList();

                default:
                    return null;
            }             

               
        }

        private async Task<IActionResult> ViewBasket(int Message=0)
        {
            ViewBasketMsg.BuildMessage();
            if (ViewBasketMsg!=null && await SendMessage(ViewBasketMsg,Message) != null)
                return base.OkResult;

            else
                return base.NotFoundResult;
        }

        private async Task<IActionResult> ClearBasket()
        {
            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                var basket = db.Basket.Where(b => b.FollowerId == FollowerId);

                foreach (var product in basket)
                    db.Remove(product);

                if (db.SaveChanges()>0 && await EditMessage(new BotMessage { TextMessage="Корзина пуста"}) != null)
                    return base.OkResult;

                else
                    return base.NotFoundResult;
            }
        }

        private async Task<IActionResult> PositionList()
        {
            if (await EditMessage(BasketPositionListMsg.BuildMessage()) != null)
                return base.OkResult;

            else
                return base.NotFoundResult;

        }

        private async Task<IActionResult> EditPositionMsg()
        {
            if (await EditMessage(BasketPositionEditMsg.BuildMessage()) != null)
                return base.OkResult;

            else
                return base.NotFoundResult;

        }

        private async Task<IActionResult> AddToBasket()
        {
            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                Basket basket = new Basket
                {
                    FollowerId = FollowerId,
                    ProductId = ProductId,
                    Enable = true,
                    DateAdd = DateTime.Now,
                    Amount = 1,
                    BotInfoId=BotInfo.Id
                };

                db.Add(basket);

                db.SaveChanges();

                return await EditPositionMsg();
            }
        }

        private async Task<IActionResult> RemoveFromBasket()
        {
            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                var basket = db.Basket.Where(b => b.ProductId == ProductId && b.FollowerId == FollowerId && b.BotInfoId==BotInfo.Id).FirstOrDefault();

                if (basket != null)
                {
                    db.Remove(basket);

                    db.SaveChanges();

                    return await EditPositionMsg();
                }

                else
                    return base.OkResult;
            }
        }
    }
}
