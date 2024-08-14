using DataAccess;
using Infrastructure.Interfaces;
using Infrastructure.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO;

namespace CBTDWeb.Pages.Products
{
    public class DeleteModel : PageModel
    {

        private readonly UnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        [BindProperty]
        public Product Product { get; set; }

        public DeleteModel(UnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;

            Product = new Product();
        }

        public IActionResult OnGet(int id)
        {
            var product = _unitOfWork.Product.GetById(id);

            if (product != null)
            {
                Product = product;
            }
            else
            {
                return NotFound();
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            var product = _unitOfWork.Product.GetById(Product.Id);

            if (product == null)
            {
                TempData["error"] = "There was a problem saving your changes.";
                return Page();
            }
            //Need to delete the physical file too
            var path = _webHostEnvironment.WebRootPath;

            if (product.ImageUrl != null)
            {
                var imagePath = Path.Combine(path, product.ImageUrl.TrimStart('\\'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            _unitOfWork.Product.Delete(product);
            TempData["success"] = "Product deleted!";

            _unitOfWork.Commit();

            return RedirectToPage("./Index");
        }
    }
}
