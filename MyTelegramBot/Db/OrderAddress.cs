using System;
using System.Collections.Generic;

namespace MyTelegramBot
{
    public partial class OrderAddress
    {
        public int AdressId { get; set; }
        public int OrderId { get; set; }
        public int? ShipPriceId { get; set; }

        public Address Adress { get; set; }
        public Orders Order { get; set; }
        public ShipPrice ShipPrice { get; set; }
    }
}
