using Microsoft.EntityFrameworkCore;

namespace Bookx.Models
{
    public class BookxContext : DbContext
    {
        private const string EnvConnectionString = "ENV_CONNECTION_STRING";

        // book related
        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Language> Languages { get; set; }

        // user related
        public DbSet<User> Users { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<OwnedBook> OwnedBooks { get; set; }

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

            // TODO: how to distinguish login (username, mail...)?
            modelBuilder.Entity<User>()
                .HasKey(u => u.Id);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Books)
                .WithMany(b => b.Users)
                .UsingEntity<OwnedBook>(
                    u => u.HasOne<Book>().WithMany(b => b.Owners).HasForeignKey(o => o.BookIsbn),
                    b => b.HasOne<User>().WithMany(u => u.OwnedBooks).HasForeignKey(o => o.UserId)
                );

            modelBuilder.Entity<User>()
                .HasMany(u => u.Tags)
                .WithOne(t => t.User)
                .HasForeignKey(t => t.UserId);

            modelBuilder.Entity<Tag>()
                .HasKey(t => t.Id);
        }
    }
}
