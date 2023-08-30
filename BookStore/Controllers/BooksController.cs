using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookStore.Data;
using BookStore.Models;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace BookStore.Controllers
{
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BooksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Books
        /* public async Task<IActionResult> Index()
        {
            return _context.Book != null ?
                        View(await _context.Book.ToListAsync()) :
                        Problem("Entity set 'ApplicationDbContext.Book'  is null.");
        } */
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var books = await _context.Book
                .Include(b => b.Category)
                .ToListAsync();

            return View(books);
        }

        // GET: Books/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Book == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .Include(b => b.Category)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Books/Create
        [Authorize(Roles = "Admin")]

        public IActionResult Create()
        {
            ViewBag.Categories = _context.Category
                .Select(c => c.Name)
                .ToList();
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Price,Category")] Book book)
        {
            if (ModelState.IsValid)
            {
                // Retrieve the selected category by name
                var selectedCategory = _context.Category.FirstOrDefault(c => c.Name == book.Category.Name);

                // Assign the retrieved category to the book
                book.Category = selectedCategory;

                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(book);
        }

        // GET: Books/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .Include(b => b.Category) // Include the related category
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
            {
                return NotFound();
            }

            // Populate the dropdown list with category names
            ViewBag.Categories = _context.Category
                .Select(c => c.Name)
                .ToList();

            return View(book);
        }


        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,Category")] Book book)
        {
            if (id != book.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Retrieve the selected category by name
                    var selectedCategory = _context.Category.FirstOrDefault(c => c.Name == book.Category.Name);

                    // Update the book's category based on the selected category name
                    book.Category = selectedCategory;

                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        // GET: Books/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Book == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Book == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Book'  is null.");
            }
            var book = await _context.Book.FindAsync(id);
            if (book != null)
            {
                _context.Book.Remove(book);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Admin")]
        private bool BookExists(int id)
        {
            return (_context.Book?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
