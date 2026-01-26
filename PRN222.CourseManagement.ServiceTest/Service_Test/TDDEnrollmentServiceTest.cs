using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using PRN222.CourseManagement.Repository.IRepository.ICourseRepository;
using PRN222.CourseManagement.Repository.IRepository.IEnrollmentRepository;
using PRN222.CourseManagement.Repository.IRepository.StudentRepository;
using PRN222.CourseManagement.Repository.IUnitOfWork;
using PRN222.CourseManagement.Repository.Models;
using PRN222.CourseManagement.Service.MessageHelper;
using PRN222.CourseManagement.Service.Service;

namespace PRN222.CourseManagement.ServiceTest.Service_Test
{
    [TestFixture]
    public class EnrollmentServiceV2Test
    {
        private Mock<IUnitOfWork> _uowMock;
        private Mock<IEnrollementRepository> _enrollmentRepoMock;
        private Mock<IStudentRepository> _studentRepoMock;
        private Mock<ICourseRepository> _courseRepoMock;


        private EnrollmentService _enrollmentService;
        [SetUp]
        public void Setup()
        {
            _uowMock = new Mock<IUnitOfWork>();
            _enrollmentRepoMock = new Mock<IEnrollementRepository>();
            _studentRepoMock = new Mock<IStudentRepository>();
            _courseRepoMock = new Mock<ICourseRepository>();
            var transactionMock = new Mock<IDbContextTransaction>();

            _uowMock.Setup(u => u.enrollementRepository)
                    .Returns(_enrollmentRepoMock.Object);

            _uowMock.Setup(u => u.studentRepository)
                    .Returns(_studentRepoMock.Object);

            _uowMock.Setup(u => u.courseRepository)
                    .Returns(_courseRepoMock.Object);

            _uowMock
    .Setup(u => u.BeginTransaction())
    .Returns(transactionMock.Object);

            _enrollmentService = new EnrollmentService(_uowMock.Object);
        }

        [Test]
        public void TC26_Enroll_Student_Under_18_Should_Fail()
        {
            // Arrange
            var student = new Student
            {
                DateOfBirth = DateTime.Today.AddYears(-17),
                IsActive = true,

            };

            var course = new Course
            {
                Credits = 3,
                IsActive = true,
            };
            var enrollment = new Enrollment
            {
                StudentId = 1,
                CourseId = 1,
                Student = student,
                Course = course,
                EnrollDate = DateTime.Now
            };

            var transactionMock = new Mock<IDbContextTransaction>();

            transactionMock.Setup(t => t.Commit());
            transactionMock.Setup(t => t.Rollback());
            transactionMock.Setup(t => t.Dispose());

            _uowMock
                .Setup(u => u.BeginTransaction())
                .Returns(transactionMock.Object);
            _studentRepoMock.Setup(r => r.GetById(1))
                .Returns(student);

            _courseRepoMock
              .Setup(r => r.GetById(1))
              .Returns(course);


            _enrollmentRepoMock
                .Setup(r => r.Exists(It.IsAny<Expression<Func<Enrollment, bool>>>()))
                .Returns(false);


            _enrollmentRepoMock
                .Setup(r => r.Count(It.IsAny<Expression<Func<Enrollment, bool>>>()))
                .Returns(3);

            //Act 
            var result = _enrollmentService.Create(enrollment);

            //Assert

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Student must be at least 18 years old", result.Message);
        }

        [Test]
        public void TC27_Enroll_Course_With_Zero_Credit_Should_Fail()
        {
            //Arrange
            var student = new Student
            {
                DateOfBirth = DateTime.Today.AddYears(18),
                IsActive = true,

            };
            var course = new Course
            {
                Credits = 0,
                IsActive = true,
            };

            var enrollment = new Enrollment
            {
                StudentId = 1,
                CourseId = 1,
                Student = student,
                Course = course,
                EnrollDate = DateTime.Now
            };

            var transactionMock = new Mock<IDbContextTransaction>();
            transactionMock.Setup(t => t.Commit());
            transactionMock.Setup(t => t.Rollback());
            transactionMock.Setup(t => t.Dispose());


            _uowMock
               .Setup(u => u.BeginTransaction())
               .Returns(transactionMock.Object);
            _studentRepoMock.Setup(r => r.GetById(1))
                .Returns(student);

            _courseRepoMock
              .Setup(r => r.GetById(1))
              .Returns(course);


            _enrollmentRepoMock
                .Setup(r => r.Exists(It.IsAny<Expression<Func<Enrollment, bool>>>()))
                .Returns(false);

            //Act
            var result = _enrollmentService.Create(enrollment);


            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Course must have at least 1 credit", result.Message);
        }

