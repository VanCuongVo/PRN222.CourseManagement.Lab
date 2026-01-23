using Microsoft.EntityFrameworkCore.Storage;
using PRN222.CourseManagement.Repository.IRepository.ICourseRepository;
using PRN222.CourseManagement.Repository.IRepository.IDepartmentRepository;
using PRN222.CourseManagement.Repository.IRepository.IEnrollmentRepository;
using PRN222.CourseManagement.Repository.IRepository.StudentRepository;

namespace PRN222.CourseManagement.Repository.IUnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        Task SaveChangeAsync();
        IStudentRepository studentRepository { get; set; }
        IEnrollementRepository enrollementRepository { get; set; }
        IDepartmentRepository departmentRepository { get; set; }
        ICourseRepository courseRepository { get; set; }

        IDbContextTransaction BeginTransaction();
    }
}
