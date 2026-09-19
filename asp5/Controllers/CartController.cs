using System.Text.Json;
using asp3.Models;
using Microsoft.AspNetCore.Mvc;

namespace asp3.Controllers;

public class CartController : Controller
{
    public IActionResult Index()
    {
        var items = GetCart()
            .Select(id => CatalogController.Products.FirstOrDefault(p => p.Id == id))
            .Where(product => product != null)
            .Select(product => new CartItem
            {
                Name = product!.Name,
                Price = product.Price
            })
            .ToList();

        return View(new CartViewModel { Items = items });
    }

    private List<int> GetCart()
    {
        var value = HttpContext.Session.GetString("cart");

        return value == null
            ? []
            : JsonSerializer.Deserialize<List<int>>(value) ?? [];
    }
}
