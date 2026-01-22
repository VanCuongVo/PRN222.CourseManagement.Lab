using PRN222.CourseManagement.Repository.Models;
using PRN222.CourseManagement.Repository.UnitOfWork;
using Repositroy_Test.Persitence_Test.Context_Test;

namespace PRN222.CourseManagement.Test.Repositories_Test
{
    public class EnrollmentTests : IDisposable
    {
        private CourseManagementDbContext _context;
        private UnitOfWork _uow;
        private Department _department;
        private Student _student;
        private Course _course;
        private Enrollment Enrollment;

        public EnrollmentTests()
        {
            // Tạo In-Memory Database context mới cho mỗi test
            _context = InMemoryDbContext_Test.GetInMemoryDbContext();
            //_uow = new UnitOfWork(_context);

            // Tạo dữ liệu chuẩn bị
            _department = new Department { Name = "IT", Description = "Information Technology" };
            _context.Departments.Add(_department);
            _context.SaveChanges();

            _student = new Student
            {
                StudentCode = "SE001",
                FullName = "John Doe",
                Email = "john@example.com",
                DepartmentId = _department.DepartmentId
            };
            _context.Students.Add(_student);

            _course = new Course
            {
                CourseCode = "PRN222",
                Title = "Advanced Programming",
                Credits = 3,
                DepartmentId = _department.DepartmentId
            };
            _context.Courses.Add(_course);
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context?.Dispose();
        }

        [Fact]
        public void AddEnrollment_Should_Succeed_With_Valid_Data()
        {
            // Arrange
            var enrollmentDate = DateTime.Now;
            var enrollment = new Enrollment
            {
                StudentId = _student.StudentId,
                CourseId = _course.CourseId,
                EnrollDate = enrollmentDate,
                Grade = null
            };

            // Act
            _uow.enrollementRepository.Add(enrollment);
            _uow.SaveChangeAsync();

            // Assert
            var savedEnrollment = _uow.enrollementRepository.GetAll()
                .FirstOrDefault(e => e.StudentId == _student.StudentId && e.CourseId == _course.CourseId);

            Assert.NotNull(savedEnrollment);
            Assert.Equal(_student.StudentId, savedEnrollment.StudentId);
            Assert.Equal(_course.CourseId, savedEnrollment.CourseId);
            Assert.Equal(enrollmentDate.Date, savedEnrollment.EnrollDate.Date);
            Assert.Null(savedEnrollment.Grade);
        }

        [Fact]
        public void GetAllEnrollments_Should_Return_All_Enrollments()
        {
            // Arrange
            var student2 = new Student
            {
                StudentCode = "SE002",
                FullName = "Jane Smith",
                Email = "jane@example.com",
                DepartmentId = _department.DepartmentId
            };
            _context.Students.Add(student2);

            var course2 = new Course
            {
                CourseCode = "PRN231",
                Title = "Web Development",
                Credits = 3,
                DepartmentId = _department.DepartmentId
            };
            _context.Courses.Add(course2);
            _context.SaveChanges();

            var enrollment1 = new Enrollment
            {
                StudentId = _student.StudentId,
                CourseId = _course.CourseId,
                EnrollDate = DateTime.Now,
                Grade = 8.5m
            };

            var enrollment2 = new Enrollment
            {
                StudentId = student2.StudentId,
                CourseId = _course.CourseId,
                EnrollDate = DateTime.Now,
                Grade = 9.0m
            };

            var enrollment3 = new Enrollment
            {
                StudentId = _student.StudentId,
                CourseId = course2.CourseId,
                EnrollDate = DateTime.Now,
                Grade = 7.5m
            };

            _uow.enrollementRepository.Add(enrollment1);
            _uow.enrollementRepository.Add(enrollment2);
            _uow.enrollementRepository.Add(enrollment3);
            _uow.SaveChangeAsync();

            // Act
            var allEnrollments = _uow.enrollementRepository.GetAll().ToList();

            // Assert
            Assert.Equal(3, allEnrollments.Count);
            Assert.Contains(allEnrollments, e => e.StudentId == _student.StudentId && e.CourseId == _course.CourseId);
            Assert.Contains(allEnrollments, e => e.StudentId == student2.StudentId && e.CourseId == _course.CourseId);
            Assert.Contains(allEnrollments, e => e.StudentId == _student.StudentId && e.CourseId == course2.CourseId);
        }

