using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityApp.Models
{
    public class TenantRoom
    {
        public int ComId { get; set; }

        public string? Status { get; set; }

        public int KeyHolderId { get; set; }

        public DateTime? SessionStart { get; set; }

        public DateTime? SessionEnd { get; set; }

    }
}
