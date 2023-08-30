using System.ComponentModel.DataAnnotations;

namespace BookStore.Models
{
    public class CartItem
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public Cart? Cart { get; set; }

        [Required]
        public Book? Book { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal TotalPrice { get; set; }
    }
}
