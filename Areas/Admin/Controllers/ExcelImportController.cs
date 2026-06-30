using BookStore.Data;
using BookStore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace BookStore.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class ExcelImportController : Controller
{
    private readonly ExcelImportService _importService;
    private readonly ApplicationDbContext _context;

    public ExcelImportController(ExcelImportService importService, ApplicationDbContext context)
    {
        _importService = importService;
        _context = context;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Index(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            TempData["Error"] = "Пожалуйста, выберите файл Excel";
            return View();
        }

        if (!file.FileName.EndsWith(".xlsx"))
        {
            TempData["Error"] = "Поддерживаются только файлы .xlsx";
            return View();
        }

        try
        {
            using var stream = file.OpenReadStream();
            var result = await _importService.ImportBooksAsync(stream);

            TempData["Success"] = $"Импорт завершён! Добавлено: {result.Added}, Пропущено: {result.Skipped}";
            return View(result);
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Ошибка при импорте: {ex.Message}";
            return View();
        }
    }

    [HttpGet]
    [HttpGet]
    public IActionResult DownloadTemplate()
    {
        ExcelPackage.License.SetNonCommercialPersonal("Учебный проект");

        using var package = new OfficeOpenXml.ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Books");

        var headers = new[]
        {
        "Title", "Description", "Price", "Stock",
        "AuthorName", "CategoryName", "PublisherName", "ImageUrl"
    };

        for (int i = 0; i < headers.Length; i++)
        {
            worksheet.Cells[1, i + 1].Value = headers[i];
            worksheet.Cells[1, i + 1].Style.Font.Bold = true;
        }

        var books = _context.Books
            .Include(b => b.Author)
            .Include(b => b.Category)
            .Include(b => b.Publisher)
            .OrderBy(b => b.Title)
            .ToList();

        int row = 2;
        foreach (var book in books)
        {
            worksheet.Cells[row, 1].Value = book.Title;
            worksheet.Cells[row, 2].Value = book.Description;
            worksheet.Cells[row, 3].Value = book.Price;
            worksheet.Cells[row, 4].Value = book.Stock;
            worksheet.Cells[row, 5].Value = book.Author?.FullName ?? "";
            worksheet.Cells[row, 6].Value = book.Category?.Name ?? "";
            worksheet.Cells[row, 7].Value = book.Publisher?.Name ?? "";
            worksheet.Cells[row, 8].Value = book.ImageUrl ?? "";
            row++;
        }

        worksheet.Cells.AutoFitColumns();

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;

        return File(stream,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "BookStore_Export.xlsx");
    }
}

