using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityApp.Models
{
    public class Report
    {
        public int ReportId { get; set; }

        public int? UserId { get; set; }

        public int? ComId { get; set; }
        public string? Text { get; set; }

        public int? Priority { get; set; }

        public int? Type { get; set; }

        public int? Status { get; set; }

        public bool? IsAnon { get; set; }

        public DateTime? CreatedAt { get; set; }

        public string Title { get; set; } = null!;

    }
}
