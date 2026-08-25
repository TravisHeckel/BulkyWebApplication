using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace BulkyWebApplication.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        // Note: IWebHostEnvironment is injected but unused here (leftover from the Product
        // controller pattern, which needs it for file uploads).
        public CompanyController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
        }

        // The Index view renders an empty table; the rows are loaded by JavaScript
        // (DataTables) which calls the GetAll() API method below. See the #region at the bottom.
        public IActionResult Index()
        {
            List<Company> objCompanyList = _unitOfWork.Company.GetAll().ToList();
            return View(objCompanyList);
        }

        // "Upsert" = Update + Insert in ONE screen. If there's no id we show a blank form to
        // CREATE; if there is an id we load that company to EDIT. One view handles both.
        public IActionResult Upsert(int? id)
        {
            if(id == null || id == 0)
            {
                //create
                return View(new Company());
            }
            else
            {
                //update
                Company companyObj = _unitOfWork.Company.Get(u=>u.Id == id);
                return View(companyObj);
            }
        }

        // POST for the Upsert form. Id == 0 means it's a new record (INSERT); otherwise UPDATE.
        [HttpPost]
        public IActionResult Upsert(Company CompanyObj )
        {
            if (ModelState.IsValid)
            {
                if(CompanyObj.Id == 0)
                {
                    _unitOfWork.Company.Add(CompanyObj);
                }
                else
                {
                    _unitOfWork.Company.Update(CompanyObj);
                }

                _unitOfWork.Save();
                TempData["success"] = "Company created successfully";
                return RedirectToAction("Index");
            }
            else
            {
                return View(CompanyObj);
            }
        }

        // ===== API CALLS =====
        // These return JSON (not views) and are called by client-side JavaScript. This is how
        // the jQuery DataTables grid loads rows and how the delete button works without a page reload.
        #region API CALLS

        // Returns all companies as JSON in the shape DataTables expects: { data: [...] }.
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Company> objCompanyList = _unitOfWork.Company.GetAll().ToList();
            return Json(new { data = objCompanyList });
        }

        // Called via an AJAX DELETE request; returns a JSON success/error the JS can show as a toast.
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var CompanyToBeDeleted = _unitOfWork.Company.Get(u=> u.Id == id);
            if (CompanyToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            _unitOfWork.Company.Remove(CompanyToBeDeleted);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Delete Successful" });
        }

        #endregion
    }
}
