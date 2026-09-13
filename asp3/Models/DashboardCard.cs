namespace asp3.DashboardCard;

enum eTrend {
	Up,
	Down,
	Stable
}

public class DashboardCard
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public decimal Value { get; set; } = 0;
    public eTrend Trend { get; set; } = "Stable";
    public string Unit { get; set; } = "%";
    public string? Description { get; set; }
}