        [Fact]
        public void UpdateEnrollmentGrade_Should_Update_Successfully()
        {
            // Arrange
            var enrollment = new Enrollment
            {
                StudentId = _student.StudentId,
                CourseId = _course.CourseId,
                EnrollDate = DateTime.Now,
                Grade = null
            };
            _uow.enrollementRepository.Add(enrollment);
            _uow.SaveChangeAsync();

            // Act
            var enrollmentToUpdate = _uow.enrollementRepository.GetAll()
                .FirstOrDefault(e => e.StudentId == _student.StudentId && e.CourseId == _course.CourseId);

            Assert.NotNull(enrollmentToUpdate);
            enrollmentToUpdate.Grade = 8.75m;
            _uow.enrollementRepository.Update(enrollmentToUpdate);
            _uow.SaveChangeAsync();

            // Assert
            var updatedEnrollment = _uow.enrollementRepository.GetAll()
                .FirstOrDefault(e => e.StudentId == _student.StudentId && e.CourseId == _course.CourseId);

            Assert.NotNull(updatedEnrollment);
            Assert.Equal(8.75m, updatedEnrollment.Grade);
        }

        [Fact]
        public void DeleteEnrollment_Should_Remove_Successfully()
        {
            // Arrange
            var enrollment = new Enrollment
            {
                StudentId = _student.StudentId,
                CourseId = _course.CourseId,
                EnrollDate = DateTime.Now,
                Grade = 8.5m
            };
            _uow.enrollementRepository.Add(enrollment);
            _uow.SaveChangeAsync();

            var enrollmentCount = _uow.enrollementRepository.GetAll().Count();
            Assert.Equal(1, enrollmentCount);

            // Act
            // Enrollment có composite key (StudentId, CourseId), nên phải tìm rồi xóa
            var enrollmentToDelete = _uow.enrollementRepository.GetAll()
                .FirstOrDefault(e => e.StudentId == _student.StudentId && e.CourseId == _course.CourseId);

            if (enrollmentToDelete != null)
            {
                _context.Enrollments.Remove(enrollmentToDelete);
                _uow.SaveChangeAsync();
            }

            // Assert
            var remainingEnrollments = _uow.enrollementRepository.GetAll().ToList();
            Assert.Empty(remainingEnrollments);
        }

        [Fact]
        public void GetEnrollmentsByStudent_Should_Return_All_Student_Enrollments()
        {
            // Arrange
            var course2 = new Course
            {
                CourseCode = "PRN231",
                Title = "Web Development",
                Credits = 3,
                DepartmentId = _department.DepartmentId
            };
            var course3 = new Course
            {
                CourseCode = "PRN301",
                Title = "Database Design",
                Credits = 4,
                DepartmentId = _department.DepartmentId
            };
            _context.Courses.AddRange(course2, course3);
            _context.SaveChanges();

            var enrollment1 = new Enrollment
            {
                StudentId = _student.StudentId,
                CourseId = _course.CourseId,
                EnrollDate = DateTime.Now,
                Grade = 8.5m
            };
            var enrollment2 = new Enrollment
            {
                StudentId = _student.StudentId,
                CourseId = course2.CourseId,
                EnrollDate = DateTime.Now,
                Grade = 9.0m
            };
            var enrollment3 = new Enrollment
            {
                StudentId = _student.StudentId,
                CourseId = course3.CourseId,
                EnrollDate = DateTime.Now,
                Grade = 7.5m
            };

            _uow.enrollementRepository.Add(enrollment1);
            _uow.enrollementRepository.Add(enrollment2);
            _uow.enrollementRepository.Add(enrollment3);
            _uow.SaveChangeAsync();

            // Act
            var studentEnrollments = _uow.enrollementRepository.GetAll()
                .Where(e => e.StudentId == _student.StudentId)
                .ToList();

            // Assert
            Assert.Equal(3, studentEnrollments.Count);
            Assert.All(studentEnrollments, e => Assert.Equal(_student.StudentId, e.StudentId));
            Assert.Contains(studentEnrollments, e => e.CourseId == _course.CourseId);
            Assert.Contains(studentEnrollments, e => e.CourseId == course2.CourseId);
            Assert.Contains(studentEnrollments, e => e.CourseId == course3.CourseId);
        }

