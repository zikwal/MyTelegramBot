using System;
using System.Collections.Generic;

namespace MyTelegramBot
{
    public partial class OrderDeleted
    {
        public int Id { get; set; }
        public int? OrderId { get; set; }
        public int? FollowerId { get; set; }
        public DateTime? DateAdd { get; set; }
        public bool? Deleted { get; set; }
        public string Text { get; set; }

        public Follower Follower { get; set; }
        public Orders Order { get; set; }
    }
}
