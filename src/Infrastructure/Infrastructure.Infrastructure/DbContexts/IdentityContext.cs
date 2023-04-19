using Application.Interfaces.Shared;
using Domain.Entities;
using Infrastructure.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Yanga.Module.EntityFrameworkCore.AuditTrail.Enums;
using Yanga.Module.EntityFrameworkCore.AuditTrail.Models;

namespace Infrastructure.Infrastructure.DbContexts
{
    public class IdentityContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        private readonly ObservableCollection<ApplicationUser> _items;
        private readonly IAuthenticatedUserService _authenticatedUser;
        public IdentityContext(DbContextOptions<IdentityContext> options, IAuthenticatedUserService authenticatedUser) : base(options)
        {
            _authenticatedUser = authenticatedUser;
        }
   
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasDefaultSchema("dbo");
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable(name: "Users");
                // entity.ToTable("Product", "dbo").HasMany(p => p.Products)
                //   .WithOne(d => d.ApplicationUser).HasForeignKey(d => d.IdUser).OnDelete(deleteBehavior: DeleteBehavior.NoAction);


            });

           // builder.Entity<IdentityRole>(entity =>
            builder.Entity<ApplicationRole>(entity =>
            {
                entity.ToTable(name: "Roles");
            });
            builder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.ToTable("UserRoles");
            });

            builder.Entity<IdentityUserClaim<string>>(entity =>
            {
                entity.ToTable("UserClaims");
            });

            builder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.ToTable("UserLogins");
            });

            builder.Entity<IdentityRoleClaim<string>>(entity =>
            {
                entity.ToTable("RoleClaims");
            });

            builder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.ToTable("UserTokens");
            });
              builder.Entity<ApplicationRole>(b =>
                {
                    // Each User can have many UserClaims
                    b.HasIndex(p => new { p.VKey }).IsUnique();
                });
        }
    }
   
}
