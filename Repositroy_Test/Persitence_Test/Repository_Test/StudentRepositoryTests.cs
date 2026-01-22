using PRN222.CourseManagement.Repository.Models;
using PRN222.CourseManagement.Repository.Repository.GenericRepository;
using Repositroy_Test.Persitence_Test.Context_Test;

namespace PRN222.CourseManagement.Test.Repositories_Test
{
    public class StudentRepositoryTests
    {
        [Fact]
        public void GetAll_ShouldReturnAllStudents()
        {
            // 1. Arrange (Chuẩn bị)
            var context = InMemoryDbContext_Test.GetInMemoryDbContext();
            InMemoryDbContext_Test.SeedData(context);

            // Khởi tạo Repo với Context ảo
            var studentRepo = new GenericRepository<Student>(context);

            // 2. Act (Thực hiện hành động)
            var result = studentRepo.GetAll().ToList();

            // 3. Assert (Kiểm tra kết quả)
            Assert.NotNull(result);
            Assert.Equal(2, result.Count()); // Vì trong SeedData mình tạo 2 sinh viên
            Assert.Contains(result, s => s.FullName == "Test Student A");
            Assert.Contains(result, s => s.FullName == "Test Student B");
        }

        [Fact]
        public void Add_ShouldAddStudentSuccessfully()
        {
            // 1. Arrange
            var context = InMemoryDbContext_Test.GetInMemoryDbContext();
            var studentRepo = new GenericRepository<Student>(context);

            var newStudent = new Student
            {
                StudentCode = "SE99",
                FullName = "New Guy",
                Email = "newguy@example.com",
                DepartmentId = 1
            };

            // 2. Act
            studentRepo.Add(newStudent);
            context.SaveChanges(); // Generic Repo của bạn tách hàm Save ra UnitOfWork, nhưng test lẻ thì gọi context.SaveChanges tạm

            // 3. Assert
            var dbStudent = context.Students.FirstOrDefault(s => s.StudentCode == "SE99");
            Assert.NotNull(dbStudent);
            Assert.Equal("New Guy", dbStudent.FullName);
        }
    }
}