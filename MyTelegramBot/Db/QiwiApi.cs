using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyTelegramBot
{
    public partial class QiwiApi
    {
        public int Id { get; set; }
        public DateTime? DateAdd { get; set; }
        public string Token { get; set; }
        public bool? Enable { get; set; }
        public string Telephone { get; set; }
        public int? BotInfoId { get; set; }

        public BotInfo BotInfo { get; set; }
    }
}
