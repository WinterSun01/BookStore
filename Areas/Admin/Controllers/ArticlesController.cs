using BookStore.Data;
using BookStore.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class ArticlesController : Controller
{
    private readonly ApplicationDbContext _context;

    public ArticlesController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var articles = await _context.Articles
            .OrderByDescending(a => a.PublishedAt)
            .ToListAsync();

        return View(articles);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Books = await _context.Books
            .OrderBy(b => b.Title)
            .ToListAsync();

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Article article, int[] selectedBooks)
    {
        if (ModelState.IsValid)
        {
            if (selectedBooks != null && selectedBooks.Any())
            {
                article.Books = await _context.Books
                    .Where(b => selectedBooks.Contains(b.Id))
                    .ToListAsync();
            }

            article.CreatedAt = DateTime.UtcNow;
            article.Slug = GenerateSlug(article.Title);

            _context.Articles.Add(article);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        ViewBag.Books = await _context.Books.OrderBy(b => b.Title).ToListAsync();
        return View(article);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var article = await _context.Articles
            .Include(a => a.Books)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (article == null) return NotFound();

        ViewBag.Books = await _context.Books.OrderBy(b => b.Title).ToListAsync();
        return View(article);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Article article, int[] selectedBooks)
    {
        if (id != article.Id) return NotFound();

        if (ModelState.IsValid)
        {
            var existingArticle = await _context.Articles
                .Include(a => a.Books)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (existingArticle == null) return NotFound();

            existingArticle.Title = article.Title;
            existingArticle.ShortDescription = article.ShortDescription;
            existingArticle.Content = article.Content;
            existingArticle.ImageUrl = article.ImageUrl;
            existingArticle.IsPublished = article.IsPublished;
            existingArticle.PublishedAt = article.PublishedAt;
            existingArticle.UpdatedAt = DateTime.UtcNow;

            existingArticle.Books.Clear();
            if (selectedBooks != null && selectedBooks.Any())
            {
                var books = await _context.Books
                    .Where(b => selectedBooks.Contains(b.Id))
                    .ToListAsync();
                foreach (var book in books)
                {
                    existingArticle.Books.Add(book);
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Books = await _context.Books.OrderBy(b => b.Title).ToListAsync();
        return View(article);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var article = await _context.Articles.FindAsync(id);
        if (article != null)
        {
            _context.Articles.Remove(article);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    private string GenerateSlug(string title)
    {
        return title.ToLower()
            .Replace(" ", "-")
            .Replace(".", "")
            .Replace(",", "");
    }
}