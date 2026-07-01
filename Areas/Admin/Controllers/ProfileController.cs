using BookStore.Data;
using BookStore.Models;
using BookStore.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BookStore.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IFavoriteService _favoriteService;

        public ProfileController(ApplicationDbContext context, IFavoriteService favoriteService)
        {
            _context = context;
            _favoriteService = favoriteService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await _context.Users.FindAsync(userId);

            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Book)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            ViewBag.TotalOrders = orders.Count;
            ViewBag.TotalSpent = orders.Sum(o => o.TotalAmount);

            int favoritesCount = 0;
            if (userId != null)
            {
                favoritesCount = (await _favoriteService.GetUserFavoritesAsync(userId)).Count;
            }
            ViewBag.FavoritesCount = favoritesCount;

            var model = new UserProfileViewModel
            {
                User = user,
                Orders = orders
            };

            return View(model);
        }

        public async Task<IActionResult> OrderDetails(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var order = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Book)
                        .ThenInclude(b => b.Author)
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

            if (order == null) return NotFound();

            return View(order);
        }

        public async Task<IActionResult> Edit()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _context.Users.FindAsync(userId);

            if (user == null) return NotFound();

            var model = new EditProfileViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                MiddleName = user.MiddleName,
                BirthDate = user.BirthDate,
                Phone = user.PhoneNumber
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditProfileViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _context.Users.FindAsync(userId);

            if (user == null) return NotFound();

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.MiddleName = model.MiddleName;
            user.BirthDate = model.BirthDate;
            user.PhoneNumber = model.Phone;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Профиль успешно обновлён ✓";
            return RedirectToAction(nameof(Index));
        }
    }
}