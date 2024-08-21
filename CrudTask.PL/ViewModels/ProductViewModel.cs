using CrudTask.DAL.Data.Entities;

namespace CrudTask.PL.ViewModels
{
    public class ProductViewModel
    {
        public IEnumerable<Product> Products { get; set; }
    }
}
