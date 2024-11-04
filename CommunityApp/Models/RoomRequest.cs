using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityApp.Models
{
    public class RoomRequest
    {
        public int RequestId { get; set; }

        public int? UserId { get; set; }

        public int? ComId { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public string? Text { get; set; }

        public bool? IsApproved { get; set; }

        public DateTime? CreatedAt { get; set; }

    }
}
