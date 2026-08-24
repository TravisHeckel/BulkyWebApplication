using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWebApplication.Areas.Admin.Controllers
{
    // [Area("Admin")] groups these admin screens under the /Admin/... URL space.
    [Area("Admin")]
    //[Authorize(Roles = SD.Role_Admin)]  // <- uncomment to restrict this to admins only
    public class CategoryController : Controller
    {
        // The controller depends on the ABSTRACTION (IUnitOfWork), not on EF directly.
        // ASP.NET Core's DI container injects the concrete UnitOfWork at runtime
        // (registered in Program.cs). This is "constructor injection".
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET /Admin/Category/Index -> list all categories.
        public IActionResult Index()
        {
            List<Category> objCategoryList = _unitOfWork.Category.GetAll().ToList();
            return View(objCategoryList); // pass the list to Index.cshtml
        }

        // GET: show the empty "create" form.
        public IActionResult Create()
        {
            return View();
        }

        // POST: handle the submitted "create" form. The Category is model-bound from the
        // posted form fields automatically.
        [HttpPost]
        public IActionResult Create(Category obj)
        {
            // Example of a custom validation rule beyond the DataAnnotations on the model.
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "The DisplayOrder cannot exactly match the Name.");
            }

            // ModelState.IsValid checks all [Required]/[Range]/etc rules AND the custom one above.
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Add(obj); // stage insert
                _unitOfWork.Save();            // commit to database
                // TempData survives exactly one redirect - used here to flash a success toast.
                TempData["success"] = "Category created successfully";
                return RedirectToAction("Index");
            }
            return View(); // validation failed -> redisplay the form with errors
        }

        // GET: show the "edit" form pre-filled with the chosen category.
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category? categoryFromDb = _unitOfWork.Category.Get(u => u.Id == id);
            //Category? categoryFromDb1 = _db.Categories.FirstOrDefault(u=>u.Id==id);
            //Category? categoryFromDb2 = _db.Categories.Where(u => u.Id == id).FirstOrDefault();
            if (categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb);
        }

        // POST: save the edited category.
        [HttpPost]
        public IActionResult Edit(Category obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "Category updated successfully";
                return RedirectToAction("Index");
            }
            return View();
        }

        // GET: show a confirmation page before deleting.
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category? categoryFromDb = _unitOfWork.Category.Get(u => u.Id == id);

            if (categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb);
        }

        // POST: actually delete. [ActionName("Delete")] lets this method be named
        // DeletePOST in C# while still responding to the "Delete" action - a common trick
        // to have separate GET (confirm) and POST (perform) methods with the same URL.
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            Category? obj = _unitOfWork.Category.Get(u => u.Id == id);

            if (obj == null)
            {
                return NotFound();
            }
            _unitOfWork.Category.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Category deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
