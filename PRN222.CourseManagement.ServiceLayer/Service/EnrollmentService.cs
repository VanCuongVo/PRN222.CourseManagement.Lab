using PRN222.CourseManagement.Repository.IUnitOfWork;
using PRN222.CourseManagement.Repository.Models;
using PRN222.CourseManagement.Service.DTO.Request;
using PRN222.CourseManagement.Service.IService;

namespace PRN222.CourseManagement.Service.Service
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public EnrollmentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        private ServiceResult ValidateEnrollment(Enrollment entity)
        {
            var result = new ServiceResult(); // default IsSuccess = true

            // BR20: Student tồn tại
            var student = _unitOfWork.studentRepository.GetById(entity.StudentId);
            if (student == null)
            {
                result.IsSuccess = false;
                result.Message = MessageHelper.EnrollmentMessages.StudentNotExist;
                return result;
            }

            // BR20: Course tồn tại
            var course = _unitOfWork.courseRepository.GetById(entity.CourseId);
            if (course == null)
            {
                result.IsSuccess = false;
                result.Message = MessageHelper.EnrollmentMessages.CourseNotExist;
                return result;
            }

            // BR16: Không enroll trùng course
            var isDuplicated = _unitOfWork.enrollementRepository.Exists(e =>
                e.StudentId == entity.StudentId &&
                e.CourseId == entity.CourseId);

            if (isDuplicated)
            {
                result.IsSuccess = false;
                result.Message = MessageHelper.EnrollmentMessages.DuplicateEnrollment;
                return result;
            }

            // BR17: Tối đa 5 course
            var enrolledCount = _unitOfWork.enrollementRepository
                .Count(e => e.StudentId == entity.StudentId);

            if (enrolledCount >= 5)
            {
                result.IsSuccess = false;
                result.Message = MessageHelper.EnrollmentMessages.MaxCourseExceeded;
                return result;
            }

            // BR18: Ngày enroll không được ở quá khứ
            if (entity.EnrollDate.Date < DateTime.Now.Date)
            {
                result.IsSuccess = false;
                result.Message = MessageHelper.EnrollmentMessages.EnrollmentDateInPast;
                return result;
            }

            // BR19: Cùng department
            if (student.DepartmentId != course.DepartmentId)
            {
                result.IsSuccess = false;
                result.Message = MessageHelper.EnrollmentMessages.DifferentDepartment;
                return result;
            }

            return result;
        }


        public ServiceResult Create(Enrollment entity)
        {
            var result = new ServiceResult();
            using var transaction = _unitOfWork.BeginTransaction(); // BR24

            try
            {
                var validate = ValidateEnrollment(entity);
                if (!validate.IsSuccess)
                {
                    return validate;
                }

                _unitOfWork.enrollementRepository.Add(entity);
                _unitOfWork.SaveChangeAsync(); // nếu async → nên await

                transaction.Commit(); // commit SAU khi save OK

                result.IsSuccess = true;
                result.Message = MessageHelper.EnrollmentMessages.Created;
                return result;
            }
            catch
            {
                transaction.Rollback();

                result.IsSuccess = false;
                result.Message = MessageHelper.EnrollmentMessages.TransactionFailed; // BR25
                return result;
            }
        }

        public ServiceResult Delete(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Enrollment> GetAll()
        {
          return _unitOfWork.enrollementRepository.GetAll();    
        }

        public ServiceResult Update(Enrollment entity)
        {
            var result = new ServiceResult();
            using var transaction = _unitOfWork.BeginTransaction(); // BR24

            try
            {
                var validate = ValidateEnrollmentForUpdate(entity);
                if (!validate.IsSuccess)
                {
                    return validate;
                }

                _unitOfWork.enrollementRepository.Update(entity);
                _unitOfWork.SaveChangeAsync();

                transaction.Commit();

                result.IsSuccess = true;
                result.Message = MessageHelper.EnrollmentMessages.Updated;
                return result;
            }
            catch
            {
                transaction.Rollback();

                result.IsSuccess = false;
                result.Message = MessageHelper.EnrollmentMessages.TransactionFailed;
                return result;
            }
        }


        private ServiceResult ValidateEnrollmentForUpdate(Enrollment entity)
        {
            var result = new ServiceResult();

            // BR21 – Enrollment phải tồn tại
            var existing = _unitOfWork.enrollementRepository.Exists(e =>
                e.StudentId == entity.StudentId &&
                e.CourseId == entity.CourseId);

            if (!existing)
            {
                result.IsSuccess = false;
                result.Message = MessageHelper.EnrollmentMessages.NotFound;
                return result;
            }

            // BR18 – Không cho update enroll date về quá khứ
            if (entity.EnrollDate.Date < DateTime.Today)
            {
                result.IsSuccess = false;
                result.Message = MessageHelper.EnrollmentMessages.EnrollmentDateInPast;
                return result;
            }

            // BR23 – Nếu đã có grade → không cho update enrollment
            var enrollment = _unitOfWork.enrollementRepository
                .Exists(e => e.StudentId == entity.StudentId && e.CourseId == entity.CourseId);

            if (!enrollment)
            {
                result.IsSuccess = false;
                result.Message = MessageHelper.EnrollmentMessages.GradeFinalized;
                return result;
            }

            result.IsSuccess = true;
            return result;
        }

    }
}
