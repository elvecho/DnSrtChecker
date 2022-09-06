using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace DnSrtChecker.Models.ViewModels
{
    public class UserViewModel:IdentityUser
    {
        public ICollection<UserRole> UserRoles { get; set; }
        public ICollection<Role> Roles { get; set; }
        public UserViewModel()
        {
            UserRoles = new Collection<UserRole>();
            Roles = new Collection<Role>();
        }
    }
}
