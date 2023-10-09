using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopAspNet.Models;

namespace ShopAspNet.Controllers
{
    
    public class CartController : Controller
    {
        private readonly ShopContext _context;
        public CartController(ShopContext context)
        {
            _context = context; 
        }

        [HttpGet("/Cart/TotalInfo")]
        public IActionResult TotalInfo()
        {
			var uid = HttpContext.Items["BuyerUid"].ToString();
			var cart = _context.Carts.Include(x => x.Products)
				.ThenInclude(x => x.Product)
				.FirstOrDefault(x => x.Uid == uid);
			return PartialView("~/Views/Home/_TotalInfo.cshtml",cart);
        }
        [HttpPost("/Cart/SetNewQuantity")]
        public IActionResult setNewQuantity([FromBody] AddCartProduct addCartProduct) 
        {
			var uid = HttpContext.Items["BuyerUid"].ToString();
			var cart = _context.Carts.Include(x => x.Products)
				.ThenInclude(x => x.Product)
				.FirstOrDefault(x => x.Uid == uid);
			var cartProduct = cart.Products.FirstOrDefault(x => x.Id == addCartProduct.ProductId);
            cartProduct.Quantity = addCartProduct.Quantity;
            _context.SaveChanges();
            return Ok(new
            {
                Ok = true,
                Price = cartProduct.Product.Price * cartProduct.Quantity,

            });
		}

        [HttpPost("/Cart/Add")]
        public IActionResult Add([FromBody] AddCartProduct addCartProduct)
        {
            var uid = HttpContext.Items["BuyerUid"].ToString();
            var cart = _context.Carts.Include(x => x.Products)
                .ThenInclude(x => x.Product)
                .FirstOrDefault(x => x.Uid == uid);
            if (cart == null)
            {
                cart = new Cart() {
                    Uid = uid,
                };
                _context.Carts.Add(cart);
            }
            var product = _context.Products.First(x => x.Id == addCartProduct.ProductId);
            var cartProduct = cart.Products.FirstOrDefault(x => x.Product.Id == product.Id);
            if (cartProduct == null)
            {
                cart.Products.Add(new CartProduct()
                {
                    Product = product,
                    Quantity = addCartProduct.Quantity,
                });
            }
            else
            {
                cartProduct.Quantity += addCartProduct.Quantity;
            }
            _context.SaveChanges();
            return Ok(new
            {
                CartProductCount = cart.Products.Count()
            });
        }
        [HttpDelete("/Cart/Delete/{id}")]
        public IActionResult Delete(int id)
        {
            var uid = HttpContext.Items["BuyerUid"].ToString();
            var cart = _context.Carts.Include(x => x.Products)
                .ThenInclude(x => x.Product)
                .FirstOrDefault(x => x.Uid == uid);
            var cartProduct = cart.Products.FirstOrDefault(x => x.Id == id);
            if (cartProduct != null)
            {
				cart.Products.Remove(cartProduct);
				_context.SaveChanges();
			}
            return Ok(new { Ok = true });
        }
    }
}
