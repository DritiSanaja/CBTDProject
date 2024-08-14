using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Stripe.Checkout;
using Microsoft.AspNetCore.Identity.UI.Services;
using Utility;

namespace CBTDWeb.Pages.Cart
{
    public class OrderConfirmationModel : PageModel
    {   

        
            [BindProperty]
            public int OrderId { get; set; }
            private readonly UnitOfWork _unitOfWork;
            private readonly IEmailSender _emailSender;   //using Microsoft.AspNetCore.Identity.UI.Services;
            public OrderConfirmationModel(UnitOfWork unitOfWork, IEmailSender emailSender)
            {
                _unitOfWork = unitOfWork;
                _emailSender = emailSender;
            }
            public void OnGet(int orderId)
            {
                OrderHeader objOrderHeader = _unitOfWork.OrderHeader.Get(o => o.Id == orderId, includes: "ApplicationUser");
                OrderId = objOrderHeader.Id;
               
                var service = new SessionService();
                Session session = service.Get(objOrderHeader.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _unitOfWork.OrderHeader.UpdateStatus(orderId, SD.StatusApproved, SD.PaymentStatusApproved);
                    _unitOfWork.Commit();
                }
                //Send an e-mail

                _emailSender.SendEmailAsync(objOrderHeader.ApplicationUser.Email, "New Order - CBTD", "<p>New Order Created.</p>Your order number is " + OrderId.ToString());

                //remove shopping cart items
                List<ShoppingCart> shoppingCartItems = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId ==
                objOrderHeader.ApplicationUserId).ToList();
                _unitOfWork.ShoppingCart.Delete(shoppingCartItems);
                _unitOfWork.Commit();
            }
        }

    }