        [Fact]
        public void GetEnrollmentsByCourse_Should_Return_All_Course_Enrollments()
        {
            // Arrange
            var student2 = new Student
            {
                StudentCode = "SE002",
                FullName = "Jane Smith",
                Email = "jane@example.com",
                DepartmentId = _department.DepartmentId
            };
            var student3 = new Student
            {
                StudentCode = "SE003",
                FullName = "Bob Johnson",
                Email = "bob@example.com",
                DepartmentId = _department.DepartmentId
            };
            _context.Students.AddRange(student2, student3);
            _context.SaveChanges();

            var enrollment1 = new Enrollment
            {
                StudentId = _student.StudentId,
                CourseId = _course.CourseId,
                EnrollDate = DateTime.Now,
                Grade = 8.5m
            };
            var enrollment2 = new Enrollment
            {
                StudentId = student2.StudentId,
                CourseId = _course.CourseId,
                EnrollDate = DateTime.Now,
                Grade = 9.0m
            };
            var enrollment3 = new Enrollment
            {
                StudentId = student3.StudentId,
                CourseId = _course.CourseId,
                EnrollDate = DateTime.Now,
                Grade = 7.5m
            };

            _uow.enrollementRepository.Add(enrollment1);
            _uow.enrollementRepository.Add(enrollment2);
            _uow.enrollementRepository.Add(enrollment3);
            _uow.SaveChangeAsync();

            // Act
            var courseEnrollments = _uow.enrollementRepository.GetAll()
                .Where(e => e.CourseId == _course.CourseId)
                .ToList();

            // Assert
            Assert.Equal(3, courseEnrollments.Count);
            Assert.All(courseEnrollments, e => Assert.Equal(_course.CourseId, e.CourseId));
            Assert.Contains(courseEnrollments, e => e.StudentId == _student.StudentId);
            Assert.Contains(courseEnrollments, e => e.StudentId == student2.StudentId);
            Assert.Contains(courseEnrollments, e => e.StudentId == student3.StudentId);
        }

        [Fact]
        public void EnrollmentWithGrade_Should_Store_Grade_Correctly()
        {
            // Arrange
            var enrollment = new Enrollment
            {
                StudentId = _student.StudentId,
                CourseId = _course.CourseId,
                EnrollDate = DateTime.Now,
                Grade = 9.5m
            };

            // Act
            _uow.enrollementRepository.Add(enrollment);
            _uow.SaveChangeAsync();

            // Assert
            var savedEnrollment = _uow.enrollementRepository.GetAll()
                .FirstOrDefault(e => e.StudentId == _student.StudentId && e.CourseId == _course.CourseId);

            Assert.NotNull(savedEnrollment);
            Assert.Equal(9.5m, savedEnrollment.Grade);
        }

        [Fact]
        public void EnrollmentWithoutGrade_Should_Have_Null_Grade()
        {
            // Arrange
            var enrollment = new Enrollment
            {
                StudentId = _student.StudentId,
                CourseId = _course.CourseId,
                EnrollDate = DateTime.Now,
                Grade = null
            };

            // Act
            _uow.enrollementRepository.Add(enrollment);
            _uow.SaveChangeAsync();

            // Assert
            var savedEnrollment = _uow.enrollementRepository.GetAll()
                .FirstOrDefault(e => e.StudentId == _student.StudentId && e.CourseId == _course.CourseId);

            Assert.NotNull(savedEnrollment);
            Assert.Null(savedEnrollment.Grade);
        }
    }
}