using Microsoft.EntityFrameworkCore;

namespace Bookx.Models
{
    public class BookxContext : DbContext
    {
        private const string EnvConnectionString = "ENV_CONNECTION_STRING";

        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Language> Languages { get; set; }

        private readonly string _dbConnectionString;

        public BookxContext()
        {
            _dbConnectionString = Environment.GetEnvironmentVariable(EnvConnectionString);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_dbConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>()
                .HasKey(b => b.Isbn);

            modelBuilder.Entity<Author>()
                .HasKey(a => a.Id);

            modelBuilder.Entity<Author>()
                .HasMany(a => a.Books)
                .WithMany(b => b.Authors);

            modelBuilder.Entity<Genre>()
                .HasKey(g => g.Id);

            modelBuilder.Entity<Genre>()
                .HasMany(g => g.Books)
                .WithMany(b => b.Genres);

            modelBuilder.Entity<Publisher>()
                .HasKey(p => p.Id);

            modelBuilder.Entity<Publisher>()
                .HasMany(p => p.Books)
                .WithOne(b => b.Publisher)
                .HasForeignKey(b => b.PublisherId)
                .IsRequired();

            modelBuilder.Entity<Language>()
                .HasKey(l => l.Id);

            modelBuilder.Entity<Language>()
                .HasMany(l => l.Books)
                .WithOne(b => b.Language)
                .HasForeignKey(b => b.LanguageId)
                .IsRequired();
        }
    }
}
