using Microsoft.EntityFrameworkCore;
using PRN222.CourseManagement.Repository.Models;
using PRN222.CourseManagement.Repository.Repository.CourseRepositroy;
using PRN222.CourseManagement.Repository.Repository.DepartmentRepository;
using PRN222.CourseManagement.Repository.Repository.EnrollmentRepository;
using PRN222.CourseManagement.Repository.Repository.StudentRepositroy;
using PRN222.CourseManagement.Repository.UnitOfWork;

namespace PRN222.CourseManagement.RepositoryTest.UnitOfWork_Test
{
    [TestFixture]
    public class UnitOfWork_Test
    {
        [Test]
        public async Task SaveChangesAsync_PersistsData()
        {
            // Arrange
            var ctx = DBContext.InMemoryDbContext_Test.GetCourseManagement();

            var uow = new UnitOfWork(
                ctx,
                new StudentRepository(ctx),
                new CourseRepository(ctx),
                new DepartmentRepository(ctx),
                new EnrollmentRepository(ctx)
            );

            var dept = new Department
            {
                Name = "SE"
            };

            // Act
            uow.departmentRepository.Add(dept);
            await uow.SaveChangeAsync();

            // Assert
            var exists = ctx.Departments.Any(d => d.Name == "SE");
            Assert.IsTrue(exists);
        }
        [Test]
        public void Dispose_Should_Not_Throw_Exception()
        {
            // Arrange
            var ctx = DBContext.InMemoryDbContext_Test.GetCourseManagement();

            var uow = new UnitOfWork(
                ctx,
                new StudentRepository(ctx),
                new CourseRepository(ctx),
                new DepartmentRepository(ctx),
                new EnrollmentRepository(ctx)
            );

            // Act & Assert
            Assert.DoesNotThrow(() => uow.Dispose());
        }
    }
}
