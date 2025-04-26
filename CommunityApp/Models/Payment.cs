using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityApp.Models
{
    public class Payment
    {
        public int PaymentId { get; set; }

        public int ComId { get; set; }

        public int UserId { get; set; }

        public int Amount { get; set; }

        public string? Details { get; set; }

        public bool? MarkedPayed { get; set; }

        public bool? WasPayed { get; set; }

        public DateOnly? PayFrom { get; set; }

        public DateOnly? PayUntil { get; set; }
    }
}
