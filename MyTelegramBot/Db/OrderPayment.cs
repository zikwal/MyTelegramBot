using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyTelegramBot
{
    public class OrderPayment
    {
        public int PaymentId { get; set; }
        public int OrderId { get; set; }

        public Orders Order { get; set; }
        public Payment Payment { get; set; }

    }
}
