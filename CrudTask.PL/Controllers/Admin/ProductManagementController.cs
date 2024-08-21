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
        public async Task<IActionResult> Index()
        {
            var item = await _repo.GetAllAsync();
            var model = new ProductViewModel
            {
                Products = item            
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
            var item = new Product
            {
                Name = model.Name,
                Price = model.Price,
                Quantity = model.Quantity,
                ExpirationDate = model.ExpirationDate,
                Description = model.Description
            };
            

            await _repo.AddAsync(item);
            _repo.SaveChanges();
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
            item.Name = model.Name;
            item.Name = model.Name;
            item.Name = model.Name;
            item.Name = model.Name;
            item.Description = model.Description;

            

            _repo.Update(item);
            _repo.SaveChanges();

            return RedirectToAction("Index");

        }
    }
}
