using Microsoft.EntityFrameworkCore;
using Ticketus.Models;

namespace Ticketus.Data
{
    public class TicketDbContext : DbContext
    {
        public TicketDbContext(DbContextOptions<TicketDbContext> options) : base(options) { }

        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Status> Statuses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(u => u.Tickets)
                .WithOne(t => t.User)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Ticket>()
               .HasOne(t => t.Status)
               .WithMany()
               .HasForeignKey(t => t.StatusId)
               .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Status>().HasData(
                new Status { StatusId = 1, StatusName = "Open" },
                new Status { StatusId = 2, StatusName = "Closed" }
            );

            //modelBuilder.Entity<User>().HasData(
            //    new User { UserId = 1, UserName = "JohnDoe", Email = "john.doe@example.com", DateJoined = DateTime.Now },
            //    new User { UserId = 2, UserName = "JaneSmith", Email = "jane.smith@example.com", DateJoined = DateTime.Now }
            //);

            //modelBuilder.Entity<Ticket>().HasData(
            //    new Ticket { TicketId = 1, Description = "Fix homepage bug", DateCreated = DateTime.Now, StatusId = 1, UserId = 1 },
            //    new Ticket { TicketId = 2, Description = "Update contact page", DateCreated = DateTime.Now, StatusId = 2, UserId = 2 }
            //);

            base.OnModelCreating(modelBuilder);
        }
    }
}
