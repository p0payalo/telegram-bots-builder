using Microsoft.EntityFrameworkCore;
using BotCreator.Core;
using BotCreator.Core.BotQueries;
using BotCreator.Core.BotResponses;

namespace WebTelegramBotsBuilder.Models
{
    public class MainContext : DbContext
    {
        public MainContext(DbContextOptions<MainContext> options): base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<BotMessageQuery>();
            builder.Entity<BotMessageResponse>();
            builder.Entity<TelegramBot>();
        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<TelegramBot> Bots { get; set; }
        public virtual DbSet<BotQuery> BotQueries { get; set; }
        public virtual DbSet<BotResponse> BotResponses { get; set; }
    }
}
