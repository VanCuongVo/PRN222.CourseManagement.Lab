using PRN222.CourseManagement.Repository.IUnitOfWork;
using PRN222.CourseManagement.Repository.Models;
using PRN222.CourseManagement.Service.DTO.Request;
using PRN222.CourseManagement.Service.IService;
using PRN222.CourseManagement.Service.MessageHelper;

namespace PRN222.CourseManagement.Service.Service
{
    public class CourseService : ICourseService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CourseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public ServiceResult Create(Course entity)
        {
            var result = new ServiceResult();   
            try
            {
                var validateResult = ValidateCourseForCreate(entity);
                if (!validateResult.IsSuccess)
                    return validateResult;

                _unitOfWork.courseRepository.Add(entity);
                _unitOfWork.SaveChangeAsync();

                result.IsSuccess = true;
                result.Message = MessageCourse.COURSE_CREATE_SUCCESS;
                return result;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
            }
            return result;
        }

        private ServiceResult ValidateCourseForCreate(Course entity)
        {
            var result = new ServiceResult();
            if (_unitOfWork.courseRepository
     .Exists(c => c.CourseCode == entity.CourseCode))
            {
                result.IsSuccess = false;
                result.Message = MessageHelper.MessageCourse.COURSE_CODE_DUPLICATED;
                return result;

            }
            // BR12: Department exists
            if (!_unitOfWork.departmentRepository
                .Exists(d => d.DepartmentId == entity.DepartmentId))
            {
                result.IsSuccess = false;
                result.Message = MessageHelper.MessageCourse.COURSE_DEPARTMENT_REQUIRED;
                return result;
            }

            // BR13: Credits between 1 and 6
            if (entity.Credits < 1 || entity.Credits > 6)
            {
                result.IsSuccess = false;
                result.Message = MessageHelper.MessageCourse.COURSE_CREDITS_INVALID;
                return result;
            }

            return result;
        }

        public ServiceResult Delete(int id)
        {
            var result = new ServiceResult();
            try
            {
                var validateResult = ValidateCourseForDelete(id);
                if (!validateResult.IsSuccess)
                    return validateResult;

                _unitOfWork.courseRepository.Delete(id);
                _unitOfWork.SaveChangeAsync();
                result.IsSuccess = true;
                result.Message = MessageCourse.COURSE_DELETE_SUCCESS;
                return result;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
            }
            return result;
        }


        private ServiceResult ValidateCourseForDelete(int courseId)
        {
            var result = new ServiceResult();
            // BR14: Cannot delete if students enrolled
            bool hasEnrollment = _unitOfWork.enrollementRepository
                .Exists(e => e.CourseId == courseId);

            if (hasEnrollment)
            {
                result.IsSuccess = true;
                result.Message = MessageHelper.MessageCourse.COURSE_HAS_ENROLLMENTS;
                return result;
            }

            return result;
        }

        public IEnumerable<Course> GetAll()
        {
            return _unitOfWork.courseRepository.GetAll();
        }

        public ServiceResult Update(Course entity)
        {
            var result = new ServiceResult();
            try
            {
                var validateResult = ValidateCourseForUpdate(entity);
                if (!validateResult.IsSuccess)
                    return validateResult;

                var course = _unitOfWork.courseRepository
                    .GetById(entity.CourseId);

                if (course == null)
                {
                    result.IsSuccess = false;
                    result.Message = MessageHelper.MessageCourse.COURSE_NOT_FOUND;
                    return result;
                }

                // Update fields
                course.Credits = entity.Credits;
                course.DepartmentId = entity.DepartmentId;

                _unitOfWork.courseRepository.Update(course);
                _unitOfWork.SaveChangeAsync();

                result.IsSuccess = true;
                result.Message = MessageHelper.MessageCourse.COURSE_UPDATE_SUCCESS;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
            }
            return result;
        }

        private ServiceResult ValidateCourseForUpdate(Course entity)
        {
            var result = new ServiceResult();
            // BR11: CourseCode unique (exclude itself)
            bool duplicatedCode = _unitOfWork.courseRepository
                .Exists(c => c.CourseId != entity.CourseId);

            if (duplicatedCode)
            {
                result.IsSuccess = false;
                result.Message = MessageHelper.MessageCourse.COURSE_NOT_FOUND;
                return result;
            }

            // BR12: Department exists
            if (!_unitOfWork.departmentRepository
                .Exists(d => d.DepartmentId == entity.DepartmentId))
            {
                result.IsSuccess = false;
                result.Message = MessageHelper.MessageCourse.COURSE_DEPARTMENT_REQUIRED;
                return result;
            }

            // BR13: Credits between 1 and 6
            if (entity.Credits < 1 || entity.Credits > 6)
            {
                result.IsSuccess = false;
                result.Message = MessageHelper.MessageCourse.COURSE_CREDITS_INVALID;
                return result;
            }

            return result;

        }
    }
}
