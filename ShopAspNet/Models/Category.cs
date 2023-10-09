namespace ShopAspNet.Models;

public class Category
{
    public Category() 
    { 
        Products = new HashSet<Product>();
    }
    public int Id { get; set; }
    public string Title { get; set; }
    public virtual ImageFile? Image { get; set; }
    public virtual ICollection<Product> Products { get; set; }
}
