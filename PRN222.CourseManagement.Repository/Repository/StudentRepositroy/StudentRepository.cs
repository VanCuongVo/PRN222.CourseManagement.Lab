using System.Diagnostics.CodeAnalysis;
using PRN222.CourseManagement.Repository.IRepository.StudentRepository;
using PRN222.CourseManagement.Repository.Models;
using PRN222.CourseManagement.Repository.Repository.GenericRepository;

namespace PRN222.CourseManagement.Repository.Repository.StudentRepositroy
{
    [ExcludeFromCodeCoverage]
    public class StudentRepository : GenericRepository<Student>, IStudentRepository
    {
        public StudentRepository(CourseManagementDbContext courseManagementDbContext) : base(courseManagementDbContext)
        {
        }
    }
}
