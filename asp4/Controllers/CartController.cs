using Microsoft.AspNetCore.Mvc;
using asp3.Models;

namespace asp3.Controllers;

public class CartController : Controller
{
    public IActionResult Index()
    {
        var model = new CartViewModel
        {
            Items = new List<CartItem>
            {
                new CartItem
                {
                    Name = "Наушники",
                    Price = 5990
                },
                new CartItem
                {
                    Name = "Клавиатура",
                    Price = 3490
                }
            }
        };

        return View(model);
    }
}
