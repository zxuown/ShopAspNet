
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ShopAspNet.Models;
using ShopAspNet.Models.LiqPaySDK;
using System.Text;

namespace ShopAspNet.Controllers
{
    public class HomeController : Controller
    {
		private readonly LiqPay _liqPay;
		public HomeController(ShopContext context, LiqPay liqPay) 
        {
            _context = context;
            _liqPay = liqPay;
            
		}
        private readonly ShopContext _context;
        
        public Cart GetCart()
        {
            var uid = HttpContext.Items["BuyerUid"].ToString();
            var cart = _context.Carts.Include(x => x.Products)
                .ThenInclude(x => x.Product)
                .ThenInclude(x => x.MainImage)
                .FirstOrDefault(x => x.Uid == uid);
            return cart;
        }

        [HttpPost("/checkout")]
        public IActionResult CheckOut([FromForm] Order order) 
        {
            var cart = GetCart();
            foreach (var item in cart.Products)
            {
                order.Products.Add(new OrderProduct
                {
                    Price = item.Product.Price,
                    Product = item.Product,
                    Quantity = item.Quantity,
                });
            }
            cart.Products.Clear();
            _context.Orders.Add(order);
            order.Status = Order.OrderStatus.Notpaid;
            order.Uid = Guid.NewGuid().ToString();
            _context.SaveChanges();
            return RedirectToAction(nameof(Pay), new
            {
                uid= order.Uid,
            });
        }
        ///liqpay/ef7526c4-05da-4326-941b-f52b1e778f53
		[HttpGet("/liqpay/{uid}")]
		public IActionResult Pay(string uid)
        {
            ViewData["Categories"] = _context.Categories.ToList();
            var order = _context.Orders
                .Include(x=>x.Products)
                .First(x=>x.Uid == uid);
            var p = _liqPay.PayParams(order.Total(),"Order Product", order.Uid);
			ViewData["data"] = _liqPay.GetData(p);
			ViewData["signature"] = _liqPay.GetSignature(p);
			return View("LiqPay", order);
        }
        [HttpPost("/paymentResult")]
        public IActionResult PayResult([FromBody]Notify notify)
        {
            if (!_liqPay.ValidateData(notify.Data,notify.Signature))
            {
                return BadRequest(new
                {
                    success = false,
                });
            }
            var json = Encoding.UTF8.GetString(Convert.FromBase64String(notify.Data));
            var response = JsonConvert.DeserializeObject<PayResponse>(json);
            if (response.Status == "success")
            {
                var order = _context.Orders.First(x => x.Uid == response.OrderId);
                order.Status = Order.OrderStatus.Paid;
                _context.SaveChanges();
            }
            return Ok(new
            {
                success = true,
            });
        }
        [HttpGet("/checkout")]
        public IActionResult CheckOut()
        {
            ViewData["Categories"] = _context.Categories.ToList();
            ViewData["cart"] = GetCart();
            return View(new Order());
        }

        [HttpGet("/cart")]
        public IActionResult Cart()
        {
            ViewData["Categories"] = _context.Categories.ToList();
            return View(GetCart());
        }

        [HttpGet("/product/{id}")]
        public IActionResult Product(int id)
        {
            ViewData["Categories"] = _context.Categories.ToList();
            var product = _context.Products
                .Include(x=>x.MainImage)
                .Include(x => x.Category)
                .SingleOrDefault(p => p.Id == id);
            FindProductCount();
            ViewData["SimilarProducts"] = _context.Products
                .Include(x => x.MainImage)
                .Include(x => x.Category)
                .Where(x=> x.Id != id && x.Category.Id == product.Category.Id).ToList();

            return View(product);
        }
        [HttpGet("/cat/{id}")]
        public IActionResult Category(int id)
        {
            ViewData["Categories"] = _context.Categories.ToList();
            return View(_context.Categories.Include(x=>x.Image)
                .Include(x => x.Products)
                .ThenInclude(x=>x.MainImage)
                .First(x=> x.Id == id));
        }
        [HttpGet("/cat-p/{sort}/{id}")]
        public IActionResult CategoryProducts(int sort, int id)
        {
            var category = _context.Categories.Include(x => x.Image)
                .Include(x => x.Products)
                .ThenInclude(x => x.MainImage)
                .First(x => x.Id == id);
            if (sort == 1)
            {
                category.Products = (ICollection<Product>)category.Products.OrderBy(x => x.Price).ToList();
            }
            else
                category.Products = (ICollection<Product>)category.Products.OrderByDescending(x => x.Price).ToList();
            return PartialView("~/Views/Home/_CategoryProducts.cshtml", category);
        }
        [HttpGet("/")]
      
        public IActionResult Index()
        {
            ViewData["Categories"] = _context.Categories.ToList();
            FindProductCount();
            return View(new {
                Products = _context.Products
                .Include(x => x.Category)
                .Include(x => x.MainImage)
                .Include(x => x.Images)
                .ToList(),
                Categories = _context.Categories
                .Include(x => x.Image),        
            });
        }
        public void FindProductCount()
        {
            var cart = GetCart();
            if (cart == null)
            {
                ViewData["CartProductsCount"] = 0;
            }
            else
            {
                ViewData["CartProductsCount"] = cart.Products.Count();
            }
        }
    }
}
