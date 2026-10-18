using asp3.Models;
using asp3.Services;
using Microsoft.AspNetCore.Mvc;

namespace asp3.ViewComponents;

public class TrendSummaryViewComponent : ViewComponent
{
    private readonly IDashboardRepository _repository;

    public TrendSummaryViewComponent(IDashboardRepository repository)
    {
        _repository = repository;
    }

    public IViewComponentResult Invoke()
    {
        var summaries = _repository
            .GetAllCards()
            .GroupBy(card => card.Trend)
            .Select(group => new TrendSummaryItem
            {
                Trend = group.Key,
                Count = group.Count(),
                Sum = group.Sum(card => card.Value)
            })
            .ToList();

        return View(summaries);
    }

    public class TrendSummaryItem
    {
        public Trend Trend { get; set; }
        public int Count { get; set; }
        public decimal Sum { get; set; }
    }
}
