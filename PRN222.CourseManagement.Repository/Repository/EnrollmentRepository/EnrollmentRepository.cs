using PRN222.CourseManagement.Repository.IRepository.IEnrollmentRepository;
using PRN222.CourseManagement.Repository.Models;
using PRN222.CourseManagement.Repository.Repository.GenericRepository;

namespace PRN222.CourseManagement.Repository.Repository.EnrollmentRepository
{
    public class EnrollmentRepository : GenericRepository<Enrollment>, IEnrollementRepository
    {
        public EnrollmentRepository(CourseManagementDbContext courseManagementDbContext) : base(courseManagementDbContext)
        {
        }
    }
}
