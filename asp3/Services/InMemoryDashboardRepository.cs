using asp3.Models;

namespace asp3.Services;

public class InMemoryDashboardRepository : IDashboardRepository
{
    private static readonly List<DashboardCard> Cards =
    [
        new DashboardCard
        {
            Id = 1,
            Title = "Выручка",
            Value = 125000,
            Trend = Trend.Up,
            Unit = "руб.",
            Description = "Общая выручка за текущий период"
        },
        new DashboardCard
        {
            Id = 2,
            Title = "Заказы",
            Value = 840,
            Trend = Trend.Up,
            Unit = "шт.",
            Description = "Количество оформленных заказов"
        },
        new DashboardCard
        {
            Id = 3,
            Title = "Конверсия",
            Value = 7.8m,
            Trend = Trend.Stable,
            Unit = "%",
            Description = "Средняя конверсия посетителей"
        },
        new DashboardCard
        {
            Id = 4,
            Title = "Возвраты",
            Value = 42,
            Trend = Trend.Down,
            Unit = "шт.",
            Description = "Количество возвращённых заказов"
        },
        new DashboardCard
        {
            Id = 5,
            Title = "Прибыль",
            Value = -15000,
            Trend = Trend.Down,
            Unit = "руб.",
            Description = "Отрицательная прибыль за период"
        },
        new DashboardCard
        {
            Id = 6,
            Title = "Новые клиенты",
            Value = 1560,
            Trend = Trend.Up,
            Unit = "шт.",
            Description = "Количество новых клиентов"
        }
    ];

    public IEnumerable<DashboardCard> GetAllCards()
    {
        return Cards;
    }

    public DashboardCard? GetById(int id)
    {
        return Cards.FirstOrDefault(card => card.Id == id);
    }

    public IEnumerable<DashboardCard> GetLatest(int count)
    {
        return Cards
            .OrderByDescending(card => card.Id)
            .Take(count);
    }
}
