using PRN222.CourseManagement.Repository.Models;

namespace PRN222.CourseManagement.RepositoryTest.Repository_Test
{
    [TestFixture]
    public class GenericErnollmentRepository_Test
    {
        [Test]
        public void Add_AddsEnrollment_WhenValid()
        {
            // Arrange
            var ctx = DBContext.InMemoryDbContext_Test.GetCourseManagement();
            var repo = new Repository.Repository.GenericRepository.GenericRepository<Enrollment>(ctx);

            var dept = new Department
            {
                Name = "SE"
            };
            ctx.Departments.Add(dept);
            ctx.SaveChanges();

            var student = new Student
            {
                StudentCode = "SE170001",
                FullName = "Nguyen Van A",
                Email = "a@fpt.edu.vn",
                DepartmentId = dept.DepartmentId
            };
            ctx.Students.Add(student);
            ctx.SaveChanges();

            var course = new Course
            {
                CourseCode = "PRN222",
                Title = "C# .NET",
                Credits = 3,
                DepartmentId = dept.DepartmentId
            };
            ctx.Courses.Add(course);
            ctx.SaveChanges();

            var enrollment = new Enrollment
            {
                StudentId = student.StudentId,
                CourseId = course.CourseId,
                EnrollDate = DateTime.Now,
                Grade = 8.5m
            };

            // Act
            repo.Add(enrollment);
            ctx.SaveChanges();

            // Assert
            var exists = repo.Exists(e =>
                e.StudentId == student.StudentId &&
                e.CourseId == course.CourseId);

            Assert.IsTrue(exists);
        }


        [Test]
        public void Update_UpdatesEnrollment_WhenExists()
        {
            // Arrange
            var ctx = DBContext.InMemoryDbContext_Test.GetCourseManagement();
            var repo = new Repository.Repository.GenericRepository.GenericRepository<Enrollment>(ctx);

            var dept = new Department { Name = "SE" };
            ctx.Departments.Add(dept);
            ctx.SaveChanges();

            var student = new Student
            {
                StudentCode = "SE170001",
                FullName = "Nguyen Van A",
                Email = "a@fpt.edu.vn",
                DepartmentId = dept.DepartmentId
            };
            ctx.Students.Add(student);
            ctx.SaveChanges();

            var course = new Course
            {
                CourseCode = "PRN222",
                Title = "C# .NET",
                Credits = 3,
                DepartmentId = dept.DepartmentId
            };
            ctx.Courses.Add(course);
            ctx.SaveChanges();

            var enrollment = new Enrollment
            {
                StudentId = student.StudentId,
                CourseId = course.CourseId,
                EnrollDate = DateTime.Now,
                Grade = 7.0m
            };

            repo.Add(enrollment);
            ctx.SaveChanges();

            // modify
            enrollment.Grade = 9.0m;

            // Act
            repo.Update(enrollment);
            ctx.SaveChanges();

            // Assert
            var updated = ctx.Enrollments.Find(student.StudentId, course.CourseId);

            Assert.IsNotNull(updated);
            Assert.AreEqual(9.0m, updated!.Grade);
        }


        [Test]
        public void Delete_RemovesEnrollment_WhenExists()
        {
            // Arrange
            var ctx = DBContext.InMemoryDbContext_Test.GetCourseManagement();
            var repo = new Repository.Repository.GenericRepository.GenericRepository<Enrollment>(ctx);

            var dept = new Department { Name = "SE" };
            ctx.Departments.Add(dept);
            ctx.SaveChanges();

            var student = new Student
            {
                StudentCode = "SE170001",
                FullName = "Nguyen Van A",
                Email = "a@fpt.edu.vn",
                DepartmentId = dept.DepartmentId
            };
            ctx.Students.Add(student);
            ctx.SaveChanges();

            var course = new Course
            {
                CourseCode = "PRN222",
                Title = "C# .NET",
                Credits = 3,
                DepartmentId = dept.DepartmentId
            };
            ctx.Courses.Add(course);
            ctx.SaveChanges();

            var enrollment = new Enrollment
            {
                StudentId = student.StudentId,
                CourseId = course.CourseId,
                EnrollDate = DateTime.Now
            };

            repo.Add(enrollment);
            ctx.SaveChanges();

            // Act
            ctx.Enrollments.Remove(enrollment);
            ctx.SaveChanges();

            // Assert
            var exists = repo.Exists(e =>
                e.StudentId == student.StudentId &&
                e.CourseId == course.CourseId);

            Assert.IsFalse(exists);
        }



    }
}
