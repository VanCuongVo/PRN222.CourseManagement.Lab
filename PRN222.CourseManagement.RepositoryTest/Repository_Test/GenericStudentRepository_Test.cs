using PRN222.CourseManagement.Repository.Models;
using PRN222.CourseManagement.Repository.Repository.GenericRepository;
using PRN222.CourseManagement.RepositoryTest.DBContext;

namespace PRN222.CourseManagement.RepositoryTest.Repository_Test
{
    [TestFixture]
    public class GenericStudentRepository_Test
    {
        [Test]
        public async Task DeleteAsync_RemovesEntity_WhenExists()
        {
            // Arrange
            var ctx = DBContext.InMemoryDbContext_Test.GetCourseManagement();
            var repo = new Repository.Repository.GenericRepository.GenericRepository<Student>(ctx);


            var department = new Department
            {
                DepartmentId = 1,
                Name = "Software Engineering",
                Description = "SE Department"
            };
            ctx.Departments.Add(department);
            await ctx.SaveChangesAsync();

            var student = new Student
            {
                StudentId = 1,
                StudentCode = "SE170001",
                FullName = "Nguyen Van A",
                Email = "a.nguyen@fpt.edu.vn",
                DepartmentId = department.DepartmentId,
                Department = department
            };

            repo.Add(student);
            await ctx.SaveChangesAsync();

            //Act
            repo.Delete(student.StudentId);
            await ctx.SaveChangesAsync();

            //Assert
            var deleteStudent = repo.GetById(student.StudentId);
            Assert.IsNull(deleteStudent);
        }

        [Test]
        public async Task UpdateAsync_UpdatesEntity_WhenExists()
        {
            // Arrange
            var ctx = DBContext.InMemoryDbContext_Test.GetCourseManagement();
            var repo = new GenericRepository<Student>(ctx);

            var department = new Department
            {
                DepartmentId = 1,
                Name = "Software Engineering",
                Description = "SE Department"
            };
            ctx.Departments.Add(department);
            await ctx.SaveChangesAsync();

            var student = new Student
            {
                StudentId = 1,
                StudentCode = "SE170001",
                FullName = "Nguyen Van A",
                Email = "a.nguyen@fpt.edu.vn",
                DepartmentId = department.DepartmentId,
                Department = department
            };

            repo.Add(student);
            await ctx.SaveChangesAsync();

            student.FullName = "Nguyen Van B";
            student.Email = "b.nguyen@fpt.edu.vn";
            // Act
            repo.Update(student);
            await ctx.SaveChangesAsync();

            //Assert

            var updateStudent = repo.GetById(student.StudentId);
            Assert.AreEqual("Nguyen Van B", updateStudent.FullName);
            Assert.AreEqual("b.nguyen@fpt.edu.vn", updateStudent.Email);
        }


        [Test]
        public async Task UpdateAsync_AttachesAndUpdates_WhenEntityNotTracked()
        {
            var ctx = InMemoryDbContext_Test.GetCourseManagement();
            var repo = new GenericRepository<Student>(ctx);

            var dept = new Department
            {
                DepartmentId = 1,
                Name = "Software Engineering",
                Description = "SE Department"
            };
            ctx.Departments.Add(dept);
            await ctx.SaveChangesAsync();

            var student = new Student
            {
                StudentCode = "SE170001",
                FullName = "Nguyen Van A",
                Email = "a@fpt.edu.vn",
                DepartmentId = dept.DepartmentId
            };

            repo.Add(student);
            await ctx.SaveChangesAsync();

            //QUAN TRỌNG: clear tracking để entity KHÔNG còn tracked
            ctx.ChangeTracker.Clear();

            // Entity mới → NOT TRACKED
            var detachedStudent = new Student
            {
                StudentId = student.StudentId,
                StudentCode = "SE170001",
                FullName = "Nguyen Van B",
                Email = "b@fpt.edu.vn",
                DepartmentId = dept.DepartmentId
            };
            // Act
            repo.Update(detachedStudent);

            // Assert
            var updated = await ctx.Students.FindAsync(student.StudentId);
            Assert.AreEqual("Nguyen Van B", updated.FullName);
        }

        [Test]
        public async Task AddAsync_AddsEntity_AndCanBeRetrieved()
        {
            // Arrange
            var ctx = InMemoryDbContext_Test.GetCourseManagement();
            var repo = new GenericRepository<Student>(ctx);

            var dept = new Department
            {
                DepartmentId = 1,
                Name = "Software Engineering",
                Description = "SE Department"

            };
            ctx.Departments.Add(dept);
            await ctx.SaveChangesAsync();

            var student = new Student
            {
                StudentCode = "SE170001",
                FullName = "Nguyen Van A",
                Email = "a@fpt.edu.vn",
                DepartmentId = dept.DepartmentId
            };

            // Act
            repo.Add(student);
            await ctx.SaveChangesAsync();

            var savedStudent = repo.Exists(s => student.StudentId == s.StudentId);
            Assert.IsTrue(savedStudent);

            var getAll = repo.GetAll();
            Assert.IsNotNull(getAll);
        }

    }
}
