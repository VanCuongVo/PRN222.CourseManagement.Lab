using System.Diagnostics.CodeAnalysis;
using PRN222.CourseManagement.Repository.IUnitOfWork;
using PRN222.CourseManagement.Repository.Models;
using PRN222.CourseManagement.Service.DTO.Request;
using PRN222.CourseManagement.Service.IService;

namespace PRN222.CourseManagement.Service.Service
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DepartmentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResult> Create(Department entity)
        {
            var result = new ServiceResult();
            try
            {
                var validate = ValidationCreateDeparment(entity);
                if (!validate.IsSuccess)
                {
                    return validate;
                }
                _unitOfWork.departmentRepository.Add(entity);
                await _unitOfWork.SaveChangeAsync();
                result.IsSuccess = true;
                result.Message = MessageHelper.DepartmentMessages.Created;
                return result;

            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
            }
            return result;
        }

        private ServiceResult ValidationCreateDeparment(Department entity)
        {
            var result = new ServiceResult();
            var departmentName = _unitOfWork.departmentRepository.Exists(s => s.Name == entity.Name);
            if (departmentName)
            {
                result.IsSuccess = false;
                result.Message = MessageHelper.DepartmentMessages.NameExists;
                return result;
            }

            if (string.IsNullOrWhiteSpace(entity.Name))
            {
                result.IsSuccess = false;
                result.Message = MessageHelper.DepartmentMessages.NameEmpty;
                return result;
            }

            if (entity.Name.Trim().Length < 3)
            {
                result.IsSuccess = false;
                result.Message = MessageHelper.DepartmentMessages.NameTooShort;
                return result;
            }

            return result;
        }

        public async Task<ServiceResult> Delete(int id)
        {
            var result = new ServiceResult();
            try
            {
                var validate = ValidationDeleteDepartment(id);
                if (!validate.IsSuccess)
                {
                    return validate;
                }
                _unitOfWork.departmentRepository.Delete(id);
                await _unitOfWork.SaveChangeAsync();

                result.IsSuccess = true;
                result.Message = MessageHelper.DepartmentMessages.Deleted;
                return result;

            }
            catch (Exception ex)
            {

                result.IsSuccess = false;
                result.Message = ex.Message;
            }
            return result;
        }

        private ServiceResult ValidationDeleteDepartment(int id)
        {
            var result = new ServiceResult();
            var department = _unitOfWork.departmentRepository.GetById(id);
            if (department == null)
            {
                result.IsSuccess = false;
                result.Message = MessageHelper.DepartmentMessages.NotFound;
                return result;
            }

            // 2. Check có student
            var hasStudents = _unitOfWork.studentRepository
                .Exists(s => s.DepartmentId == id);

            if (hasStudents)
            {
                result.IsSuccess = false;
                result.Message = MessageHelper.DepartmentMessages.HasStudents;
                return result;
            }

            // 3. Check có course
            var hasCourses = _unitOfWork.courseRepository
                .Exists(c => c.DepartmentId == id);

            if (hasCourses)
            {
                result.IsSuccess = false;
                result.Message = MessageHelper.DepartmentMessages.HasCourses;
                return result;
            }


            return result;

        }
        [ExcludeFromCodeCoverage]
        public IEnumerable<Department> GetAll()
        {
            return _unitOfWork.departmentRepository.GetAll();
        }

        [ExcludeFromCodeCoverage]
        public Department? GetById(int id)
        {
            return _unitOfWork.departmentRepository.GetById(id);
        }

        [ExcludeFromCodeCoverage]
        public async Task<ServiceResult> Update(Department entity)
        {
            var result = new ServiceResult();
            try
            {
                var validate = ValidationUpdateDeparment(entity);
                if (!validate.IsSuccess)
                {
                    return validate;
                }
                // Lấy entity gốc từ DB
                var department = _unitOfWork.departmentRepository.GetById(entity.DepartmentId);
                if (department == null)
                {
                    return new ServiceResult
                    {
                        IsSuccess = false,
                        Message = MessageHelper.DepartmentMessages.NotFound
                    };
                }
                // Update field cần thiết
                department.Name = entity.Name.Trim();
                department.Description = entity.Description.Trim();

                _unitOfWork.departmentRepository.Update(entity);
                await _unitOfWork.SaveChangeAsync();
                result.IsSuccess = true;
                result.Message = MessageHelper.DepartmentMessages.Updated;
                return result;

            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
            }
            return result;
        }
        [ExcludeFromCodeCoverage]
        private ServiceResult ValidationUpdateDeparment(Department entity)
        {
            var result = new ServiceResult();

            var department = _unitOfWork.departmentRepository.GetById(entity.DepartmentId);
            if (department == null)
            {
                return new ServiceResult
                {
                    IsSuccess = false,
                    Message = MessageHelper.DepartmentMessages.NotFound
                };
            }

            if (string.IsNullOrWhiteSpace(entity.Name))
            {
                result.IsSuccess = false;
                result.Message = MessageHelper.DepartmentMessages.NameEmpty;
                return result;
            }

            if (entity.Name.Trim().Length < 3)
            {
                result.IsSuccess = false;
                result.Message = MessageHelper.DepartmentMessages.NameTooShort;
                return result;
            }

            // Check trùng tên (trừ chính nó)
            var isExist = _unitOfWork.departmentRepository.Exists(d =>
                d.Name == entity.Name.Trim() && d.DepartmentId != entity.DepartmentId);

            if (isExist)
            {
                result.IsSuccess = false;
                result.Message = MessageHelper.DepartmentMessages.NameExists;
                return result;
            }

            return result;

        }

    }
}
