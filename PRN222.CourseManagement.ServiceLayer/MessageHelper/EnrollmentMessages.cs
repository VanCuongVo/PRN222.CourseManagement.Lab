namespace PRN222.CourseManagement.Service.MessageHelper
{
    public class EnrollmentMessages
    {
        // Common
        public const string NotFound = "Enrollment not found.";
        public const string Created = "Enrollment created successfully.";
        public const string Failed = "Enrollment operation failed.";

        // BR16
        public const string DuplicateEnrollment =
            "Student has already enrolled in this course.";

        // BR17
        public const string MaxCourseExceeded =
            "Student cannot enroll in more than 5 courses.";

        // BR18
        public const string EnrollmentDateInPast =
            "Enrollment date cannot be in the past.";

        // BR19
        public const string DifferentDepartment =
            "Student can only enroll in courses of the same department.";

        // BR20
        public const string StudentNotExist =
            "Student does not exist.";

        public const string CourseNotExist =
            "Course does not exist.";

        // BR24
        public const string TransactionFailed =
            "Enrollment transaction failed. All changes were rolled back.";

        public const string GradeFinalized =
    "Grade has been finalized and cannot be updated.";

        public const string Updated =
    "Enrollment updated successfully.";

        public const string StudentUnder18 = "Student must be at least 18 years old";
        public const string CourseHasNoCredit = "Course must have at least 1 credit";
        public const string CourseInactive = "Course is inactive";
        public const string StudentInactive = "Student is inactive";
        public const string InvalidGrade = "Enrollment grade must be between 0 and 10";

        public const string GradingPeriodExpired = "Grading period expired";

    }
}
