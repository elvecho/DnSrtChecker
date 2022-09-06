using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DnSrtChecker.Models
{
    public class Role:IdentityRole
    {
        //public Role(string roleName) : base(roleName)
        //{
        //    UserRoles = new Collection<UserRole>();
        //}
        public virtual ICollection<UserRole> UserRoles { get; set; }

      
       
    }

}
