using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EfCoreFulltextSearchOwnedTypeBug
{
    public partial class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseLoggerFactory(LoggerFactory.Create(x => x
                    .AddConsole()
                    .AddFilter(y => y >= LogLevel.Debug)))
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors();

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Product>(entityBuilder =>
                {
                    entityBuilder.HasKey(x => x.Id);
                    entityBuilder.Property(x => x.Id).ValueGeneratedOnAdd();
                    entityBuilder.HasIndex(x => x.Id).IsUnique().HasName("PK_Products_Id");
                    entityBuilder.OwnsOne(x => x.Lookup);
                });

            base.OnModelCreating(modelBuilder);
        }
    }
}