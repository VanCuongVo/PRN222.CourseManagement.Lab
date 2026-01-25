using System.Diagnostics.CodeAnalysis;
using PRN222.CourseManagement.Repository.IRepository.IDepartmentRepository;
using PRN222.CourseManagement.Repository.Models;
using PRN222.CourseManagement.Repository.Repository.GenericRepository;

namespace PRN222.CourseManagement.Repository.Repository.DepartmentRepository
{
    [ExcludeFromCodeCoverage]
    public class DepartmentRepository : GenericRepository<Department>, IDepartmentRepository
    {
        public DepartmentRepository(CourseManagementDbContext courseManagementDbContext) : base(courseManagementDbContext)
        {
        }
    }
}
