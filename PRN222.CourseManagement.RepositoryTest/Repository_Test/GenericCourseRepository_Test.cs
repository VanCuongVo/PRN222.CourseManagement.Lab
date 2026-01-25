using PRN222.CourseManagement.Repository.Models;
namespace PRN222.CourseManagement.RepositoryTest.Repository_Test
{
    public class GenericCourseRepository_Test
    {
        [Test]
        public async Task DeleteAsync_RemovesEntity_WhenExists()
        {
            // Arrange
            var ctx = DBContext.InMemoryDbContext_Test.GetCourseManagement();
            var repo = new Repository.Repository.GenericRepository.GenericRepository<Course>(ctx);

            var dept = new Department
            {
                Name = "Software Engineering",
                Description = "SE Dept"
            };

            ctx.Departments.Add(dept);
            await ctx.SaveChangesAsync();

            var course = new Course
            {
                CourseCode = "PRN222",
                Title = "C# with .NET",
                Credits = 3,
                DepartmentId = dept.DepartmentId
            };

            repo.Add(course);
            await ctx.SaveChangesAsync();

            var courseId = course.CourseId;

            // Act
            repo.Delete(courseId);
            await ctx.SaveChangesAsync();
            // Assert
            var exists = repo.Exists(c => c.CourseId == courseId);
            Assert.IsFalse(exists);
        }


        [Test]
        public void Add_AddsEntity_WhenValid()
        {
            // Arrange
            var ctx = DBContext.InMemoryDbContext_Test.GetCourseManagement();
            var repo = new Repository.Repository.GenericRepository.GenericRepository<Course>(ctx);

            var dept = new Department
            {
                Name = "Software Engineering",
                Description = "SE Dept"
            };

            ctx.Departments.Add(dept);
            ctx.SaveChanges();

            var course = new Course
            {
                CourseCode = "PRN222",
                Title = "C# with .NET",
                Credits = 3,
                DepartmentId = dept.DepartmentId
            };

            // Act
            repo.Add(course);
            ctx.SaveChanges();

            // Assert
            var exists = repo.Exists(c => c.CourseId == course.CourseId);
            Assert.IsTrue(exists);

            
        }



        [Test]
        public void Update_UpdatesEntity_WhenExists()
        {
            // Arrange
            var ctx = DBContext.InMemoryDbContext_Test.GetCourseManagement();
            var repo = new Repository.Repository.GenericRepository.GenericRepository<Course>(ctx);

            var dept = new Department
            {
                Name = "Software Engineering",
                Description = "SE Dept"
            };

            ctx.Departments.Add(dept);
            ctx.SaveChanges();

            var course = new Course
            {
                CourseCode = "PRN222",
                Title = "C# with .NET",
                Credits = 3,
                DepartmentId = dept.DepartmentId
            };

            repo.Add(course);
            ctx.SaveChanges();

            // modify
            course.Title = "Advanced .NET";
            course.Credits = 4;

            // Act
            repo.Update(course);
            ctx.SaveChanges();

            // Assert
            var updatedCourse =repo.GetById(course.CourseId);

            Assert.IsNotNull(updatedCourse);
            Assert.AreEqual("Advanced .NET", updatedCourse!.Title);
            Assert.AreEqual(4, updatedCourse.Credits);
        }
    }
}


