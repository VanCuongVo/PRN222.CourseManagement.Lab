using System.Linq.Expressions;

namespace PRN222.CourseManagement.Repository.IRepository.IGenericRepository
{
    public interface IGenericRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        T? GetById(object id);
        void Add(T entity);
        void Update(T entity);
        void Delete(object id);

        bool Exists(Expression<Func<T, bool>> predicate);
    }
}
