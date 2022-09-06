using DnSrtChecker.Models;
using DnSrtChecker.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DnSrtChecker.Persistence
{
    public class Seed
    {
        public static void SeedUsers(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            var listRoles = new List<Role>();
            listRoles.Add(new Role
            {
                Name = "OperatorAdmin",
                NormalizedName = "OPERATORADMIN"
            });
            listRoles.Add(new Role
            {
                Name = "OperatorIT",
                NormalizedName = "OPERATORIT"
            });
            listRoles.Add(new Role
            {
                Name = "Archive",
                NormalizedName = "ARCHIVE"
            });
            listRoles.Add(new Role
            {
                Name = "Control",
                NormalizedName = "CONTROL"
            });
            foreach (Role element in listRoles)
            {
                if (!roleManager.RoleExistsAsync(element.Name).Result)
                {
                    IdentityResult resultRole = roleManager.CreateAsync(element).Result;
                }
            }


            var emails = new List<UserViewModel>();
            emails.Add(new UserViewModel { Email = "user.it@retexspa.com", PasswordHash = "it", UserName = "it", Roles = listRoles.Where(x => x.Name == "OperatorIT").ToList() }); ;
            emails.Add(new UserViewModel { Email = "user.admin@retexspa.com", PasswordHash = "admin", UserName = "admin", Roles = listRoles.Where(x => x.Name == "OperatorAdmin").ToList() });
            emails.Add(new UserViewModel { Email = "user.archive@retexspa.com", PasswordHash = "archive", UserName = "archive", Roles = listRoles.Where(x => x.Name == "Archive").ToList() });
            emails.Add(new UserViewModel { Email = "user.control@retexspa.com", PasswordHash = "control", UserName = "control", Roles = listRoles.Where(x => x.Name == "Control").ToList() });
            //Utenti Operativi Reali
            emails.Add(new UserViewModel { Email = "", UserName = "magostena", PasswordHash = "magostena1", Roles = listRoles.Where(x => x.Name == "OperatorIT").ToList() });
            emails.Add(new UserViewModel { Email = "", UserName = "sbaffi", PasswordHash = "sbaffi2", Roles = listRoles.Where(x => x.Name == "OperatorIT").ToList() });
            emails.Add(new UserViewModel { Email = "", UserName = "imouchafi", PasswordHash = "imouchafi3", Roles = listRoles.Where(x => x.Name == "OperatorIT").ToList() });
            emails.Add(new UserViewModel { Email = "", UserName = "fpastore", PasswordHash = "fpastore4", Roles = listRoles.Where(x => x.Name == "OperatorAdmin").ToList() });
            emails.Add(new UserViewModel { Email = "", UserName = "fmassa", PasswordHash = "fmassa5", Roles = listRoles.Where(x => x.Name == "OperatorAdmin").ToList() });
            emails.Add(new UserViewModel { Email = "", UserName = "mstaiti", PasswordHash = "mstaiti6", Roles = listRoles.Where(x => x.Name == "OperatorIT").ToList() });


            foreach (UserViewModel element in emails)
            {
                //if (userManager.FindByEmailAsync(element.Email).Result == null && userManager.FindByNameAsync(element.UserName).Result==null)
                var t = userManager.FindByNameAsync(element.UserName).Result;
                if (userManager.FindByNameAsync(element.UserName).Result == null)

                {
                    User user = new User
                    {
                        UserName = element.UserName,
                        Email = element.Email,
                        PasswordHash = element.PasswordHash

                    };

                    IdentityResult result = userManager.CreateAsync(user, user.PasswordHash).Result;
                    if (result.Succeeded)
                    {
                        var userRole = new IdentityResult();
                        userRole = userManager.AddToRoleAsync(user, element.Roles.FirstOrDefault().Name).Result;
                        //if (user.Email.Contains("user.it@retexspa.com"))
                        //{
                        //    userRole = userManager.AddToRoleAsync(user, "OperatorIT").Result;
                        //}
                        //if (user.Email.Contains("user.admin@retexspa.com"))
                        //{
                        //    userRole = userManager.AddToRoleAsync(user, "OperatorAdmin").Result;
                        //}
                    }
                }
            }
        }
    }
}
