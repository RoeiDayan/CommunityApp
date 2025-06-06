using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityApp.Models
{
    public class RoomRequestWithMember
    {
        public RoomRequest Request { get; set; }
        public MemberAccount MemberAccount { get; set; }
    }
}
