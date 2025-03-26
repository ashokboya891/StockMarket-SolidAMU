using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ServiceContracts.DTO;
using System;
using System.Threading.Tasks;

namespace Entities
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
         public ApplicationDbContext(DbContextOptions options) : base(options)
         {
         }

        public DbSet<BuyOrder> BuyOrders { get; set; }
        public DbSet<SellOrder> SellOrders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
               base.OnModelCreating(modelBuilder);

               modelBuilder.Entity<BuyOrder>().ToTable("BuyOrders");
              modelBuilder.Entity<SellOrder>().ToTable("SellOrders");

            modelBuilder.Entity<BuyOrder>()
       .ToTable("BuyOrders")
       .HasOne<ApplicationUser>()
       .WithMany()
       .HasForeignKey(b => b.UserID)
       .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SellOrder>()
                .ToTable("SellOrders")
                .HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(s => s.UserID)
                .OnDelete(DeleteBehavior.Cascade);

        }
 }
}

