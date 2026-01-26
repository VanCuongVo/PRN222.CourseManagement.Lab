using PRN222.CourseManagement.Repository.Models;

namespace PRN222.CourseManagement.RepositoryTest.Repository_Test
{
    [TestFixture]
    public class GenericDepartmentRepository_Test
    {
        [Test]
        public async Task DeleteAsync_RemovesEntity_WhenExists()
        {
            // Arrange
            var ctx = DBContext.InMemoryDbContext_Test.GetCourseManagement();
            var repo = new Repository.Repository.GenericRepository.GenericRepository<Department>(ctx);

            var dept = new Department
            {
                Name = "Software Engineering",
                Description = "SE Department"
            };
            repo.Add(dept);
            await ctx.SaveChangesAsync();

            var deptId = dept.DepartmentId;

            // Act
            repo.Delete(deptId);
            await ctx.SaveChangesAsync();

            //Assert

            var exists = repo.Exists(d => d.DepartmentId == deptId);
            Assert.IsFalse(exists);
        }


        [Test]
        public async Task UpdateAsync_UpdatesEntity_WhenExists()
        {
            // Arrange
            var ctx = DBContext.InMemoryDbContext_Test.GetCourseManagement();
            var repo = new Repository.Repository.GenericRepository.GenericRepository<Department>(ctx);

            var dept = new Department
            {
                Name = "Software Engineering",
                Description = "Old description"
            };

            repo.Add(dept);
            await ctx.SaveChangesAsync();

            // modify data
            dept.Name = "Information Technology";
            dept.Description = "Updated description";

            // Act
            repo.Update(dept);

            // Assert
            var updatedDept = repo.GetById(dept.DepartmentId);

            Assert.IsNotNull(updatedDept);
            Assert.AreEqual("Information Technology", updatedDept!.Name);
            Assert.AreEqual("Updated description", updatedDept.Description);
        }
        [Test]
        public async Task AddAsync_AddsEntity_AndCanBeRetrieved()
        {
            // Arrange
            var ctx = DBContext.InMemoryDbContext_Test.GetCourseManagement();
            var repo = new Repository.Repository.GenericRepository.GenericRepository<Department>(ctx);

            var dept = new Department
            {
                Name = "Software Engineering",
                Description = "SE Department"
            };

            // Act
            repo.Add(dept);

            // Assert
            var savedDept = repo.GetById(dept.DepartmentId);

            Assert.IsNotNull(savedDept);
            Assert.AreEqual("Software Engineering", savedDept!.Name);
            Assert.AreEqual("SE Department", savedDept.Description);

            var getAll = repo.GetAll();
            Assert.IsNotNull(getAll);
        }
    }
}
