using CommunityApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityApp.Models
{
    public class Account
    {
        public int Id { get; set; }

        public string? Email { get; set; }

        public string? Name { get; set; }

        public string? Password { get; set; }

        public DateTime? CreatedAt { get; set; }
        public string? PhoneNumber { get; set; }

        public string? ProfilePhotoFileName { get; set; }

        public string ProfilePhotoUrl
        {
            get
            {
                return CommunityWebAPIProxy.ImageBaseAddress + ProfilePhotoFileName;
            }
        }

    }
}
