using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityApp.Models
{
    public class Community
    {
        public int ComId { get; set; }

        public string? ComName { get; set; }

        public string? ComDesc { get; set; }

        public string? ComCode { get; set; }

        public string? GatePhoneNum { get; set; }

        public DateTime? CreatedAt { get; set; }

    }
}
