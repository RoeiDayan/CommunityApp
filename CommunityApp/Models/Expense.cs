using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityApp.Models
{
    public class Expense
    {
        public int ExpenseId { get; set; }

        public int ComId { get; set; }

        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public decimal Amount { get; set; }

        public DateOnly ExpenseDate { get; set; }

        public DateTime CreatedAt { get; set; }
    
    }
}
