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

        //список всех авторов
        public async Task<IActionResult> Index()
        {
            var authors = await _context.Authors
                .OrderBy(a => a.FullName)
                .ToListAsync();

            return View(authors);
        }

        //форма доб. нов. автора
        public IActionResult Create()
        {
            return View();
        }

        //отправка формы созд. автора
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Author author)
        {
            if (ModelState.IsValid)
            {
                var exists = await _context.Authors
                    .AnyAsync(a => a.FullName.ToLower() == author.FullName.ToLower());

                if (exists)
                {
                    ModelState.AddModelError("FullName", "Автор с таким именем уже существует");
                    return View(author);
                }

                _context.Authors.Add(author);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Автор успешно добавлен";
                return RedirectToAction(nameof(Index));
            }

            return View(author);
        }

        [HttpPost]
        public async Task<IActionResult> CreateFromModal(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                return Json(new { success = false, message = "Имя автора не может быть пустым" });

            var exists = await _context.Authors
                .AnyAsync(a => a.FullName.ToLower() == fullName.Trim().ToLower());

            if (exists)
                return Json(new { success = false, message = "Автор с таким именем уже существует" });

            var author = new Author { FullName = fullName.Trim() };
            _context.Authors.Add(author);
            await _context.SaveChangesAsync();

            return Json(new { success = true, authorId = author.Id, fullName = author.FullName });
        }

        //форма ред. автора
        public async Task<IActionResult> Edit(int id)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author == null) return NotFound();

            return View(author);
        }

        //сохр. изм. автора
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

        //стр. подтвер. удал. автора
        public async Task<IActionResult> Delete(int id)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author == null) return NotFound();

            return View(author);
        }

        //удал. автора
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