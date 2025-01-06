using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityApp.Models
{
    public class MemberCommunityDTO
    {
        public Member Member { get; set; }  // Membership data
        public Community Community { get; set; }  // Community data
    }

}
