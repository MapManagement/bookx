using Microsoft.EntityFrameworkCore;

namespace Bookx.Models
{
    public class BookxContext : DbContext
    {
        #region Fields

        private const string EnvDbConnectionString = "DB_CONNECTION_STRING";
        private readonly string _dbConnectionString;

        #endregion

        #region Properties

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

        #endregion

        #region Constructor

        public BookxContext()
        {
            _dbConnectionString = Environment.GetEnvironmentVariable(EnvDbConnectionString);
        }

        #endregion

        #region Overrides

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
                .Property(a => a.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Author>()
                .HasMany(a => a.Books)
                .WithMany(b => b.Authors);

            modelBuilder.Entity<Genre>()
                .HasKey(g => g.Id);

            modelBuilder.Entity<Genre>()
                .Property(g => g.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Genre>()
                .HasMany(g => g.Books)
                .WithMany(b => b.Genres);

            modelBuilder.Entity<Publisher>()
                .HasKey(p => p.Id);

            modelBuilder.Entity<Publisher>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Publisher>()
                .HasMany(p => p.Books)
                .WithOne(b => b.Publisher)
                .HasForeignKey(b => b.PublisherId)
                .IsRequired();

            modelBuilder.Entity<Language>()
                .HasKey(l => l.Id);

            modelBuilder.Entity<Language>()
                .Property(l => l.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Language>()
                .HasMany(l => l.Books)
                .WithOne(b => b.Language)
                .HasForeignKey(b => b.LanguageId)
                .IsRequired();

            modelBuilder.Entity<User>()
                .HasKey(u => u.Id);

            modelBuilder.Entity<User>()
                .Property(u => u.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.MailAddress)
                .IsUnique();

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

            modelBuilder.Entity<Tag>()
                .Property(t => t.Id)
                .ValueGeneratedOnAdd();
        }

        #endregion
    }
}
