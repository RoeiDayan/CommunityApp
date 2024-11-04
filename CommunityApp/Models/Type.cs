using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityApp.Models
{
    public class Type
    {
        [Key]
        public int TypeNum { get; set; }

        [Column("Type")]
        [StringLength(10)]
        public string? Type1 { get; set; }

        //[InverseProperty("TypeNavigation")]
        //public virtual ICollection<Report> Reports { get; set; } = new List<Report>();

        public Models.Type GetType()
        {
            Models.Type type = new Models.Type();
            type.TypeNum = TypeNum;
            type.Type1 = Type1;
            return type;
        }
        public Type(Models.Type type)
        {
            this.TypeNum = type.TypeNum;
            this.Type1 = type.Type1;
        }
        //test
        //test
    }
}
