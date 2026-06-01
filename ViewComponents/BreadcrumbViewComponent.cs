using Microsoft.AspNetCore.Mvc;

namespace BookStore.ViewComponents
{
    public class BreadcrumbViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string currentPageTitle = "")
        {
            var items = new List<BreadcrumbItem>();

            // Всегда добавляем "Главная"
            items.Add(new BreadcrumbItem
            {
                Title = "Главная",
                Url = Url.Action("Index", "Home")
            });

            // Определяем текущий контроллер и действие
            var controller = ViewContext.RouteData.Values["controller"]?.ToString();
            var action = ViewContext.RouteData.Values["action"]?.ToString();

            if (controller == "Book" && action == "Index")
            {
                items.Add(new BreadcrumbItem { Title = "Каталог", Url = null });
            }
            else if (controller == "Book" && action == "Details")
            {
                items.Add(new BreadcrumbItem { Title = "Каталог", Url = Url.Action("Index", "Book") });
                items.Add(new BreadcrumbItem { Title = currentPageTitle, Url = null });
            }
            else if (controller == "Info")
            {
                items.Add(new BreadcrumbItem { Title = "Информация", Url = null });
            }
            else if (controller == "Cart")
            {
                items.Add(new BreadcrumbItem { Title = "Корзина", Url = null });
            }
            else if (controller == "Order")
            {
                items.Add(new BreadcrumbItem { Title = "Оформление заказа", Url = null });
            }

            return View(items);
        }
    }

    public class BreadcrumbItem
    {
        public string Title { get; set; }
        public string Url { get; set; }
    }
}