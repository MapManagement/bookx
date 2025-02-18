using Microsoft.EntityFrameworkCore;

namespace Bookx.Models
{
    public class BookxContext : DbContext
    {
        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Language> Languages { get; set; }

        private readonly string _dbConnectionString;

        public BookxContext()
        {
            _dbConnectionString = "";
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("<connection string>");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Author>()
                .HasMany(a => a.Books)
                .WithMany(b => b.Authors);

            modelBuilder.Entity<Genre>()
                .HasMany(g => g.Books)
                .WithMany(b => b.Genres);

            modelBuilder.Entity<Publisher>()
                .HasMany(p => p.Books)
                .WithOne(b => b.Publisher)
                .HasForeignKey(b => b.PublisherId)
                .IsRequired();

            modelBuilder.Entity<Language>()
                .HasMany(l => l.Books)
                .WithOne(b => b.Language)
                .HasForeignKey(b => b.LanguageId)
                .IsRequired();
        }
    }
}
