using PRN222.CourseManagement.Repository.IRepository.ICourseRepository;
using PRN222.CourseManagement.Repository.Models;
using PRN222.CourseManagement.Repository.Repository.GenericRepository;

namespace PRN222.CourseManagement.Repository.Repository.CourseRepositroy
{
    public class CourseRepository : GenericRepository<Course>, ICourseRepository
    {
        public CourseRepository(CourseManagementDbContext courseManagementDbContext) : base(courseManagementDbContext)
        {
        }
    }
}
