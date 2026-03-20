using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebLibrary.Data;
using WebLibrary.Models;

namespace WebLibrary.Controllers
{
    [Authorize]
    public class ReviewsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReviewsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Reviews (optional - list all)
        public async Task<IActionResult> Index()
        {
            var reviews = _context.Review
                .Include(r => r.Book)
                .Include(r => r.Member);

            return View(await reviews.ToListAsync());
        }

        // GET: Reviews/Create
        public IActionResult Create(int bookId)
        {
            ViewData["BookId"] = bookId;
            return View();
        }

        // POST: Reviews/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Review review)
        {
            if (ModelState.IsValid)
            {
                review.CreatedAt = DateTime.Now;

                _context.Add(review);
                await _context.SaveChangesAsync();

                return RedirectToAction("Details", "Books", new { id = review.BookId });
            }

            return View(review);
        }

        // GET: Reviews/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Review
                .Include(r => r.Book)
                .Include(r => r.Member)
                .FirstOrDefaultAsync(m => m.ReviewId == id);

            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }

        // POST: Reviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var review = await _context.Review.FindAsync(id);

            if (review != null)
            {
                _context.Review.Remove(review);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Details", "Books", new { id = review.BookId });
        }
    }
}
