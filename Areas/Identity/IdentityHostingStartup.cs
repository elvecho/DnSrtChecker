﻿using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(DnSrtChecker.Areas.Identity.IdentityHostingStartup))]
namespace DnSrtChecker.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
               /* services.AddDbContext<DnSrtCheckerDbContext>(options =>
                  options.UseSqlServer(
                      context.Configuration.GetConnectionString("DefaultConnection")));*/
            });
        }
    }
}