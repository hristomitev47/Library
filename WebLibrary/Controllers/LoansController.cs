using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebLibrary.Data;
using WebLibrary.Models;

namespace LibraryWeb.Controllers
{
    public class LoansController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LoansController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Loans
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Loans
                .Include(l => l.Book)
                .Include(l => l.Member)
                .Include(l => l.Staff);

            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Loans/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loan = await _context.Loans
                .Include(l => l.Book)
                .Include(l => l.Member)
                .Include(l => l.Staff)
                .FirstOrDefaultAsync(m => m.LoanId == id);

            if (loan == null)
            {
                return NotFound();
            }

            return View(loan);
        }

        // GET: Loans/Create
        public IActionResult Create()
        {
            ViewData["BookId"] = new SelectList(_context.Books, "BookId", "Title");
            ViewData["MemberId"] = new SelectList(_context.Set<Member>(), "MemberId", "MemberName");
            ViewData["StaffId"] = new SelectList(_context.Set<Staff>(), "StaffId", "StaffName");

            return View();
        }

        // POST: Loans/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LoanId,MemberId,BookId,StaffId")] Loan loan)
        {
            if (ModelState.IsValid)
            {
                loan.LoanDate = DateTime.Now;
                loan.DueDate = DateTime.Now.AddDays(7);
                loan.LoanStatus = "Active";

                _context.Add(loan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["BookId"] = new SelectList(_context.Books, "BookId", "Title", loan.BookId);
            ViewData["MemberId"] = new SelectList(_context.Set<Member>(), "MemberId", "MemberName", loan.MemberId);
            ViewData["StaffId"] = new SelectList(_context.Set<Staff>(), "StaffId", "StaffName", loan.StaffId);

            return View(loan);
        }

        // GET: Loans/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loan = await _context.Loans.FindAsync(id);

            if (loan == null)
            {
                return NotFound();
            }

            ViewData["BookId"] = new SelectList(_context.Books, "BookId", "Title", loan.BookId);
            ViewData["MemberId"] = new SelectList(_context.Set<Member>(), "MemberId", "MemberName", loan.MemberId);
            ViewData["StaffId"] = new SelectList(_context.Set<Staff>(), "StaffId", "StaffName", loan.StaffId);

            return View(loan);
        }

        // POST: Loans/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LoanId,MemberId,BookId,StaffId,LoanDate,DueDate,ReturnDate,LoanStatus")] Loan loan)
        {
            if (id != loan.LoanId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(loan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LoanExists(loan.LoanId))
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

            ViewData["BookId"] = new SelectList(_context.Books, "BookId", "Title", loan.BookId);
            ViewData["MemberId"] = new SelectList(_context.Set<Member>(), "MemberId", "MemberName", loan.MemberId);
            ViewData["StaffId"] = new SelectList(_context.Set<Staff>(), "StaffId", "StaffName", loan.StaffId);

            return View(loan);
        }

        // GET: Loans/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loan = await _context.Loans
                .Include(l => l.Book)
                .Include(l => l.Member)
                .Include(l => l.Staff)
                .FirstOrDefaultAsync(m => m.LoanId == id);

            if (loan == null)
            {
                return NotFound();
            }

            return View(loan);
        }

        // POST: Loans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var loan = await _context.Loans.FindAsync(id);

            if (loan != null)
            {
                _context.Loans.Remove(loan);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LoanExists(int id)
        {
            return _context.Loans.Any(e => e.LoanId == id);
        }
    }
}