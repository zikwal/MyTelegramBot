using System;
using System.Collections.Generic;

namespace MyTelegramBot
{
    public partial class ShipPrice
    {
        public ShipPrice()
        {
            OrderAddress = new HashSet<OrderAddress>();
        }

        public int ShipPriceId { get; set; }
        public double? ShipPriceValue { get; set; }
        public bool? ShipPriceEnable { get; set; }

        public ICollection<OrderAddress> OrderAddress { get; set; }
    }
}
