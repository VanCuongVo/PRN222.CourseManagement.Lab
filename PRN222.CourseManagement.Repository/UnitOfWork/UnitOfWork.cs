using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PRN222.CourseManagement.Repository.IRepository.ICourseRepository;
using PRN222.CourseManagement.Repository.IRepository.IDepartmentRepository;
using PRN222.CourseManagement.Repository.IRepository.IEnrollmentRepository;
using PRN222.CourseManagement.Repository.IRepository.StudentRepository;
using PRN222.CourseManagement.Repository.Models;

namespace PRN222.CourseManagement.Repository.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork.IUnitOfWork
    {
        public readonly CourseManagementDbContext _courseManagementDbContext;

        public IStudentRepository studentRepository { get; set; }
        public IEnrollementRepository enrollementRepository { get; set; }
        public IDepartmentRepository departmentRepository { get; set; }
        public ICourseRepository courseRepository { get; set; }

        public UnitOfWork(CourseManagementDbContext courseManagementDbContext, IStudentRepository _studentRepository, ICourseRepository _courseRepository, IDepartmentRepository _departmentRepository, IEnrollementRepository _enrollementRepository)
        {
            _courseManagementDbContext = courseManagementDbContext;
            studentRepository = _studentRepository;
            courseRepository = _courseRepository;
            enrollementRepository = _enrollementRepository;
            departmentRepository = _departmentRepository;
        }
        public void Dispose()
        {
            _courseManagementDbContext.Dispose();
        }

        public async Task SaveChangeAsync()
        {
            await _courseManagementDbContext.SaveChangesAsync();
        }

        public void SaveChanges()
        {
            _courseManagementDbContext.SaveChanges();
        }

        [ExcludeFromCodeCoverage]
        public IDbContextTransaction BeginTransaction()
        {
            return _courseManagementDbContext.Database.BeginTransaction();
        }

    }
}
