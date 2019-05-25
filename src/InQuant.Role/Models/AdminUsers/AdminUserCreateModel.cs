using System;
using System.Collections.Generic;
using System.Text;

namespace InQuant.Security.Models.AdminUsers
{
    public class AdminUserCreateModel
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public bool IsAdmin { get; set; }
    }
}
