namespace asp3.Interface;

using asp3.DashboardCard;

public interface IDashboardRepository
{
	List<DashboardCard> GetAllCards();
	DashboardCard? GetById(int id);
	DashboardCard? GetLatest(int count);
}
