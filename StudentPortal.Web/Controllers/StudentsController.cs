using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentPortal.Web.Data;
using StudentPortal.Web.Models;
using StudentPortal.Web.Models.Entities;

namespace StudentPortal.Web.Controllers
{
    public class StudentsController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public StudentsController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Add(AddStudentViewModel viewModel)
        {
            var student = new Student
            {
                Name = viewModel.Name,
                Phone = viewModel.Phone,
                Admission = viewModel.Admission,
                Gender = viewModel.Gender,
                Fees = viewModel.Fees,
                Email = viewModel.Email,
                Subscribed = viewModel.Subscribed,
            };
            await dbContext.Students.AddAsync(student);
            await dbContext.SaveChangesAsync();

            return View();
        }
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var students= await dbContext.Students.ToListAsync();
            return View(students);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var student = await dbContext.Students.FindAsync(id);
            return View(student);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Student viewModel)
        {
            var student = await dbContext.Students.FindAsync(viewModel.Id);
            if (student is not null)
            {
                student.Name = viewModel.Name;
                student.Phone = viewModel.Phone;
                student.Admission = viewModel.Admission;
                student.Gender = viewModel.Gender;
                student.Fees = viewModel.Fees;
                student.Email = viewModel.Email;
                student.Subscribed = viewModel.Subscribed;

                await dbContext.SaveChangesAsync();
            }

            return RedirectToAction("List", "Students");
        }
        [HttpGet]
        public async Task<IActionResult> Delete(Guid? id)
        {
            
            var student = await dbContext.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var student = await dbContext.Students.FindAsync(id);
            if(student is not null)
            {
                dbContext.Students.Remove(student);
            }
            await dbContext.SaveChangesAsync();
            return RedirectToAction("List", "Students");
        }

        [HttpGet]
        public async Task<IActionResult> Search(string searchTerm)
        {
            if (dbContext.Students == null)
            {
                return Problem("Student 'searchTerm' is not available.");
            }

            var students = from name in dbContext.Students
                           select name;

            if (!string.IsNullOrEmpty(searchTerm))
            {
                students = students.Where(name => name.Name.Contains(searchTerm) || name.Email.Contains(searchTerm) || name.Phone.Contains(searchTerm) || name.Gender.Contains(searchTerm));
            }

            await dbContext.SaveChangesAsync();

            return View(await students.ToListAsync());
        }
    }
}
