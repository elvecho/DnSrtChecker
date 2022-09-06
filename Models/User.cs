using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace DnSrtChecker.Models
{
    public class User : IdentityUser
    {
       
    //public string Name { get; set; }
    //  [JsonIgnore]
    public virtual ICollection<UserRole> UserRoles { get; set; }
        //public User()
        //{
        //    UserRoles = new Collection<UserRole>();
        //}

    }
}
