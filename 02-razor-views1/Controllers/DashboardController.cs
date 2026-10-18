using asp3.Services;
using Microsoft.AspNetCore.Mvc;

namespace asp3.Controllers;

public class DashboardController : Controller
{
    private readonly IDashboardRepository _repository;

    public DashboardController(IDashboardRepository repository)
    {
        _repository = repository;
    }

    public IActionResult Index()
    {
        ViewData["Title"] = "Дашборд";
        ViewData["ReportPeriod"] = "Сентябрь 2026";
        ViewBag.Currency = "RUB";

        var cards = _repository.GetAllCards();

        return View(cards);
    }
}
