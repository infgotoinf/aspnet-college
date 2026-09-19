using asp3.Models;

namespace asp3.Services;

public interface IDashboardRepository
{
    IEnumerable<DashboardCard> GetAllCards();
    DashboardCard? GetById(int id);
    IEnumerable<DashboardCard> GetLatest(int count);
}
