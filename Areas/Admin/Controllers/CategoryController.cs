using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookStore.Data;
using BookStore.Models.Entities;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        //список всех катег. (жанров)
        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories
                .OrderBy(c => c.Name)
                .ToListAsync();

            return View(categories);
        }

        //форма доб-ия нов. категории
        public IActionResult Create() => View();

        //создание нов. категории с пров. на дубл.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                var exists = await _context.Categories
                    .AnyAsync(c => c.Name.ToLower() == category.Name.ToLower());

                if (exists)
                {
                    ModelState.AddModelError("Name", "Такая категория уже существует");
                    return View(category);
                }

                _context.Categories.Add(category);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Категория успешно добавлена";
                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }

        //форма ред-ия категории
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();

            return View(category);
        }

        //сохр. измен. категории
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Category category)
        {
            if (id != category.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(category);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Категория обновлена";
                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }

        //стр. подтвержд. удал. кат.
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();

            return View(category);
        }

        //удал. категории
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Категория удалена";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}