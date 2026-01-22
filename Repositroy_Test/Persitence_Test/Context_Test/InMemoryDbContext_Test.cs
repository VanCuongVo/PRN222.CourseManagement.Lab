using Microsoft.EntityFrameworkCore;
using PRN222.CourseManagement.Repository.Models;

namespace Repositroy_Test.Persitence_Test.Context_Test
{
    public class InMemoryDbContext_Test
    {

        public static CourseManagementDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<CourseManagementDbContext>()
           .UseInMemoryDatabase(Guid.NewGuid().ToString())
           .Options;
            var context = new CourseManagementDbContext(options);

            //Tùy chọn: Xóa toàn bộ dữ liệu cũ trước mỗi test
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            return context;
        }

    // <summary>
        /// Hàm hỗ trợ Fake dữ liệu mẫu để test
        /// </summary>
        public static void SeedData(CourseManagementDbContext context)
        {
            // Fake Department
            var dep1 = new Department { Name = "IT", Description = "Software Engineering" };
            var dep2 = new Department { Name = "Business", Description = "Biz Administration" };
            context.Departments.AddRange(dep1, dep2);
            context.SaveChanges();

            // Fake Student
            var stu1 = new Student
            {
                StudentCode = "SE01",
                FullName = "Test Student A",
                Email = "student1@example.com",
                DepartmentId = dep1.DepartmentId
            };
            var stu2 = new Student
            {
                StudentCode = "SE02",
                FullName = "Test Student B",
                Email = "student2@example.com",
                DepartmentId = dep1.DepartmentId
            };
            context.Students.AddRange(stu1, stu2);

            context.SaveChanges();
        }
    }
}
