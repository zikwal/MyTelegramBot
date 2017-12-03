using System;
using System.Collections.Generic;

namespace MyTelegramBot
{
    public partial class FeedBack
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime? DateAdd { get; set; }
        public int? OrderId { get; set; }
        public int? RaitingId { get; set; }

        public Orders Order { get; set; }
        public Raiting Raiting { get; set; }
    }
}
