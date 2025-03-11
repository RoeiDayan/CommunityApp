using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityApp.Models
{
    public class Member
    {
        
        public int ComId { get; set; }

        
        public int UserId { get; set; }

        
        public string? Role { get; set; }

        public int? Balance { get; set; }

        public int? UnitNum { get; set; }

        public bool? IsLiable { get; set; }

        public bool? IsResident { get; set; }

        public bool? IsManager { get; set; }

        public bool? IsProvider { get; set; }
        public bool? IsApproved { get; set; }

    }
}
