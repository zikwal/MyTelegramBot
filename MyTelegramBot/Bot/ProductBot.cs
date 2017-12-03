using System;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot.Types;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyTelegramBot.Messages;

namespace MyTelegramBot.Bot
{
    public class ProductBot : BotCore
    {
        private int ProductId { get; set; }

        private ProductRemoveFromBasket ProductRemoveFromBasketMsg { get; set; }

        private ProductViewMessage ProductViewMsg { get; set; }

        private AddProductToBasketMessage AddProductToBasketMsg { get; set; }

        public const string GetProductCmd = "GetProduct";

        public const string AddToBasketCmd = "AddToBasket";

        public const string RemoveFromBasketCmd = "RemoveFromBasket";

        public const string MoreInfoProductCmd = "MoreInfoProduct";

        public const string ProductCmd = "/product";

        public ProductBot(Update _update) : base(_update)
        {
           
        }

        protected override void Constructor()
        {
            try
            {
                if (base.Argumetns.Count > 0)
                {
                    ProductId = Argumetns[0];
                    ProductViewMsg = new ProductViewMessage(this.ProductId,BotInfo.Id);
                    AddProductToBasketMsg = new AddProductToBasketMessage(base.FollowerId, this.ProductId,BotInfo.Id);
                    ProductRemoveFromBasketMsg = new ProductRemoveFromBasket(this.FollowerId, this.ProductId,BotInfo.Id);
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
                //Пользователь нажал на кнопку вперед или назад при листнинге товаров
                case GetProductCmd:
                    return await GetProduct();

                //Пользователь нажал на +, добавил товар в корзину в кол-во 1 шт.
                case AddToBasketCmd:
                    return await AddToBasket();

                //Пользователь нажалн на-, удалил товар из корзины в кол-во 1 шт.
                case RemoveFromBasketCmd:
                    return await RemoveFromBasket();

                //Пользователь нажал "Подробнее" 
                case MoreInfoProductCmd:
                    return await MoreInfoProduct();
            }

            //inlene поиск
            if (Update.InlineQuery != null && Update.InlineQuery.Query!="")
                await ProductInlineSearch(Update.InlineQuery.Query);

            //ПОльзователь через инлай режим отправил в чат навзание 
            //товара. Отправляем пользователю сообщение с этим товаром
            if (Update.Message != null && Update.Message.Text != null && Update.Message.Text.Length > 0 && Connection.getConnection().Product.Where(p => p.Name == CommandName).FirstOrDefault() != null)
            {
                ProductViewMsg = new ProductViewMessage(base.CommandName);
                return await GetProduct();
            }

            //команда /product
            if (base.CommandName.Contains(ProductCmd))
                return await GetProductCommand();

            else
                return null;
        }

        private async Task<IActionResult> ProductInlineSearch(string Query)
        {
            ProductSearchInline productSearchInline = new ProductSearchInline(Query);

            if (await base.AnswerInlineQueryAsync(productSearchInline.ProductInlineSearch()))
                return OkResult;

            else
                return base.NotFoundResult;
        }

        private async Task<IActionResult> AddToBasket()
        {
            var message = AddProductToBasketMsg.BuildMessage();

            if (await base.AnswerCallback(message.CallBackTitleText))
                return base.OkResult;

            else
                return base.NotFoundResult;
        }

        /// <summary>
        /// Срабатывает по команде /product[id] напримиер /product12
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> GetProductCommand()
        {
            try
            {
                int id = Convert.ToInt32(base.CommandName.Substring(ProductCmd.Length));

                ProductViewMsg = new ProductViewMessage(id,BotInfo.Id);
                return await GetProduct();
            }

            catch
            {
                return base.NotFoundResult;
            }
        }

        private async Task<IActionResult> GetProduct()
        {
            if (ProductViewMsg != null)
            {
                ProductViewMsg = ProductViewMsg.BuildMessage();
                if (ProductViewMsg!=null && await SendPhoto(ProductViewMsg) != null)
                    return base.OkResult;
                else
                    return base.NotFoundResult;
            }
            else
                return base.NotFoundResult;

        }

        private async Task<IActionResult> RemoveFromBasket()
        {
            var message= ProductRemoveFromBasketMsg.BuildMessage();
            if (await AnswerCallback(message.CallBackTitleText))
                return base.OkResult;

            else
                return base.NotFoundResult;

        }

        private async Task<IActionResult> MoreInfoProduct()
        {
            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                if (await SendMessage(new BotMessage
                {
                    TextMessage = db.Product.Where(p => p.Id == ProductId).FirstOrDefault().TelegraphUrl
                }) != null)
                    return base.OkResult;

                else
                    return base.NotFoundResult;
            }
        }
    }
}
