using BookStore.Data;
using BookStore.Models.Entities;
using BookStore.Models.ViewModels;
using BookStore.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class BookController : Controller
    {
        private readonly IBookService _bookService;
        private readonly ApplicationDbContext _context;   // Для выпадающих списков

        public BookController(IBookService bookService, ApplicationDbContext context)
        {
            _bookService = bookService;
            _context = context;
        }

        // GET: /Admin/Book
        // GET: /Admin/Book
        public async Task<IActionResult> Index(string? searchTerm)
        {
            var booksQuery = _context.Books
                .Include(b => b.Author)
                .Include(b => b.Category)
                .Include(b => b.Publisher)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower().Trim();

                booksQuery = booksQuery.Where(b =>
                    // По названию книги — поиск по любому слову
                    (b.Title != null && b.Title.ToLower().Contains(searchTerm)) ||

                    // По автору, издательству и категории — поиск с начала слова
                    (b.Author != null && b.Author.FullName.ToLower().StartsWith(searchTerm)) ||
                    (b.Publisher != null && b.Publisher.Name.ToLower().StartsWith(searchTerm)) ||
                    (b.Category != null && b.Category.Name.ToLower().StartsWith(searchTerm))
                );
            }

            var books = await booksQuery
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            ViewBag.SearchTerm = searchTerm;

            return View(books);
        }

        // GET: /Admin/Book/Create
        public async Task<IActionResult> Create()
        {
            await LoadDropdownsAsync();
            return View();
        }

        // POST: /Admin/Book/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    var fileName = Path.GetFileName(model.ImageFile.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/books", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(stream);
                    }

                    model.ImageUrl = "/images/books/" + fileName;   // ← Должна быть эта строка
                }

                await _bookService.AddBookAsync(model);
                TempData["Success"] = "✅ Книга успешно добавлена!";
                return RedirectToAction(nameof(Index));
            }

            await LoadDropdownsAsync();
            return View(model);
        }

        // GET: /Admin/Book/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _bookService.GetBookForEditAsync(id);
            if (model == null)
                return NotFound();

            await LoadDropdownsAsync();
            return View(model);
        }

        // POST: /Admin/Book/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BookViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    // загружаем новую картинку
                    var fileName = Path.GetFileName(model.ImageFile.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/books", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(stream);
                    }

                    model.ImageUrl = "/images/books/" + fileName;
                }
                else
                {
                    // ← Вот эта часть важна!
                    model.ImageUrl = model.CurrentImageUrl;
                }

                await _bookService.UpdateBookAsync(model);
                TempData["Success"] = "✅ Книга успешно обновлена!";
                return RedirectToAction(nameof(Index));
            }

            await LoadDropdownsAsync();
            return View(model);
        }

        // GET: /Admin/Book/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var model = await _bookService.GetBookForEditAsync(id);
            if (model == null)
                return NotFound();

            return View(model);
        }

        // POST: /Admin/Book/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _bookService.DeleteBookAsync(id);
            TempData["Success"] = "✅ Книга успешно удалена!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> AddAuthor([FromBody] AddAuthorDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.FullName))
                return BadRequest();

            var author = new Author { FullName = dto.FullName.Trim() };
            _context.Authors.Add(author);
            await _context.SaveChangesAsync();

            return Json(new { id = author.Id, fullName = author.FullName });
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory([FromBody] AddCategoryDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest();

            var category = new Category { Name = dto.Name.Trim() };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return Json(new { id = category.Id, name = category.Name });
        }

        [HttpPost]
        public async Task<IActionResult> AddPublisher([FromBody] AddPublisherDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest();

            var publisher = new Publisher { Name = dto.Name.Trim() };
            _context.Publishers.Add(publisher);
            await _context.SaveChangesAsync();

            return Json(new { id = publisher.Id, name = publisher.Name });
        }

        // Вспомогательные классы (можно положить в отдельный файл)
        public class AddAuthorDto { public string FullName { get; set; } }
        public class AddCategoryDto { public string Name { get; set; } }
        public class AddPublisherDto { public string Name { get; set; } }

        // ==================== ВСПОМОГАТЕЛЬНЫЙ МЕТОД ====================
        private async Task LoadDropdownsAsync()
        {
            ViewBag.Authors = await _context.Authors
                .OrderBy(a => a.FullName)                    // ← Сортировка по имени автора
                .Select(a => new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = a.FullName
                })
                .ToListAsync();

            ViewBag.Categories = await _context.Categories
                .OrderBy(c => c.Name)                        // ← Сортировка по названию категории
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })
                .ToListAsync();

            ViewBag.Publishers = await _context.Publishers
                .OrderBy(p => p.Name)                        // ← Сортировка по названию издательства
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Name
                })
                .ToListAsync();
        }
    }
}