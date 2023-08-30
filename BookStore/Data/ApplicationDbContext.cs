using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BookStore.Models;

namespace BookStore.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<BookStore.Models.UserRole> UserRole { get; set; } = default!;
        public DbSet<BookStore.Models.Book> Book { get; set; } = default!;
        public DbSet<BookStore.Models.Category> Category { get; set; } = default!;
        public DbSet<BookStore.Models.Cart> Cart { get; set; } = default!;
        public DbSet<BookStore.Models.CartItem> CartItem { get; set; } = default!;
    }
}