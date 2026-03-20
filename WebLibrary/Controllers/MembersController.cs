using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebLibrary.Data;
using WebLibrary.Models;

namespace LibraryWeb.Controllers
{
    [Authorize(Roles = "Staff")]
    public class MembersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MembersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Members
        public async Task<IActionResult> Index()
        {
            return View(await _context.Members.ToListAsync());
        }

        // GET: Members/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var member = await _context.Members
                .FirstOrDefaultAsync(m => m.MemberId == id);

            if (member == null) return NotFound();

            return View(member);
        }

        public IActionResult Create() => Forbid();

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult CreatePost() => Forbid();

        public IActionResult Edit(int? id) => Forbid();

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult EditPost() => Forbid();

        public IActionResult Delete(int? id) => Forbid();

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id) => Forbid();

        private bool MemberExists(int id)
        {
            return _context.Members.Any(e => e.MemberId == id);
        }
    }
}
