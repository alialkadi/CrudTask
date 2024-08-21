using System.ComponentModel.DataAnnotations;

namespace CrudTask.PL.ViewModels
{
    public class ProductDTO
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Name is Required")]
        public string? Name { get; set; }
        [Required(ErrorMessage = "Description is required")]
        public string? Description { get; set; }
        [Required(ErrorMessage = "Price is required")]
        public decimal Price { get; set; }
        [Required(ErrorMessage = "Quantity is required")]
        public int Quantity { get; set; }
        [Required(ErrorMessage = "Date is required")]
        public DateTime ExpirationDate { get; set; }
    }
}
