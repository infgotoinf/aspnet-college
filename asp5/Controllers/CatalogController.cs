using System.Text.Json;
using asp3.Models;
using Microsoft.AspNetCore.Mvc;

namespace asp3.Controllers;

public class CatalogController : Controller
{
    public static readonly List<Product> Products =
    [
        new(1, "Ноутбук", "Производительный ноутбук для работы и учёбы.", 79990, "Скидка"),
        new(2, "Наушники", "Беспроводные наушники с хорошим качеством звука.", 5990, "Хит"),
        new(3, "Клавиатура", "Удобная клавиатура для дома и офиса.", 3490, "Новинка")
    ];

    public IActionResult Index() => View(Products);

    public IActionResult Search(string query)
    {
        var result = string.IsNullOrWhiteSpace(query)
            ? Products
            : Products.Where(x =>
                x.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                x.Description.Contains(query, StringComparison.OrdinalIgnoreCase));

        return PartialView("_ProductList", result);
    }

    [HttpPost]
    public IActionResult AddToCart(int id)
    {
        var cart = GetCart();
        cart.Add(id);
        HttpContext.Session.SetString("cart", JsonSerializer.Serialize(cart));

        return Json(new { success = true, cartCount = cart.Count });
    }

    public IActionResult GetCartCount() =>
        Json(new { count = GetCart().Count });

    private List<int> GetCart()
    {
        var value = HttpContext.Session.GetString("cart");

        return value == null
            ? []
            : JsonSerializer.Deserialize<List<int>>(value) ?? [];
    }
}
