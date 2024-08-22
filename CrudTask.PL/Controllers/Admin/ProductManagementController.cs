using CrudTask.BLL.Interfaces;
using CrudTask.DAL.Data.Entities;
using CrudTask.PL.Helpers;
using CrudTask.PL.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CrudTask.PL.Controllers.Admin
{
    public class ProductManagementController : Controller
    {
        private readonly IGenericRepository<Product> _repo;

        public ProductManagementController(IGenericRepository<Product> _repo)
        {
            this._repo = _repo;
        }
        [HttpGet]
        public async Task<IActionResult> Index(int pageNumber =1)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!User.IsInRole("Admin"))
            {
                return Unauthorized();
            }

            int pageSize = 10; // Number of items per page
            var allProducts = await _repo.GetAllAsync();
            var totalProducts = allProducts.Count();

            var products = allProducts
                            .Skip((pageNumber - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();

            var model = new ProductViewModel
            {
                Products = products,
                CurrentPage = pageNumber,
                TotalPages = (int)Math.Ceiling(totalProducts / (double)pageSize)
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(ProductDTO model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Check if the ExpirationDate is today or in the past
            if (model.ExpirationDate <= DateTime.Today)
            {
                ModelState.AddModelError("ExpirationDate", "The expiration date must be a future date.");
                return View(model);
            }

            var item = new Product
            {
                Name = model.Name,
                Price = model.Price,
                Quantity = model.Quantity,
                ExpirationDate = model.ExpirationDate,
                Description = model.Description
            };

            await _repo.AddAsync(item);
            _repo.SaveChanges(); // Use async save method
            return RedirectToAction(nameof(Index));

        }
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _repo.GetAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            var viewModel = new ProductViewModel
            {
                Products = new List<Product> { item }
            };

            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _repo.GetAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            _repo.Delete(item);
            _repo.SaveChanges();

            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var model = _repo.GetAsync(id).Result;
            if (model == null)
            {
                return NotFound();
            }
            var viewModel = new ProductDTO
            {
                Name = model.Name,
                Price = model.Price,
                Quantity = model.Quantity,
                ExpirationDate = model.ExpirationDate,
                Description = model.Description
            };

            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ProductDTO model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var item = await _repo.GetAsync(model.Id);
            if (item == null)
                return NotFound();
            if (model.ExpirationDate <= DateTime.Today)
            {
                ModelState.AddModelError("ExpirationDate", "The expiration date must be a future date.");
                return View(model);
            }
            item.Name = model.Name;
            item.Quantity = model.Quantity;
            item.ExpirationDate = model.ExpirationDate;
            item.Price = model.Price;
            item.Description = model.Description;

            

            _repo.Update(item);
            _repo.SaveChanges();

            return RedirectToAction("Index");

        }
        // ProductController.cs
        public async Task<IActionResult> Details(int id)
        {
            var product = await _repo.GetAsync(id); // Adjust method as needed to get the product by Id

            if (product == null)
            {
                return NotFound(); // Handle case where product is not found
            }

            var model = new ProductDTO
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Quantity = product.Quantity,
                ExpirationDate = product.ExpirationDate
            };

            return View(model);
        }

    }
}
