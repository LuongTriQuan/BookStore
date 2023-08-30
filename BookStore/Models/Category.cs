using System.ComponentModel.DataAnnotations;

namespace BookStore.Models
{
    public class Category
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        public ICollection<Book>? Book { get; set; }
    }
}
