using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ShopAspNet.Models;
//: DbContext
public class ShopContext : IdentityDbContext<User, IdentityRole<int>, int>
{
    public ShopContext(DbContextOptions options) : base(options) { }
    public virtual DbSet<Category> Categories { get; set; } = null!;
    public virtual DbSet<Product> Products { get; set; } = null!;

    public virtual DbSet<ImageFile> ImageFiles { get; set; } = null!;

    public virtual DbSet<Cart> Carts { get; set; } = null!;

    public virtual DbSet<CartProduct> CartsProducts { get; set; } = null!;

    public virtual DbSet<Order> Orders { get; set; } = null!;

    public virtual DbSet<OrderProduct> OrderProducts { get; set; } = null!;

	public override DbSet<User> Users { get; set; } = null!;
}
