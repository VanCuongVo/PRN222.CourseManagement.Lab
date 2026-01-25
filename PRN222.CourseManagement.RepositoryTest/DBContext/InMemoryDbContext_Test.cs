using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PRN222.CourseManagement.Repository.Models;

namespace PRN222.CourseManagement.RepositoryTest.DBContext
{
    public class InMemoryDbContext_Test
    {
        public static CourseManagementDbContext GetCourseManagement()
        {
            var options = new DbContextOptionsBuilder<CourseManagementDbContext>()
          .UseInMemoryDatabase(Guid.NewGuid().ToString())
          .Options;
            var context = new CourseManagementDbContext(options);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            return context;
        }
    }
}