        [Test]
        public void TC28_Enroll_Inactive_Course_Should_Fail()
        {
            //Arrange
            var student = new Student
            {
                DateOfBirth = DateTime.Today.AddYears(18),
                IsActive = true,

            };
            var course = new Course
            {
                Credits = 3,
                IsActive = false,
            };

            var enrollment = new Enrollment
            {
                StudentId = 1,
                CourseId = 1,
                Student = student,
                Course = course,
                EnrollDate = DateTime.Now
            };

            var transactionMock = new Mock<IDbContextTransaction>();
            transactionMock.Setup(t => t.Commit());
            transactionMock.Setup(t => t.Rollback());
            transactionMock.Setup(t => t.Dispose());


            _uowMock
               .Setup(u => u.BeginTransaction())
               .Returns(transactionMock.Object);
            _studentRepoMock.Setup(r => r.GetById(1))
                .Returns(student);

            _courseRepoMock
              .Setup(r => r.GetById(1))
              .Returns(course);


            _enrollmentRepoMock
                .Setup(r => r.Exists(It.IsAny<Expression<Func<Enrollment, bool>>>()))
                .Returns(false);

            //Act
            var result = _enrollmentService.Create(enrollment);


            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Course is inactive", result.Message);
        }


        [Test]
        public void TC29_Enroll_Inactive_Student_Should_Fail()
        {
            //Arrange
            var student = new Student
            {
                DateOfBirth = DateTime.Today.AddYears(18),
                IsActive = false,

            };
            var course = new Course
            {
                Credits = 2,
                IsActive = true,
            };

            var enrollment = new Enrollment
            {
                StudentId = 1,
                CourseId = 1,
                Student = student,
                Course = course,
                EnrollDate = DateTime.Now
            };

            var transactionMock = new Mock<IDbContextTransaction>();
            transactionMock.Setup(t => t.Commit());
            transactionMock.Setup(t => t.Rollback());
            transactionMock.Setup(t => t.Dispose());


            _uowMock
               .Setup(u => u.BeginTransaction())
               .Returns(transactionMock.Object);
            _studentRepoMock.Setup(r => r.GetById(1))
                .Returns(student);

            _courseRepoMock
              .Setup(r => r.GetById(1))
              .Returns(course);


            _enrollmentRepoMock
                .Setup(r => r.Exists(It.IsAny<Expression<Func<Enrollment, bool>>>()))
                .Returns(false);

            //Act
            var result = _enrollmentService.Create(enrollment);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Student is inactive", result.Message);

        }
        [Test]
        public void TC30_Assign_Grade_Outside_Grading_Period_Should_Fail()
        {
            //Arrange
            var student = new Student
            {
                DateOfBirth = DateTime.Today.AddYears(19),
                IsActive = true,

            };
            var course = new Course
            {
                Credits = 2,
                IsActive = true,
            };

            var enrollment = new Enrollment
            {
                StudentId = 1,
                CourseId = 1,
                Student = student,
                Course = course,
                EnrollDate = DateTime.Today.AddDays(32)
            };

            var transactionMock = new Mock<IDbContextTransaction>();
            transactionMock.Setup(t => t.Commit());
            transactionMock.Setup(t => t.Rollback());
            transactionMock.Setup(t => t.Dispose());


            _uowMock
               .Setup(u => u.BeginTransaction())
               .Returns(transactionMock.Object);
            _studentRepoMock.Setup(r => r.GetById(1))
                .Returns(student);

            _courseRepoMock
              .Setup(r => r.GetById(1))
              .Returns(course);


            _enrollmentRepoMock
                .Setup(r => r.Exists(It.IsAny<Expression<Func<Enrollment, bool>>>()))
                .Returns(false);

            //Act
            var result = _enrollmentService.Create(enrollment);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(EnrollmentMessages.GradingPeriodExpired, result.Message);

        }

    }
}
