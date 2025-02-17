using System.ComponentModel.DataAnnotations;

namespace Bookx.Models
{
    public class Author
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public DateTime Birthdate { get; set; }
    }

    public class Book
    {
        [Key]
        public string Isbn { get; set; }
        [Required]
        public string Title { get; set; }
        public string CoverPath { get; set; }
        [Required]
        public int NumerOfPages { get; set; }
        public string ShopLink { get; set; }
        public string Blurb { get; set; }
        [Required]
        public Author Author { get; set; }
        [Required]
        public Language Language { get; set; }
        [Required]
        public Publisher Publisher { get; set; }
        public List<Genre> Genres { get; set; }
    }

    public class Publisher
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
    }

    public class Genre
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
    }

    public class Language
    {
        [Key]
        public int id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
