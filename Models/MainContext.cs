using Microsoft.EntityFrameworkCore;
using BotCreator.Core;
using BotCreator.Core.BotQueries;

namespace WebTelegramBotsBuilder.Models
{
    public class MainContext : DbContext
    {
        public MainContext(DbContextOptions<MainContext> options): base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<TelegramBot>();
        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<TelegramBot> Bots { get; set; }
        public virtual DbSet<BotQuery> BotQueries { get; set; }
        public virtual DbSet<BotResponse> BotResponses { get; set; }
    }
}
