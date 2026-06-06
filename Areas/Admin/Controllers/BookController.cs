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
        private readonly ApplicationDbContext _context;

        public BookController(IBookService bookService, ApplicationDbContext context)
        {
            _bookService = bookService;
            _context = context;
        }

        //список книг с возм. поиска
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
                    //поиск по названию книги (содержит слово)
                    (b.Title != null && b.Title.ToLower().Contains(searchTerm)) ||
                    //поиск по автору, издательству и категории (начинается со слова)
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

        //Admin/Book/Create (форма доб. нов. книги
        public async Task<IActionResult> Create()
        {
            await LoadDropdownsAsync();
            return View();
        }

        //созд. нов. книги + загр-ка обложки
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

                    model.ImageUrl = "/images/books/" + fileName;
                }

                await _bookService.AddBookAsync(model);
                TempData["Success"] = "✅ Книга успешно добавлена!";
                return RedirectToAction(nameof(Index));
            }

            await LoadDropdownsAsync();
            return View(model);
        }

        //форма ред. книги
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _bookService.GetBookForEditAsync(id);
            if (model == null)
                return NotFound();

            await LoadDropdownsAsync();
            return View(model);
        }

        //обнов. книги + загр-ка нов. обложки
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BookViewModel model)
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

                    model.ImageUrl = "/images/books/" + fileName;
                }
                else
                {
                    model.ImageUrl = model.CurrentImageUrl;
                }

                await _bookService.UpdateBookAsync(model);
                TempData["Success"] = "✅ Книга успешно обновлена!";
                return RedirectToAction(nameof(Index));
            }

            await LoadDropdownsAsync();
            return View(model);
        }

        //стр. подтв. удал. книги
        public async Task<IActionResult> Delete(int id)
        {
            var model = await _bookService.GetBookForEditAsync(id);
            if (model == null)
                return NotFound();

            return View(model);
        }

        //удал. книги
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _bookService.DeleteBookAsync(id);
            TempData["Success"] = "✅ Книга успешно удалена!";
            return RedirectToAction(nameof(Index));
        }

        //для динамического добавления

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

        public class AddAuthorDto { public string FullName { get; set; } }
        public class AddCategoryDto { public string Name { get; set; } }
        public class AddPublisherDto { public string Name { get; set; } }

        //данные для выпад. списков
        private async Task LoadDropdownsAsync()
        {
            ViewBag.Authors = await _context.Authors
                .OrderBy(a => a.FullName)
                .Select(a => new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = a.FullName
                })
                .ToListAsync();

            ViewBag.Categories = await _context.Categories
                .OrderBy(c => c.Name)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })
                .ToListAsync();

            ViewBag.Publishers = await _context.Publishers
                .OrderBy(p => p.Name)
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Name
                })
                .ToListAsync();
        }
    }
}