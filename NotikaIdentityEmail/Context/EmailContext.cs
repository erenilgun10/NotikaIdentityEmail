using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NotikaIdentityEmail.Entities;

namespace NotikaIdentityEmail.Context
{
    public class EmailContext(DbContextOptions<EmailContext> options) : IdentityDbContext<AppUser>(options)
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Notification> Notifications { get; set; }
    }
}
