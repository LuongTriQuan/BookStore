using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
namespace BookStore.Models
{
    public class Cart
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public IdentityUser? User { get; set; }

        public bool IsDeleted { get; set; } = false;

        public virtual ICollection<CartItem> CartItem { get; set; } = new List<CartItem>();
    }
}
