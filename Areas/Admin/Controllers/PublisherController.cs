using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookStore.Data;
using BookStore.Models.Entities;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PublisherController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PublisherController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var publishers = await _context.Publishers.OrderBy(p => p.Name).ToListAsync();
            return View(publishers);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Publisher publisher)
        {
            if (ModelState.IsValid)
            {
                _context.Publishers.Add(publisher);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Издательство успешно добавлено";
                return RedirectToAction(nameof(Index));
            }
            return View(publisher);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var publisher = await _context.Publishers.FindAsync(id);
            if (publisher == null) return NotFound();
            return View(publisher);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Publisher publisher)
        {
            if (id != publisher.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(publisher);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Издательство обновлено";
                return RedirectToAction(nameof(Index));
            }
            return View(publisher);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var publisher = await _context.Publishers.FindAsync(id);
            if (publisher == null) return NotFound();
            return View(publisher);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var publisher = await _context.Publishers.FindAsync(id);
            if (publisher != null)
            {
                _context.Publishers.Remove(publisher);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Издательство удалено";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}