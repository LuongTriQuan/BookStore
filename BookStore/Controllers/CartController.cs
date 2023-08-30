using BookStore.Data;
using BookStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace BookStore.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _contextAccessor;

        public CartController(ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _contextAccessor = contextAccessor;
        }
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var userId = GetUserId();
            if (userId == null)
                throw new Exception("please login");
            var cart = await _context.Cart
                                    .Include(a => a.CartItem)
                                    .ThenInclude(a => a.Book)
                                    .ThenInclude(a => a.Category)
                                    .Where(a => a.User.Id == userId && a.IsDeleted == false).FirstOrDefaultAsync();
            if (cart == null)
            {
                cart = new Cart()
                {
                    User = _context.Users.FirstOrDefault(x => x.Id == GetUserId())
                };
                await _context.Cart.AddAsync(cart);
                await _context.SaveChangesAsync();
            }
            return View(cart);
        }
        [Authorize]
        public async Task<IActionResult> AddItem(int BookId, int quantity = 1, int redirect = 0)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                if (string.IsNullOrEmpty(GetUserId()))
                {
                    throw new Exception("user not found");
                }
                else
                {
                    var cart = GetCart(GetUserId());
                    if (cart == null)
                    {
                        cart = new Cart()
                        {
                            User = _context.Users.FirstOrDefault(x => x.Id == GetUserId())
                        };
                        _context.Cart.Add(cart);
                    }
                    _context.SaveChanges();
                    var cartItem = _context.CartItem.FirstOrDefault(x => x.Cart.Id == cart.Id && x.Book.Id == BookId);
                    if (cartItem != null)
                    {
                        cartItem.Quantity += quantity;
                        cartItem.TotalPrice += (quantity * _context.Book.FirstOrDefault(x => x.Id == BookId)!.Price);
                    }
                    else
                    {
                        cartItem = new CartItem()
                        {
                            Cart = _context.Cart.FirstOrDefault(x => x.Id == cart.Id),
                            Book = _context.Book.FirstOrDefault(x => x.Id == BookId),
                            Quantity = quantity,
                            TotalPrice = quantity * _context.Book.FirstOrDefault(x => x.Id == BookId)!.Price
                        };
                        _context.CartItem.Add(cartItem);
                    }
                    _context.SaveChanges();
                    transaction.Commit();
                }
            }
            catch (Exception e)
            {
                throw;
            }

            return RedirectToAction("Index");
        }
        [Authorize]
        public IActionResult RemoveItem(int BookId)
        {
            try
            {
                if (string.IsNullOrEmpty(GetUserId()))
                {
                    throw new Exception("user not found");
                }
                else
                {
                    var cart = GetCart(GetUserId());
                    if (cart == null)
                    {
                        throw new Exception("cart not found");
                    }
                    var cartItem = _context.CartItem.FirstOrDefault(x => x.Cart.Id == cart.Id && x.Book.Id == BookId);
                    if (cartItem == null)
                    {
                        throw new Exception("cart don't have item");
                    }
                    else if (cartItem.Quantity == 1)
                    {
                        _context.CartItem.Remove(cartItem);
                    }
                    else
                    {
                        cartItem.Quantity--;
                        cartItem.TotalPrice -= _context.Book.FirstOrDefault(x => x.Id == BookId)!.Price;
                    }
                    _context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw;
            }
            return RedirectToAction("Index");
        }
        [Authorize]
        public IActionResult DeleteItem(int BookId)
        {
            try
            {
                if (string.IsNullOrEmpty(GetUserId()))
                {
                    throw new Exception("user not found");
                }
                else
                {
                    var cart = GetCart(GetUserId());
                    if (cart == null)
                    {
                        throw new Exception("cart not found");
                    }
                    var cartItem = _context.CartItem.FirstOrDefault(x => x.Cart.Id == cart.Id && x.Book.Id == BookId);
                    if (cartItem == null)
                    {
                        throw new Exception("cart doesn't have item");
                    }
                    else
                    {
                        _context.CartItem.Remove(cartItem);
                    }
                    _context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw;
            }
            return RedirectToAction("Index");
        }

        [Authorize]
        public IActionResult GetTotalCartItem()
        { 
            var userID = GetUserId();
            var data = (from Cart in _context.Cart.Where(x => x.IsDeleted == false && x.User.Id == userID)
                         join CartItem in _context.CartItem
                         on Cart.Id equals CartItem.Cart.Id
                         select new { CartItem.Id }).ToList();
            int count = data.Count();
            return Ok(count);
        }
        [Authorize]
        public IActionResult DeleteCart()
        {
            var userId = GetUserId();
            var cart = GetCart(userId);
            cart.IsDeleted = true;
            _context.SaveChanges();
            return Redirect("/Home/index");
        }

        public Cart GetCart(string userId)
        {
            var cart = _context.Cart.Where(x => x.User.Id == userId).ToList().Where(x => x.IsDeleted == false).FirstOrDefault();
            return cart;
        }

        private string GetUserId()
        {
            var user = _contextAccessor.HttpContext.User;
            string userId = _userManager.GetUserId(user);
            return userId;
        }
    }
}
