using CBTDWeb.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace CBTDWeb.Pages.Cart
{
    public class IndexModel : PageModel
    {
        private readonly UnitOfWork _unitOfWork;
        public ShoppingCartVM? ShoppingCartVM { get; set; }

        public IndexModel(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            InitializeShoppingCartVM();
        }

        private void InitializeShoppingCartVM()
        {
            ShoppingCartVM = new ShoppingCartVM
            {
                cartItems = []
            };
        }


        public IActionResult OnGet()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;

            var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartVM = new ShoppingCartVM()
            {
                cartItems = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value, u => u.ProductId, "Product"),
            };
            foreach (var item in ShoppingCartVM.cartItems)
            {
                item.CartPrice = ShoppingCartVM.GetPriceBasedOnQuantity(item.Count, item.Product.UnitPrice,
                    item.Product.HalfDozenPrice, item.Product.DozenPrice);
                ShoppingCartVM.CartTotal += (item.CartPrice * item.Count);

            }
            return Page();

        }
       

        public IActionResult OnPostMinus(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.Get(c => c.Id == cartId);
            if (cart.Count == 1)
            {
                _unitOfWork.ShoppingCart.Delete(cart);
            }


            else
            {
                cart.Count -= 1;


                _unitOfWork.ShoppingCart.Update(cart);
            }
            _unitOfWork.Commit();
            var cnt = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == cart.ApplicationUserId).Count();

            return RedirectToPage();
        }


        public IActionResult OnPostPlus(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.Get(c => c.Id == cartId);
            cart.Count += 1;
            _unitOfWork.ShoppingCart.Update(cart);
            _unitOfWork.Commit();
            return RedirectToPage();
        }


        public IActionResult OnPostRemove(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.Get(c => c.Id == cartId);
            _unitOfWork.ShoppingCart.Delete(cart);
            _unitOfWork.Commit();
            var cnt = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == cart.ApplicationUserId).Count();

            return RedirectToPage();


        }


    }
}