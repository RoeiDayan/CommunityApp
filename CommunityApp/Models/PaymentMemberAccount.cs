using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityApp.Models
{
    public class PaymentMemberAccount
    {
        public PaymentMemberAccount() { }
        public Payment Payment { get; set; }
        public Member Member { get; set; }
        public Account Account { get; set; }
    }
}
