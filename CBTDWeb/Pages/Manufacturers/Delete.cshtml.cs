using DataAccess;
using Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CBTDWeb.Pages.Manufacturers
{
    public class DeleteModel : PageModel
    {

        private readonly UnitOfWork _UnitOfWork;
        [BindProperty]  //synchronizes form fields with values in code behind
        public Manufacturer objManufacturer { get; set; }

        public DeleteModel(UnitOfWork unitOfWork)  //dependency injection
        {
            _UnitOfWork = unitOfWork;
            objManufacturer = new Manufacturer();
        }

        public IActionResult OnGet(int? id)

        {
            objManufacturer = new Manufacturer();

            //am I in edit mode?
            if (id != 0)
            {
                objManufacturer = _UnitOfWork.Manufacturer.GetById(id);
            }

            if (objManufacturer == null)  //nothing found in DB
            {
                return NotFound();   //built in page
            }

            //assuming I'm in create mode
            return Page();
        }
        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            _UnitOfWork.Manufacturer.Delete(objManufacturer);  //Removes from memory
            TempData["success"] = "Manufacturer Deleted Successfully";
            _UnitOfWork.Commit();   //saves to DB

            return RedirectToPage("./Index");
        }

    }
}
