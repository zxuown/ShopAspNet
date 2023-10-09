using System.ComponentModel.DataAnnotations.Schema;

namespace ShopAspNet.Models;

public class Product
{
    public Product()
    {
        Images = new HashSet<ImageFile>();
    }
    public int Id { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
    [ForeignKey(nameof(CategoryId))]
    public virtual Category Category { get; set; }
    public virtual ImageFile? MainImage { get; set; }
    public virtual ICollection<ImageFile> Images { get; set; }

}
