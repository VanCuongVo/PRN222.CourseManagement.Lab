using System.Linq.Expressions;
using Moq;
using PRN222.CourseManagement.Repository.IRepository.IDepartmentRepository;
using PRN222.CourseManagement.Repository.IRepository.IEnrollmentRepository;
using PRN222.CourseManagement.Repository.IRepository.StudentRepository;
using PRN222.CourseManagement.Repository.IUnitOfWork;
using PRN222.CourseManagement.Repository.Models;
using PRN222.CourseManagement.Service.MessageHelper;
using PRN222.CourseManagement.Service.Service;

namespace PRN222.CourseManagement.ServiceTest.Service_Test
{
    [TestFixture]
    public class StudentServiceTest
    {
        private Mock<IUnitOfWork> _uowMock;
        private Mock<IStudentRepository> _studentRepoMock;
        private Mock<IDepartmentRepository> _deptRepoMock;
        private Mock<IEnrollementRepository> _enrollmentRepoMock;
        private StudentService _studentService;
        [SetUp]
        public void Setup()
        {
            _deptRepoMock = new Mock<IDepartmentRepository>();
            _studentRepoMock = new Mock<IStudentRepository>();
            _enrollmentRepoMock = new Mock<IEnrollementRepository>();
            _uowMock = new Mock<IUnitOfWork>();

            _uowMock.Setup(u => u.departmentRepository).Returns(_deptRepoMock.Object);

            _uowMock.Setup(u => u.studentRepository).Returns(_studentRepoMock.Object);
            _uowMock.Setup(u => u.enrollementRepository).Returns(_enrollmentRepoMock.Object);

            _studentService = new StudentService(_uowMock.Object);

        }

        [Test]
        public async Task Create_StudentCodeDuplicated_Fail()
        {
            // Arrange
            var student = new Student
            {
                StudentCode = "S001",
                FullName = "Nguyen Van A",
                DepartmentId = 1
            };
            _studentRepoMock
        .Setup(u =>
             u.Exists(It.IsAny<Expression<Func<Student, bool>>>()))
            .Returns(true);
            // Act
            var result = await _studentService.Create(student);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(Service.MessageHelper.MessageStudent.STUDENT_CODE_DUPLICATED, result.Message);

        }


        [Test]
        public async Task Create_FullNameEmpty_Fail()
        {
            var student = new Student
            {
                StudentCode = "S002",
                FullName = "",
                DepartmentId = 1
            };

            _studentRepoMock.Setup(u =>
                u.Exists(It.IsAny<Expression<Func<Student, bool>>>()))
                .Returns(false);

            var result = await _studentService.Create(student);

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(
                MessageStudent.STUDENT_FULLNAME_REQUIRED,
                result.Message
            );
        }

        [Test]
        public async Task Create_FullNameTooShort_Fail()
        {
            var student = new Student
            {
                StudentCode = "S003",
                FullName = "An",
                DepartmentId = 1
            };

            _studentRepoMock.Setup(u =>
                u.Exists(It.IsAny<Expression<Func<Student, bool>>>()))
                .Returns(false);

            var result = await _studentService.Create(student);

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(
                MessageStudent.STUDENT_FULLNAME_MIN_LENGTH,
                result.Message
            );
        }

        [Test]
        public async Task Create_EmailDuplicated_Fail()
        {
            var student = new Student
            {
                StudentCode = "S004",
                FullName = "Nguyen Van B",
                Email = "a@gmail.com",
                DepartmentId = 1
            };

            _studentRepoMock.SetupSequence(u =>
                u.Exists(It.IsAny<Expression<Func<Student, bool>>>()))
                .Returns(false) // StudentCode
                .Returns(true); // Email

            var result = await _studentService.Create(student);

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(
                MessageStudent.STUDENT_EMAIL_DUPLICATED,
                result.Message
            );
        }


        [Test]
        public async Task Create_DepartmentNotExist_Fail()
        {
            var student = new Student
            {
                StudentCode = "S005",
                FullName = "Nguyen Van C",
                DepartmentId = 99
            };

            _studentRepoMock.Setup(u =>
                u.Exists(It.IsAny<Expression<Func<Student, bool>>>()))
                .Returns(false);

            _deptRepoMock.Setup(u =>
                u.Exists(It.IsAny<Expression<Func<Department, bool>>>()))
                .Returns(false);

            var result = await _studentService.Create(student);

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(
                MessageStudent.STUDENT_DEPARTMENT_REQUIRED,
                result.Message
            );
        }

        [Test]
        public async Task Create_Student_SuccessFull()
        {
            // Arrange
            var student = new Student
            {
                StudentCode = "S001",
                FullName = "Nguyen Van A",
                DepartmentId = 1
            };

            // StudentCode + Email không trùng
            _studentRepoMock
                .Setup(r => r.Exists(It.IsAny<Expression<Func<Student, bool>>>()))
                .Returns(false);

            // Department tồn tại
            _deptRepoMock
                .Setup(r => r.Exists(It.IsAny<Expression<Func<Department, bool>>>()))
                .Returns(true);

            // Act
            var result = await _studentService.Create(student);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(
                MessageStudent.STUDENT_CREATE_SUCCESS,
                result.Message
            );
        }

        [Test]
        public async Task Delete_Student_HasEnrollments_Fail()
        {
            // Arrange
            int studentId = 1;
            _studentRepoMock
                   .Setup(r => r.GetById(studentId))
                   .Returns(new Student { StudentId = studentId });

            _enrollmentRepoMock
                .Setup(r => r.Exists(It.IsAny<Expression<Func<Enrollment, bool>>>()))
                .Returns(true); // có enrollment


            // Act
            var result = await _studentService.Delete(studentId);
            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(
                MessageStudent.STUDENT_HAS_ENROLLMENTS,
                result.Message
            );
        }

        [Test]
        public async Task Delete_Student_NoEnrollments_Success()
        {
            int studentId = 1;
            _studentRepoMock
             .Setup(r => r.GetById(studentId))
             .Returns(new Student { StudentId = studentId });

            _enrollmentRepoMock
                .Setup(r => r.Exists(It.IsAny<Expression<Func<Enrollment, bool>>>()))
                .Returns(false); // không có enrollment
            var result = await _studentService.Delete(studentId);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(
                MessageStudent.STUDENT_DELETE_SUCCESS,
                result.Message
            );
        }
    }
}
