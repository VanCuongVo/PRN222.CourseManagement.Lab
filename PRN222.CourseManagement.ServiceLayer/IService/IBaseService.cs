using PRN222.CourseManagement.Service.DTO.Request;

namespace PRN222.CourseManagement.Service.IService
{
    public interface IBaseService<T>
    {
        Task<ServiceResult> Create(T entity);
        Task<ServiceResult> Update(T entity);
        Task<ServiceResult> Delete(int id);
        IEnumerable<T> GetAll();
        T? GetById(int id);
    }
}
