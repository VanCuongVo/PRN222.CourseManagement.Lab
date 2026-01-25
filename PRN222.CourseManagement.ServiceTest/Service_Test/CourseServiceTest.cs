using System.Linq.Expressions;
using Moq;
using PRN222.CourseManagement.Repository.IRepository.ICourseRepository;
using PRN222.CourseManagement.Repository.IRepository.IDepartmentRepository;
using PRN222.CourseManagement.Repository.IRepository.IEnrollmentRepository;
using PRN222.CourseManagement.Repository.IRepository.StudentRepository;
using PRN222.CourseManagement.Repository.IUnitOfWork;
using PRN222.CourseManagement.Repository.Models;
using PRN222.CourseManagement.Service.IService;
using PRN222.CourseManagement.Service.MessageHelper;
using PRN222.CourseManagement.Service.Service;

namespace PRN222.CourseManagement.ServiceTest.Service_Test
{
    public class CourseServiceTest
    {
        private Mock<IUnitOfWork> _uowMock;
        private Mock<ICourseRepository> _courseRepoMock;
        private Mock<IDepartmentRepository> _deptRepoMock;
        private Mock<IEnrollementRepository> _enrollmentRepoMock;
        private CourseService _courseService;

        [SetUp]
        public void Setup()
        {
            _deptRepoMock = new Mock<IDepartmentRepository>();
            _courseRepoMock = new Mock<ICourseRepository>();
            _enrollmentRepoMock = new Mock<IEnrollementRepository>();
            _uowMock = new Mock<IUnitOfWork>();

            _uowMock.Setup(u => u.departmentRepository).Returns(_deptRepoMock.Object);

            _uowMock.Setup(u => u.courseRepository).Returns(_courseRepoMock.Object);
            _uowMock.Setup(u => u.enrollementRepository).Returns(_enrollmentRepoMock.Object);

            _courseService = new CourseService(_uowMock.Object);
        }

        [Test]
        public void Create_CourseCodeDuplicated_Fail()
        {
            var course = new Course
            {
                CourseCode = "C001",
                Credits = 3,
                DepartmentId = 1
            };

            _courseRepoMock
                .Setup(r => r.Exists(It.IsAny<Expression<Func<Course, bool>>>()))
                .Returns(true);

            var result = _courseService.Create(course);

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(
                MessageCourse.COURSE_CODE_DUPLICATED,
                result.Message
            );
        }


        [Test]
        public void Create_Course_DepartmentNotExist_Fail()
        {
            var course = new Course
            {
                CourseCode = "C002",
                Credits = 3,
                DepartmentId = 99
            };

            _courseRepoMock
                .Setup(r => r.Exists(It.IsAny<Expression<Func<Course, bool>>>()))
                .Returns(false);

            _deptRepoMock
                .Setup(r => r.Exists(It.IsAny<Expression<Func<Department, bool>>>()))
                .Returns(false);

            var result = _courseService.Create(course);

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(
                MessageCourse.COURSE_DEPARTMENT_REQUIRED,
                result.Message
            );
        }

        [Test]
        public void Create_Course_InvalidCredits_Fail()
        {
            var course = new Course
            {
                CourseCode = "C003",
                Credits = 7,
                DepartmentId = 1
            };

            _courseRepoMock
                .Setup(r => r.Exists(It.IsAny<Expression<Func<Course, bool>>>()))
                .Returns(false);

            _deptRepoMock.Setup(r => r.Exists(It.IsAny<Expression<Func<Department, bool>>>()))
                .Returns(true);
            var result = _courseService.Create(course);

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(
                MessageCourse.COURSE_CREDITS_INVALID,
                result.Message
            );
        }

        [Test]
        public void Create_Course_Success()
        {
            var course = new Course
            {
                CourseCode = "C004",
                Credits = 3,
                DepartmentId = 1
            };

            _courseRepoMock
                .Setup(r => r.Exists(It.IsAny<Expression<Func<Course, bool>>>()))
                .Returns(false);

            _deptRepoMock
                .Setup(r => r.Exists(It.IsAny<Expression<Func<Department, bool>>>()))
                .Returns(true);

            var result = _courseService.Create(course);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(
                MessageCourse.COURSE_CREATE_SUCCESS,
                result.Message
            );
        }

        [Test]
        public void Delete_Course_HasEnrollments_Fail()
        {
            int courseId = 1;

            _courseRepoMock
        .Setup(r => r.Exists(It.IsAny<Expression<Func<Course, bool>>>()))
        .Returns(true);

            _enrollmentRepoMock
                .Setup(r => r.Exists(It.IsAny<Expression<Func<Enrollment, bool>>>()))
                .Returns(true);

            var result = _courseService.Delete(courseId);

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(
                MessageCourse.COURSE_HAS_ENROLLMENTS,
                result.Message
            );
        }

        [Test]
        public void Delete_Courrse_HasEnrollments_Success()
        {
            int courseId = 1;

            _courseRepoMock
        .Setup(r => r.Exists(It.IsAny<Expression<Func<Course, bool>>>()))
        .Returns(true);

            _enrollmentRepoMock
                .Setup(r => r.Exists(It.IsAny<Expression<Func<Enrollment, bool>>>()))
                .Returns(false);

            var result = _courseService.Delete(courseId);

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(
                MessageCourse.COURSE_DELETE_SUCCESS,
                result.Message
            );
        }
    }
}
