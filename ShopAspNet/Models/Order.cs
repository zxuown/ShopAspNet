namespace ShopAspNet.Models;

public class Order
{
    public Order()
    {
        Products = new HashSet<OrderProduct>();
    }
  
    public enum OrderStatus
    {
        Notpaid, Paid, Done
    }
    public OrderStatus Status { get; set; }
    public int Id { get; set; }
    public string Uid { get; set; }
    public virtual ICollection<OrderProduct> Products { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string StreetAdress { get; set; }

    public string Country { get; set; }

    public string City { get; set; }

    public string PostCode {  get; set; }  

    public string Phone {  get; set; }

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
