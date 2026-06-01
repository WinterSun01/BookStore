using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookStore.Data;
using BookStore.Models.Entities;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AuthorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuthorController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Admin/Author
        public async Task<IActionResult> Index()
        {
            var authors = await _context.Authors
                .OrderBy(a => a.FullName)
                .ToListAsync();

            return View(authors);
        }

        // GET: /Admin/Author/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Admin/Author/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Author author)
        {
            if (ModelState.IsValid)
            {
                _context.Authors.Add(author);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Автор успешно добавлен";
                return RedirectToAction(nameof(Index));
            }
            return View(author);
        }

        // GET: /Admin/Author/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author == null) return NotFound();

            return View(author);
        }

        // POST: /Admin/Author/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Author author)
        {
            if (id != author.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(author);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Автор успешно обновлён";
                return RedirectToAction(nameof(Index));
            }
            return View(author);
        }

        // GET: /Admin/Author/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author == null) return NotFound();

            return View(author);
        }

        // POST: /Admin/Author/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author != null)
            {
                _context.Authors.Remove(author);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Автор удалён";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}