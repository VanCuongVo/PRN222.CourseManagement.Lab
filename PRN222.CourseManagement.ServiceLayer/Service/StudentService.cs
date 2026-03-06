using System.Diagnostics.CodeAnalysis;
using PRN222.CourseManagement.Repository.IUnitOfWork;
using PRN222.CourseManagement.Repository.Models;
using PRN222.CourseManagement.Service.DTO.Request;
using PRN222.CourseManagement.Service.IService;
using PRN222.CourseManagement.Service.MessageHelper;

namespace PRN222.CourseManagement.Service.Service
{
    public class StudentService : IStudentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public StudentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResult> Create(Student entity)
        {
            var result = new ServiceResult();
            try
            {
                var validateResult = ValidationStudentForCreate(entity);
                if (!validateResult.IsSuccess)
                {
                    return validateResult;
                }
                _unitOfWork.studentRepository.Add(entity);
                await _unitOfWork.SaveChangeAsync();

                result.IsSuccess = true;
                result.Message = MessageStudent.STUDENT_CREATE_SUCCESS;
                return result;

            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.IsSuccess = false;
            }
            return result;
        }

        private ServiceResult ValidationStudentForCreate(Student entity)
        {
            var result = new ServiceResult();
            var isExist = _unitOfWork.studentRepository.Exists(s => s.StudentCode == entity.StudentCode);
            if (isExist)
            {
                result.IsSuccess = false;
                result.Message = MessageStudent.STUDENT_CODE_DUPLICATED;
                return result;
            }

            if (string.IsNullOrWhiteSpace(entity.FullName))
            {
                result.IsSuccess = false;
                result.Message = MessageStudent.STUDENT_FULLNAME_REQUIRED;
                return result;
            }

            if (entity.FullName.Trim().Length < 3)
            {
                result.IsSuccess = false;
                result.Message = MessageHelper.MessageStudent.STUDENT_FULLNAME_MIN_LENGTH;
                return result;
            }

            var isExistByEmail = _unitOfWork.studentRepository.Exists(s => s.Email == entity.Email);
            if (isExistByEmail)
            {
                result.IsSuccess = false;
                result.Message = MessageStudent.STUDENT_EMAIL_DUPLICATED;
                return result;
            }

            bool departmentExists = _unitOfWork.departmentRepository.Exists(d => d.DepartmentId == entity.DepartmentId);

            if (!departmentExists)
            {
                result.IsSuccess = false;
                result.Message = MessageStudent.STUDENT_DEPARTMENT_REQUIRED;
                return result;
            }
            return result;


        }

        public async Task<ServiceResult> Delete(int id)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                var validateResult = ValidationStudentForDelete(id);
                if (!validateResult.IsSuccess)
                {
                    return validateResult;

                }
                _unitOfWork.studentRepository.Delete(id);
                await _unitOfWork.SaveChangeAsync();
                result.IsSuccess = true;
                result.Message = MessageStudent.STUDENT_DELETE_SUCCESS;
                return result;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
            }
            return result;

        }

        private ServiceResult ValidationStudentForDelete(int id)
        {
            var result = new ServiceResult();

            // 1. Check student tồn tại
            var student = _unitOfWork.studentRepository.GetById(id);
            if (student == null)
            {
                result.IsSuccess = false;
                result.Message = MessageStudent.STUDENT_NOT_FOUND;
                return result;
            }

            // 2. Check có enrollment
            bool hasEnrollment = _unitOfWork.enrollementRepository
                .Exists(e => e.StudentId == id);

            if (hasEnrollment)
            {
                result.IsSuccess = false;
                result.Message = MessageStudent.STUDENT_HAS_ENROLLMENTS;
                return result;
            }

            return result;
        }

        [ExcludeFromCodeCoverage]
        public IEnumerable<Student> GetAll()
        {
            return _unitOfWork.studentRepository.GetAll();
        }

        [ExcludeFromCodeCoverage]
        public Student? GetById(int id)
        {
            return _unitOfWork.studentRepository.GetById(id);
        }

        [ExcludeFromCodeCoverage]
        public async Task<ServiceResult> Update(Student entity)
        {
            var result = new ServiceResult();
            try
            {
                var validateResult = ValidateStudentForUpdate(entity);
                if (!validateResult.IsSuccess)
                    return validateResult;

                // Check if student exists using Exists (no tracking)
                bool exists = _unitOfWork.studentRepository.Exists(s => s.StudentId == entity.StudentId);
                if (!exists)
                {
                    result.IsSuccess = false;
                    result.Message = MessageStudent.STUDENT_NOT_FOUND;
                    return result;
                }

                // Update the entity directly
                _unitOfWork.studentRepository.Update(entity);
                await _unitOfWork.SaveChangeAsync();

                result.IsSuccess = true;
                result.Message = MessageStudent.STUDENT_UPDATE_SUCCESS;
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
        private ServiceResult ValidateStudentForUpdate(Student entity)
        {
            var result = new ServiceResult();

            // Check StudentCode duplicate (exclude current student)
            bool duplicatedCode = _unitOfWork.studentRepository
                .Exists(s => s.StudentCode == entity.StudentCode && s.StudentId != entity.StudentId);
            if (duplicatedCode)
            {
                result.IsSuccess = false;
                result.Message = MessageStudent.STUDENT_CODE_DUPLICATED;
                return result;
            }

            if (string.IsNullOrWhiteSpace(entity.FullName))
            {
                result.IsSuccess = false;
                result.Message = MessageStudent.STUDENT_FULLNAME_REQUIRED;
                return result;
            }

            if (entity.FullName.Trim().Length < 3)
            {
                result.IsSuccess = false;
                result.Message = MessageHelper.MessageStudent.STUDENT_FULLNAME_MIN_LENGTH;
                return result;
            }

            // Check Email duplicate (exclude current student)
            var isExistByEmail = _unitOfWork.studentRepository
                .Exists(s => s.Email == entity.Email && s.StudentId != entity.StudentId);
            if (isExistByEmail)
            {
                result.IsSuccess = false;
                result.Message = MessageStudent.STUDENT_EMAIL_DUPLICATED;
                return result;
            }

            bool departmentExists = _unitOfWork.departmentRepository.Exists(d => d.DepartmentId == entity.DepartmentId);

            if (!departmentExists)
            {
                result.IsSuccess = false;
                result.Message = MessageStudent.STUDENT_DEPARTMENT_REQUIRED;
                return result;
            }
            return result;
        }
    }
}
