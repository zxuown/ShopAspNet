namespace ShopAspNet.Models;

public class Cart
{
	public Cart()
	{
		Products = new HashSet<CartProduct>();
	}
	public int Id { get; set; }
	public string Uid { get; set; }

	public virtual ICollection<CartProduct> Products { get; set; }

	public decimal SubTotal()
	{
		return Products.Sum(x => x.TotalPrice());
	}
	public decimal Shipping()
	{
		return SubTotal() * 0.05m;
	}
	public decimal Total()
	{
		return SubTotal() + Shipping();
	}

}
