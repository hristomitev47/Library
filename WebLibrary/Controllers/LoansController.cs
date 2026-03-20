using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebLibrary.Data;
using WebLibrary.Models;

namespace LibraryWeb.Controllers
{
    [Authorize]
    public class LoansController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public LoansController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Loans - Staff sees all loans with filter, Member sees only their own
        public async Task<IActionResult> Index(string filter)
        {
            var loans = _context.Loans
                .Include(l => l.Book)
                .Include(l => l.Member)
                .Include(l => l.Staff)
                .AsQueryable();

            if (User.IsInRole("Member"))
            {
                // Member only sees their own loans
                var user = await _userManager.GetUserAsync(User);
                var member = await _context.Members
                    .FirstOrDefaultAsync(m => m.Email == user.Email);
                if (member != null)
                    loans = loans.Where(l => l.MemberId == member.MemberId);
            }
            else
            {
                // Staff can filter
                if (filter == "Active")
                    loans = loans.Where(l => l.LoanStatus == "Active");
                else if (filter == "Returned")
                    loans = loans.Where(l => l.LoanStatus == "Returned");
                else if (filter == "Overdue")
                    loans = loans.Where(l => l.LoanStatus == "Active" && l.DueDate < DateTime.Now);

                ViewData["CurrentFilter"] = filter;
            }

            return View(await loans.ToListAsync());
        }

        // GET: Loans/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var loan = await _context.Loans
                .Include(l => l.Book)
                .Include(l => l.Member)
                .Include(l => l.Staff)
                .FirstOrDefaultAsync(m => m.LoanId == id);

            if (loan == null) return NotFound();

            return View(loan);
        }

        // GET: Loans/Create - Members only
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> Create(int? bookId)
        {
            if (bookId == null) return RedirectToAction("Index", "Books");

            var user = await _userManager.GetUserAsync(User);
            var member = await _context.Members
                .FirstOrDefaultAsync(m => m.Email == user.Email);

            if (member == null) return RedirectToAction("Index", "Books");

            var book = await _context.Books.FindAsync(bookId);
            if (book == null || book.CopiesTotal <= 0)
                return RedirectToAction("Index", "Books");

            var loan = new Loan
            {
                MemberId = member.MemberId,
                BookId = book.BookId,
                LoanDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(14),
                LoanStatus = "Active"
            };

            book.CopiesTotal -= 1;
            _context.Update(book);
            _context.Add(loan);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Books");
        }

        // POST: Loans/Create - Members only
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> Create([Bind("MemberId,BookId")] Loan loan)
        {
            ModelState.Remove("LoanStatus");
            ModelState.Remove("LoanDate");
            ModelState.Remove("DueDate");
            var book = await _context.Books.FindAsync(loan.BookId);

            if (book == null || book.CopiesTotal <= 0)
            {
                ModelState.AddModelError("", "This book is not available.");
            }

            if (ModelState.IsValid)
            {
                loan.LoanDate = DateTime.Now;
                loan.DueDate = DateTime.Now.AddDays(14);
                loan.LoanStatus = "Active";

                // Decrease available copies
                book.CopiesTotal -= 1;
                _context.Update(book);

                _context.Add(loan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["BookId"] = new SelectList(_context.Books.Where(b => b.CopiesTotal > 0), "BookId", "Title", loan.BookId);
            ViewData["MemberId"] = new SelectList(_context.Set<Member>(), "MemberId", "FirstName", loan.MemberId);
            return View(loan);
        }

        // GET: Loans/Return/5 - Staff marks a loan as returned
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> Return(int? id)
        {
            if (id == null) return NotFound();

            var loan = await _context.Loans
                .Include(l => l.Book)
                .Include(l => l.Member)
                .FirstOrDefaultAsync(l => l.LoanId == id);

            if (loan == null) return NotFound();

            return View(loan);
        }

        // POST: Loans/Return/5 - Staff marks a loan as returned
        [HttpPost, ActionName("Return")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> ReturnConfirmed(int id)
        {
            var loan = await _context.Loans
                .Include(l => l.Book)
                .FirstOrDefaultAsync(l => l.LoanId == id);

            if (loan != null)
            {
                loan.ReturnDate = DateTime.Now;
                loan.LoanStatus = "Returned";

                // Give the copy back
                if (loan.Book != null)
                    loan.Book.CopiesTotal += 1;

                _context.Update(loan);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // DELETE: Staff only
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var loan = await _context.Loans
                .Include(l => l.Book)
                .Include(l => l.Member)
                .FirstOrDefaultAsync(m => m.LoanId == id);

            if (loan == null) return NotFound();

            return View(loan);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var loan = await _context.Loans.FindAsync(id);
            if (loan != null)
                _context.Loans.Remove(loan);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LoanExists(int id)
        {
            return _context.Loans.Any(e => e.LoanId == id);
        }
    }
}
