using PRN222.CourseManagement.Service.DTO.Request;

namespace PRN222.CourseManagement.Service.IService
{
    public interface IBaseService<T>
    {
        ServiceResult Create(T entity);
        ServiceResult Update(T entity);
        ServiceResult Delete(int id);
        IEnumerable<T> GetAll();
    }
}
