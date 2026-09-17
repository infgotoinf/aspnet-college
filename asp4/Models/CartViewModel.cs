namespace asp3.Models;

public class CartItem
{
    public string Name { get; set; } = "";
    public decimal Price { get; set; }
}

public class CartViewModel
{
    public List<CartItem> Items { get; set; } = new();
    public decimal TotalAmount => Items.Sum(item => item.Price);
}
