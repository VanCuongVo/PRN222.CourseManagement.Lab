using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PRN222.CourseManagement.Repository.Models;
using PRN222.CourseManagement.Service.IService;
using System.Diagnostics.CodeAnalysis;

namespace PRN222.CourseManagement.Web.Controllers
{
    [ExcludeFromCodeCoverage]
    public class StudentController : Controller
    {

        private readonly IStudentService _studentService;
        private readonly IDepartmentService _departmentService;

        public StudentController(IStudentService studentService, IDepartmentService departmentService)
        {
            _studentService = studentService;
            _departmentService = departmentService;
        }

        [HttpGet]
        public IActionResult Create()
        {
            LoadDepartments();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StudentCode,FullName,Email,DateOfBirth,DepartmentId,IsActive")] Student model)
        {
            // Remove validation for navigation properties
            ModelState.Remove("Department");
            ModelState.Remove("Enrollments");

            if (!ModelState.IsValid)
            {
                LoadDepartments();
                return View(model);
            }

            var result = await _studentService.Create(model);
            if (!result.IsSuccess)
            {
                LoadDepartments();
                ModelState.AddModelError("", result.Message);
                return View(model);
            }
            TempData["Success"] = result.Message;
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var student = _studentService.GetById(id);
            if (student == null)
            {
                TempData["Error"] = "Student not found!";
                return RedirectToAction("Index");
            }
            LoadDepartments();
            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("StudentId,StudentCode,FullName,Email,DateOfBirth,DepartmentId,IsActive")] Student model)
        {
            // Remove validation for navigation properties
            ModelState.Remove("Department");
            ModelState.Remove("Enrollments");

            if (!ModelState.IsValid)
            {
                LoadDepartments();
                return View(model);
            }

            var result = await _studentService.Update(model);
            if (!result.IsSuccess)
            {
                LoadDepartments();
                ModelState.AddModelError("", result.Message);
                return View(model);
            }
            TempData["Success"] = result.Message;
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var student = _studentService.GetById(id);
            if (student == null)
            {
                TempData["Error"] = "Student not found!";
                return RedirectToAction("Index");
            }
            return View(student);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var student = _studentService.GetById(id);
            if (student == null)
            {
                TempData["Error"] = "Student not found!";
                return RedirectToAction("Index");
            }
            return View(student);
        }


        public IActionResult Index()
        {
            var students = _studentService.GetAll();
            return View(students);
        }

        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _studentService.Delete(id);

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

        private void LoadDepartments()
        {
            var departments = _departmentService.GetAll();
            ViewBag.Departments = new SelectList(departments, "DepartmentId", "Name");
        }
    }
}
