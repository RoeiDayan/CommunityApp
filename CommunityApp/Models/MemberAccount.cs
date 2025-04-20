using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityApp.Models
{
    public class MemberAccount
    {
        public Member Member { get; set; }  // Membership data
        public Account Account { get; set; }
        public MemberAccount() { }
    }
}
