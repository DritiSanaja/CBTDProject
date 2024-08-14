using Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace CBTDWeb.Pages
{
    public class ProductDetailsModel : PageModel
    {
        private readonly UnitOfWork _unitOfWork;

        public Product objProduct;

        [BindProperty]
        public int txtCount { get; set; }
        public ShoppingCart objCart = new();

        public ProductDetailsModel(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            objProduct = new Product();
        }

        // In the HTML page the asp-route-productId is why we get the int and the name
        public IActionResult OnGet(int? id)
        {
            // In the HTML page the asp-route-productId is why we get the int and the name

            {
                //check to see if user logged on
                var claimsIdentity = User.Identity as ClaimsIdentity;
                var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);
                TempData["UserLoggedIn"] = claim;
                objProduct = _unitOfWork.Product.Get(p => p.Id == id, includes: "Category,Manufacturer");
                return Page();
            }

        }

        public IActionResult OnPost(Product objProduct)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var userId = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var existingCart = _unitOfWork.ShoppingCart.Get(
                u => u.ApplicationUserId == userId && u.ProductId == objProduct.Id);

            if (existingCart == null)
            {
                var newCart = new ShoppingCart
                {
                    ApplicationUserId = userId,
                    ProductId = objProduct.Id,
                    Count = txtCount
                };

                _unitOfWork.ShoppingCart.Add(newCart);
            }
            else
            {
                _unitOfWork.ShoppingCart.IncrementCount(existingCart, txtCount);
                _unitOfWork.ShoppingCart.Update(existingCart);
            }

            _unitOfWork.Commit();
            return RedirectToPage("Index");
        }


    }
}