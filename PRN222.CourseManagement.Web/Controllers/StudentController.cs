using Microsoft.AspNetCore.Mvc;
using PRN222.CourseManagement.Repository.Models;
using PRN222.CourseManagement.Service.IService;

namespace PRN222.CourseManagement.Web.Controllers
{
    public class StudentController : Controller
    {

        private readonly IStudentService _studentService;

        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpPost]
        public IActionResult Create(Student model)
        {
            var result = _studentService.Create(model);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", result.Message);
                return View(model);
            }
            TempData["Success"] = result.Message;
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Delete(int id)
        {
            var result = _studentService.Delete(id);

            if (!result.IsSuccess)
            {
                TempData["Error"] = result.Message;
            }
            else
            {
                TempData["Success"] = result.Message;
            }

            return RedirectToAction("Index");
        }


        public IActionResult Index()
        {
            var students = _studentService.GetAll();
            return View(students);
        }
    }
}
