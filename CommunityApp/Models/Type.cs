using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityApp.Models
{
    public class Type
    {
        public int TypeNum { get; set; }

        public string? Type1 { get; set; }

        //[InverseProperty("TypeNavigation")]
        //public virtual ICollection<Report> Reports { get; set; } = new List<Report>();

    }
}
