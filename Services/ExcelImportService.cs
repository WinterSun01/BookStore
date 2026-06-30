using BookStore.Data;
using BookStore.Models.DTOs;
using BookStore.Models.Entities;
using BookStore.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace BookStore.Services;

public class ExcelImportService
{
    private readonly ApplicationDbContext _context;
    private readonly IBookRepository _bookRepository;

    public ExcelImportService(ApplicationDbContext context, IBookRepository bookRepository)
    {
        _context = context;
        _bookRepository = bookRepository;
    }

    public async Task<ImportResult> ImportBooksAsync(Stream fileStream)
    {
        ExcelPackage.License.SetNonCommercialPersonal("Учебный проект");

        var result = new ImportResult();

        using var package = new ExcelPackage(fileStream);
        var worksheet = package.Workbook.Worksheets[0];
        var rowCount = worksheet.Dimension?.Rows ?? 0;

        result.TotalRows = rowCount - 1;

        for (int row = 2; row <= rowCount; row++)
        {
            try
            {
                var title = worksheet.Cells[row, 1].Value?.ToString()?.Trim();
                if (string.IsNullOrWhiteSpace(title))
                {
                    result.Skipped++;
                    continue;
                }

                if (await _context.Books.AnyAsync(b => b.Title == title))
                {
                    result.Skipped++;
                    continue;
                }

                var description = worksheet.Cells[row, 2].Value?.ToString()?.Trim() ?? "Описание отсутствует";
                var price = decimal.TryParse(worksheet.Cells[row, 3].Value?.ToString(), out var p) ? p : 500;
                var stock = int.TryParse(worksheet.Cells[row, 4].Value?.ToString(), out var s) ? s : 10;
                var authorName = worksheet.Cells[row, 5].Value?.ToString()?.Trim() ?? "Неизвестный автор";
                var categoryName = worksheet.Cells[row, 6].Value?.ToString()?.Trim() ?? "Без категории";
                var publisherName = worksheet.Cells[row, 7].Value?.ToString()?.Trim() ?? "Неизвестное издательство";
                var imageUrl = worksheet.Cells[row, 8].Value?.ToString()?.Trim();

                var author = await GetOrCreateAuthorAsync(authorName);
                var category = await GetOrCreateCategoryAsync(categoryName);
                var publisher = await GetOrCreatePublisherAsync(publisherName);

                var book = new Book
                {
                    Title = title,
                    Description = description,
                    Price = price,
                    Stock = stock,
                    AuthorId = author.Id,
                    CategoryId = category.Id,
                    PublisherId = publisher.Id,
                    ImageUrl = string.IsNullOrWhiteSpace(imageUrl) ? null : imageUrl,
                    CreatedAt = DateTime.UtcNow
                };

                await _bookRepository.AddAsync(book);
                result.Added++;
            }
            catch (Exception ex)
            {
                result.Errors.Add($"Строка {row}: {ex.Message}");
            }
        }

        if (result.Added > 0)
            await _context.SaveChangesAsync();

        return result;
    }

    private async Task<Author> GetOrCreateAuthorAsync(string name)
    {
        if (name.Length > 120) name = name.Substring(0, 120).Trim();

        var author = await _context.Authors.FirstOrDefaultAsync(a => a.FullName == name);
        if (author == null)
        {
            author = new Author { FullName = name };
            _context.Authors.Add(author);
            await _context.SaveChangesAsync();
        }
        return author;
    }

    private async Task<Category> GetOrCreateCategoryAsync(string name)
    {
        if (name.Length > 80) name = name.Substring(0, 80).Trim();

        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Name == name);
        if (category == null)
        {
            category = new Category { Name = name };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
        }
        return category;
    }

    private async Task<Publisher> GetOrCreatePublisherAsync(string name)
    {
        if (name.Length > 80) name = name.Substring(0, 80).Trim();

        var publisher = await _context.Publishers.FirstOrDefaultAsync(p => p.Name == name);
        if (publisher == null)
        {
            publisher = new Publisher { Name = name };
            _context.Publishers.Add(publisher);
            await _context.SaveChangesAsync();
        }
        return publisher;
    }
}