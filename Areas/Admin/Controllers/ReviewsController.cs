using BookStore.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ReviewsController : Controller
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        public async Task<IActionResult> Index()
        {
            var pendingReviews = await _reviewService.GetPendingReviewsAsync();
            return View(pendingReviews);
        }

        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            await _reviewService.ApproveReviewAsync(id);
            TempData["Success"] = "Отзыв успешно опубликован.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _reviewService.DeleteReviewAsync(id);
            TempData["Success"] = "Отзыв удалён.";
            return RedirectToAction("Index");
        }
    }
}