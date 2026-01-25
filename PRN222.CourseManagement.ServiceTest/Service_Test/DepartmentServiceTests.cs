using System.Linq.Expressions;
using Moq;
using PRN222.CourseManagement.Repository.IRepository.ICourseRepository;
using PRN222.CourseManagement.Repository.IRepository.IDepartmentRepository;
using PRN222.CourseManagement.Repository.IRepository.StudentRepository;
using PRN222.CourseManagement.Repository.IUnitOfWork;
using PRN222.CourseManagement.Repository.Models;
using PRN222.CourseManagement.Service.Service;

namespace PRN222.CourseManagement.ServiceTest.Service_Test
{
    public class DepartmentServiceTests
    {
        private Mock<IUnitOfWork> _uowMock;
        private Mock<IDepartmentRepository> _deptRepoMock;
        private Mock<IStudentRepository> _studentRepoMock;
        private Mock<ICourseRepository> _courseRepoMock;

        private DepartmentService _service;

        [SetUp]
        public void Setup()
        {
            _courseRepoMock = new Mock<ICourseRepository>();
            _deptRepoMock = new Mock<IDepartmentRepository>();
            _studentRepoMock = new Mock<IStudentRepository>();
            _uowMock = new Mock<IUnitOfWork>();

            _uowMock.Setup(u => u.departmentRepository).Returns(_deptRepoMock.Object);
            _uowMock.Setup(u => u.courseRepository).Returns(_courseRepoMock.Object);
            _uowMock.Setup(u => u.studentRepository).Returns(_studentRepoMock.Object);

            _service = new DepartmentService(_uowMock.Object);
        }

        [Test]
        public void TC01_Create_Department_With_Duplicate_Name_Should_Fail()
        {
            // Arrange
            var dept = new Department { Name = "SE" };
            _deptRepoMock
        .Setup(r => r.Exists(It.IsAny<Expression<Func<Department, bool>>>()))
        .Returns(true);

            // Act
            var result = _service.Create(dept);
            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(Service.MessageHelper.DepartmentMessages.NameExists, result.Message);

            _deptRepoMock.Verify(r => r.Add(It.IsAny<Department>()), Times.Never);
        }

        [Test]
        public void TC02_Create_Department_With_Short_Name_Should_Fail()
        {
            // Arrange
            var dept = new Department { Name = "IT" }; // < 3 ký tự

            _deptRepoMock
      .Setup(r => r.Exists(It.IsAny<Expression<Func<Department, bool>>>()))
      .Returns(false);

            // Act
            var result = _service.Create(dept);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(Service.MessageHelper.DepartmentMessages.NameTooShort, result.Message);

            _deptRepoMock.Verify(r => r.Add(It.IsAny<Department>()), Times.Never);
        }

        [Test]
        public void Delete_Department_With_Students_Should_Fail()
        {
            // Arrange
            int deptId = 1;

            _deptRepoMock
                .Setup(r => r.GetById(deptId))
                .Returns(new Department { DepartmentId = deptId });

            _studentRepoMock
                .Setup(r => r.Exists(It.IsAny<Expression<Func<Student, bool>>>()))
                .Returns(true);

            // Act
            var result = _service.Delete(deptId);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(
                Service.MessageHelper.DepartmentMessages.HasStudents,
                result.Message
            );

            _deptRepoMock.Verify(r => r.Delete(It.IsAny<int>()), Times.Never);
        }


        [Test]
        public void TC04_Delete_Department_With_Courses_Should_Fail()
        {
            // Arrange
            int deptId = 2;

            _deptRepoMock
                .Setup(r => r.GetById(deptId))
                .Returns(new Department { DepartmentId = deptId });

            _studentRepoMock
              .Setup(r => r.Exists(It.IsAny<Expression<Func<Student, bool>>>())).Returns(false);

            _courseRepoMock
              .Setup(r => r.Exists(It.IsAny<Expression<Func<Course, bool>>>())).Returns(true);

            // Act
            var result = _service.Delete(deptId);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(Service.MessageHelper.DepartmentMessages.HasCourses, result.Message);

            _deptRepoMock.Verify(r => r.Delete(It.IsAny<int>()), Times.Never);
        }


        [Test]
        public void Delete_Department_Not_Found_Should_Fail()
        {
            _deptRepoMock
                .Setup(r => r.GetById(It.IsAny<int>()))
                .Returns((Department)null);

            var result = _service.Delete(99);

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(
                Service.MessageHelper.DepartmentMessages.NotFound,
                result.Message
            );
        }



        [Test]
        public void Create_Department_Valid_Should_Succeed()
        {
            var dept = new Department { Name = "Software Engineering" };

            _deptRepoMock
                .Setup(r => r.Exists(It.IsAny<Expression<Func<Department, bool>>>()))
        .Returns(false);


            var result = _service.Create(dept);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(Service.MessageHelper.DepartmentMessages.Created, result.Message);

            _deptRepoMock.Verify(r => r.Add(dept), Times.Once);
            _uowMock.Verify(u => u.SaveChangeAsync(), Times.Once);
        }
    }
}
