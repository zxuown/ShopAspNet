using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopAspNet.Models;

namespace ShopAspNet.Controllers
{
    [Authorize(Roles = "Admin")]
    public class OrdersController : Controller
    {
        private readonly ShopContext _context;
        public OrdersController(ShopContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View(_context.Orders.Include(x=>x.Products).ThenInclude(x=>x.Product).ToList());
        }
        [HttpGet("/Orders/Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var order = _context.Orders.Include(x => x.Products).ThenInclude(x => x.Product).First(x=>x.Id == id);
            return View(order);
        }

        [HttpPost("/Orders/Edit/{id}")]
        public async Task<IActionResult> EditAsync(int id, [FromForm] Order orderItem)
        {
            var order = _context.Orders.First(x => x.Id == id);
            order.FirstName = orderItem.FirstName;
            order.LastName = orderItem.LastName;
            order.City = orderItem.City;
            order.StreetAdress = orderItem.StreetAdress;
            order.Phone = orderItem.Phone;
            order.Country = orderItem.Country;
            order.Phone = orderItem.Phone;
            order.PostCode = orderItem.PostCode;
            order.Status = orderItem.Status;
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost("/Orders/Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var order = _context.Orders.First(x => x.Id == id);
            _context.Orders.Remove(order);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
