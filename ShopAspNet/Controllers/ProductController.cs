using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopAspNet.Models;
using ShopAspNet.Models.LiqPaySDK;
using static System.Net.Mime.MediaTypeNames;

namespace ShopAspNet.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
	{
		private readonly ShopContext _context;
		private readonly IWebHostEnvironment _webHostEnvironment;
		public ProductController(ShopContext context, IWebHostEnvironment hostEnvironment)
		{
			_context = context;
			_webHostEnvironment = hostEnvironment;
		}

		private async Task<Product> FindOneAsync(int id)
		{
			return await _context.Products
				.Include(p => p.Category)
				.Include(p => p.MainImage)
				.Include(p => p.Images)
				.FirstAsync(x => x.Id == id);
		}

		private async Task<List<Category>> CategoriesAll()
		{
			return await _context.Categories.ToListAsync();
		}
		[HttpGet("/AdminProduct")]
		public IActionResult Index()
		{
			return View(_context.Products
				.Include(x => x.Category)
				.Include(x => x.MainImage)
				.Include(x => x.Images)	
				.ToList());
		}
		[HttpGet("/AdminProduct/Create")]
		public async Task<IActionResult> Create()
		{
			ViewData["Categories"] = await CategoriesAll();
			return View(new Product());
		}

		private async Task<ImageFile> Upload(IFormFile imageFile)
		{
			var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);

			using (var file = System.IO.File.OpenWrite(Path.Combine(_webHostEnvironment.WebRootPath, "categoriesImage", fileName)))
			{
				await imageFile.CopyToAsync(file);
			}
			return new ImageFile { FileName = fileName };
		}
		private void RemoveImage(ImageFile imageFile) 
		{
			System.IO.File.Delete(Path.Combine(_webHostEnvironment.WebRootPath, "categoriesImage", imageFile.FileName));
			_context.Remove(imageFile);
		}

		[HttpPost("/AdminProduct/Create")]
		public async Task<IActionResult> Create([FromForm] Product product, IFormFile? mainImage, ICollection<IFormFile>? images)
		{
			product.Category = await _context.Categories.FirstAsync(x => x.Id == product.CategoryId);
			if (mainImage != null)
			{
				product.MainImage = await Upload(mainImage);
			}
			if (images != null)
			{
				foreach (var image in images)
				{
					product.Images.Add(await Upload(image));
				}
			}
			_context.Add(product);
			await _context.SaveChangesAsync();

			return RedirectToAction("Index");
		}
		[HttpGet]
		public IActionResult Show(int id)
		{
			var product = _context.Products.First(x => x.Id == id);
			return View(product);
		}
		[HttpGet("/AdminProduct/Edit/{id}")]
		public async Task<IActionResult> Edit(int id)
		{
			ViewData["Categories"] = await CategoriesAll();
			var product = await FindOneAsync(id);
			return View(product);
		}

		[HttpPost("/AdminProduct/Edit/{id}")]
		public async Task<IActionResult> EditAsync(int id, [FromForm] Product productItem, IFormFile? mainImage, ICollection<IFormFile>? images)
		{
			ViewData["Categories"] = await CategoriesAll();

			//if (!ModelState.IsValid)
			//{
			//	return View(productItem);
			//}

			var product = await FindOneAsync(id);
			if (mainImage != null)
			{
				if (product.MainImage != null)
				{
					RemoveImage(product.MainImage);
				}
				product.MainImage = await Upload(mainImage);

			}
			if (images != null && images.Count > 0)
			{
				foreach (var item in product.Images)
				{
					RemoveImage(item);
				}
				foreach (var image in images)
				{
					product.Images.Add(await Upload(image));
				}
			}
			product.Title = productItem.Title;
			product.Price = productItem.Price;
			product.Description = productItem.Description;

			_context.SaveChanges();
			return RedirectToAction("Index");
		}

		[HttpPost("/AdminProduct/Delete/{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			var product = await FindOneAsync(id);
			if (product.MainImage != null) 
			{
				RemoveImage(product.MainImage);
			}
			
			foreach (var item in product.Images)
			{
				RemoveImage(item);
			}
			_context.Products.Remove(product);
			_context.SaveChanges();
			return RedirectToAction("Index");
		}
	}
}
