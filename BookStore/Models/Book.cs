using System.ComponentModel.DataAnnotations;

namespace BookStore.Models
{
    public class Book
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string? Name { get; set; }

        [Required]
        public Category? Category { get; set; }
        [Required]
        public decimal Price { get; set; }
        public ICollection<CartItem> CartItem { get; set; } = new List<CartItem>();
    }
}
