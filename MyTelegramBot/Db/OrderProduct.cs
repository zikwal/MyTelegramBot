using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace MyTelegramBot
{
    public partial class OrderProduct
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Count { get; set; }
        public int PriceId { get; set; }
        public DateTime? DateAdd { get; set; }

        public Orders Order { get; set; }
        public ProductPrice Price { get; set; }
        public Product Product { get; set; }

        public override string ToString()
        {
            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                if (Product.Unit == null)
                    Product.Unit = db.Units.Where(u => u.Id == Product.UnitId).FirstOrDefault();

            }

            if (Price != null && Product != null && Product.Unit != null)
                return Product.Name + " " + Price.Value.ToString() + " руб. " +
                    " x " + Count.ToString() + " " + Product.Unit.ShortName + " = " + (Count * Price.Value).ToString() + " руб.";

            else
                return String.Empty;
        }

        public string AdminText()
        {
            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                if (Product.Unit == null)

                    Product.Unit = db.Units.Where(u => u.Id == Product.UnitId).FirstOrDefault();

                if (Product.Stock == null || Product.Stock.Count == 0)
                    Product.Stock = db.Stock.Where(s => s.ProductId == ProductId).OrderByDescending(s => s.Id).ToList();
            }

            if (Price != null && Product != null && Product.Unit != null && Product.Stock.Count > 0 && Product.Stock.FirstOrDefault().Balance >=
                Count)
                return Product.Name + " " + Price.Value.ToString() + " руб. " +
                    " x " + Count.ToString() + " " + Product.Unit.ShortName + " = " + (Count * Price.Value).ToString() + " руб."
                    + " | в наличии: " + Product.Stock.FirstOrDefault().Balance.ToString() + " " + Product.Unit.ShortName;

            //если заказывают больше чем есть. Выделяем жирным
            if (Price != null && Product != null && Product.Unit != null && Product.Stock.Count > 0 && Product.Stock.FirstOrDefault().Balance <
                Count)
                return Product.Name + " " + Price.Value.ToString() + " руб. " +
                    " x " + Count.ToString() + " " + Product.Unit.ShortName + " = " + (Count * Price.Value).ToString() + " руб."
                    + " |" + Bot.BotMessage.Bold("в наличии: " + Product.Stock.FirstOrDefault().Balance.ToString() + " " + Product.Unit.ShortName);

            //в наличии 0
            if (Price != null && Product != null && Product.Unit != null && Product.Stock.Count == 0)
                return Product.Name + " " + Price.Value.ToString() + " руб. " +
                    " x " + Count.ToString() + " " + Product.Unit.ShortName + " = " + (Count * Price.Value).ToString() + " руб."
                    + " |" + Bot.BotMessage.Bold(" в наличии: 0 " + Product.Unit.ShortName);

            else
                return String.Empty;
        }
    }
}
