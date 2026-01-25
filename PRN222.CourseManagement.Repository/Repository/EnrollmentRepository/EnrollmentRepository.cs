using System.Diagnostics.CodeAnalysis;
using PRN222.CourseManagement.Repository.IRepository.IEnrollmentRepository;
using PRN222.CourseManagement.Repository.Models;
using PRN222.CourseManagement.Repository.Repository.GenericRepository;

namespace PRN222.CourseManagement.Repository.Repository.EnrollmentRepository
{
    [ExcludeFromCodeCoverage]
    public class EnrollmentRepository : GenericRepository<Enrollment>, IEnrollementRepository
    {
        public EnrollmentRepository(CourseManagementDbContext courseManagementDbContext) : base(courseManagementDbContext)
        {
        }
    }
}
