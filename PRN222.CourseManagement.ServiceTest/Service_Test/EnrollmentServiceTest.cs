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
    public class EnrollmentServiceTest
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
        public void Create_Enrollment_Duplicated_Fail()
        {
            var enrollment = new Enrollment
            {
                StudentId = 1,
                CourseId = 1,
                EnrollDate = DateTime.Now
            };
            //BR20: student tồn tại
            _studentRepoMock
                .Setup(r => r.GetById(1))
                .Returns(new Student { StudentId = 1, DepartmentId = 1 });

            // BR20: course tồn tại
            _courseRepoMock
                .Setup(r => r.GetById(1))
                .Returns(new Course { CourseId = 1, DepartmentId = 1 });



            _enrollmentRepoMock
                .Setup(r => r.Exists(It.IsAny<Expression<Func<Enrollment, bool>>>()))
                .Returns(true);

            var result = _enrollmentService.Create(enrollment);

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(
                EnrollmentMessages.DuplicateEnrollment,
                result.Message
            );
        }

        [Test]
        public void Create_Enrollment_OverLimit_Fail()
        {
            var enrollment = new Enrollment
            {
                StudentId = 1,
                CourseId = 1,
                EnrollDate = DateTime.Now
            };

            //BR20: student tồn tại
            _studentRepoMock
                .Setup(r => r.GetById(1))
                .Returns(new Student { StudentId = 1, DepartmentId = 1 });

            // BR20: course tồn tại
            _courseRepoMock
                .Setup(r => r.GetById(1))
                .Returns(new Course { CourseId = 1, DepartmentId = 1 });

            // BR16: không trùng
            _enrollmentRepoMock
                .Setup(r => r.Exists(It.IsAny<Expression<Func<Enrollment, bool>>>()))
                .Returns(false);

            //  BR17: đã enroll 5 course
            _enrollmentRepoMock
                .Setup(r => r.Count(It.IsAny<Expression<Func<Enrollment, bool>>>()))
                .Returns(5);

            var result = _enrollmentService.Create(enrollment);

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(
                EnrollmentMessages.MaxCourseExceeded,
                result.Message
            );
        }


        [Test]
        public void Create_Enrollment_DateInPast_Fail()
        {
            var enrollment = new Enrollment
            {
                StudentId = 1,
                CourseId = 1,
                EnrollDate = DateTime.Now.AddDays(-1)
            };

            //BR20: student tồn tại
            _studentRepoMock
                .Setup(r => r.GetById(1))
                .Returns(new Student { StudentId = 1, DepartmentId = 1 });

            // BR20: course tồn tại
            _courseRepoMock
                .Setup(r => r.GetById(1))
                .Returns(new Course { CourseId = 1, DepartmentId = 1 });

            var result = _enrollmentService.Create(enrollment);

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(
                EnrollmentMessages.EnrollmentDateInPast,
                result.Message
            );
        }


        [Test]
        public void Create_Enrollment_DifferentDepartment_Fail()
        {
            var enrollment = new Enrollment
            {
                StudentId = 1,
                CourseId = 1,
                EnrollDate = DateTime.Now
            };

            _studentRepoMock
                .Setup(r => r.GetById(1))
                .Returns(new Student { StudentId = 1, DepartmentId = 1 });

            _courseRepoMock
                .Setup(r => r.GetById(1))
                .Returns(new Course { CourseId = 1, DepartmentId = 2 });

            var result = _enrollmentService.Create(enrollment);

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(
                EnrollmentMessages.DifferentDepartment,
                result.Message
            );
        }

        [Test]
        public void Create_Enrollment_StudentNotFound_Fail()
        {
            var enrollment = new Enrollment
            {
                StudentId = 1,
                CourseId = 1,
                EnrollDate = DateTime.Now
            };

            _studentRepoMock
                .Setup(r => r.GetById(1))
                .Returns((Student)null);

            var result = _enrollmentService.Create(enrollment);

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(
                EnrollmentMessages.StudentNotExist,
                result.Message
            );
        }


        [Test]
        public void Create_Enrollment_CourseNotFound_Fail()
        {
            var enrollment = new Enrollment
            {
                StudentId = 1,
                CourseId = 1,
                EnrollDate = DateTime.Now
            };

            _studentRepoMock
                .Setup(r => r.GetById(1))
                .Returns(new Student { StudentId = 1, DepartmentId = 1 });

            _courseRepoMock
                .Setup(r => r.GetById(1))
                .Returns((Course)null);

            var result = _enrollmentService.Create(enrollment);

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(
                EnrollmentMessages.CourseNotExist,
                result.Message
            );
        }

        [Test]
        public void Create_Enrollment_Success()
        {
            var enrollment = new Enrollment
            {
                StudentId = 1,
                CourseId = 1,
                EnrollDate = DateTime.Now
            };
            var transactionMock = new Mock<IDbContextTransaction>();
            _uowMock
                .Setup(u => u.BeginTransaction())
                .Returns(transactionMock.Object);
            _studentRepoMock
                .Setup(r => r.GetById(1))
                .Returns(new Student { StudentId = 1, DepartmentId = 1 });

            _courseRepoMock
                .Setup(r => r.GetById(1))
                .Returns(new Course { CourseId = 1, DepartmentId = 1 });

            _enrollmentRepoMock
                .Setup(r => r.Exists(It.IsAny<Expression<Func<Enrollment, bool>>>()))
                .Returns(false);

            _enrollmentRepoMock
                .Setup(r => r.Count(It.IsAny<Expression<Func<Enrollment, bool>>>()))
                .Returns(3);


            var result = _enrollmentService.Create(enrollment);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(
                EnrollmentMessages.Created,
                result.Message
            );
        }






    }
}
