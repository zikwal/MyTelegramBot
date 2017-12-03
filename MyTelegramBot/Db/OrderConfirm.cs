using System;
using System.Collections.Generic;

namespace MyTelegramBot
{
    public partial class OrderConfirm
    {
        public int Id { get; set; }
        public DateTime? DateAdd { get; set; }
        public string Text { get; set; }
        public int? FollowerId { get; set; }
        public int? OrderId { get; set; }
        public bool? Confirmed { get; set; }

        public Follower Follower { get; set; }
        public Orders Order { get; set; }
    }
}
