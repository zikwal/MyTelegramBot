using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyTelegramBot
{
    public partial class Payment
    {
        public Payment()
        {
            OrderPayment = new HashSet<OrderPayment>();
        }

        public int Id { get; set; }
        public int? PaymentTypeId { get; set; }
        public string TxId { get; set; }
        public DateTime? DataAdd { get; set; }
        public string Comment { get; set; }

        public double Summ { get; set; }

        public PaymentType PaymentType { get; set; }
        public ICollection<OrderPayment> OrderPayment { get; set; }
    }
}
