using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace DnSrtChecker.Models.ViewModels
{
    public class RoleViewModel:IdentityRole
    {
        public virtual ICollection<UserRole> UserRoles { get; set; }

    }
}
