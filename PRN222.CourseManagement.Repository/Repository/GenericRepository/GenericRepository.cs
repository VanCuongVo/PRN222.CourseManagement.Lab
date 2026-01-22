using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using PRN222.CourseManagement.Repository.IRepository.IGenericRepository;
using PRN222.CourseManagement.Repository.Models;

namespace PRN222.CourseManagement.Repository.Repository.GenericRepository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        public readonly CourseManagementDbContext _courseManagementDbContext;

        public GenericRepository(CourseManagementDbContext courseManagementDbContext)
        {
            _courseManagementDbContext = courseManagementDbContext;
        }

        public void Add(T entity)
        {
            _courseManagementDbContext.Set<T>().AddAsync(entity);
        }

        public void Delete(object id)
        {
            var existingEntity = GetById(id);
            if (existingEntity != null)
            {
                _courseManagementDbContext.Remove(existingEntity);
            }
        }

        public IEnumerable<T> GetAll()
        {

            return _courseManagementDbContext.Set<T>().ToList();
        }

        public T? GetById(object id)
        {
            return _courseManagementDbContext.Set<T>().Find(id);

        }

        public void Update(T entity)
        {
            _courseManagementDbContext.Set<T>().Update(entity);
        }

        public bool Exists(Expression<Func<T, bool>> predicate)
        {
            return _courseManagementDbContext.Set<T>().Any(predicate);
        }
    }
}
