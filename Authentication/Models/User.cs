using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Authentication.Models
{
    public class User : IdentityUser
    {
        public bool IsChecked { get; set; }
        public string Status { get; set; }
        public DateTime LoginDate { get; set; }
        public DateTime RegisterDate { get; set; }
    }

    public class Users {
        public string Status { get; set; }
        public List<User> users { get; set; }
    }
}
