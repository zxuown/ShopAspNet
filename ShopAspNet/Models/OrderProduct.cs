namespace ShopAspNet.Models;

public class OrderProduct
{
    public int Id { get; set; }

    public virtual Order Order { get; set; }

    public virtual Product Product { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }

	public decimal TotalPrice()
	{
		return Price * Quantity;
	}
}
