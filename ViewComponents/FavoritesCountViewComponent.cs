using BookStore.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookStore.ViewComponents;

public class FavoritesCountViewComponent : ViewComponent
{
    private readonly IFavoriteService _favoriteService;

    public FavoritesCountViewComponent(IFavoriteService favoriteService)
    {
        _favoriteService = favoriteService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        if (!HttpContext.User.Identity?.IsAuthenticated == true)
        {
            return View(0);
        }

        var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
        {
            return View(0);
        }

        var count = (await _favoriteService.GetUserFavoritesAsync(userId)).Count;
        return View(count);
    }
}