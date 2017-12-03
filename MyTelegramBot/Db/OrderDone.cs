using System;
using System.Collections.Generic;

namespace MyTelegramBot
{
    public partial class OrderDone
    {
        public int Id { get; set; }
        public int? OrderId { get; set; }
        public DateTime? DateAdd { get; set; }
        public int? FollowerId { get; set; }
        public bool? Done { get; set; }
        public Follower Follower { get; set; }
        public Orders Order { get; set; }
    }
}
