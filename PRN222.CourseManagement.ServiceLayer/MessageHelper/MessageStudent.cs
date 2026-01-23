namespace PRN222.CourseManagement.Service.MessageHelper
{
    public class MessageStudent
    {
        // BR05. StudentCode must be unique
        public const string STUDENT_CODE_DUPLICATED =
            "Student code must be unique.";

        // BR06. Student must belong to exactly one department
        public const string STUDENT_DEPARTMENT_REQUIRED =
            "Student must belong to exactly one department.";

        // BR07. Student full name cannot be null or empty
        public const string STUDENT_FULLNAME_REQUIRED =
            "Student full name cannot be null or empty.";

        // BR08. Student full name must be at least 3 characters
        public const string STUDENT_FULLNAME_MIN_LENGTH =
            "Student full name must be at least 3 characters.";

        // BR09. Student email (if provided) must be unique
        public const string STUDENT_EMAIL_DUPLICATED =
            "Student email must be unique.";

        // BR10. A student cannot be deleted if the student has enrollments
        public const string STUDENT_HAS_ENROLLMENTS =
            "Cannot delete student because the student has enrollments.";

        public const string STUDENT_CREATE_SUCCESS =
    "Student created successfully.";

        public const string STUDENT_DELETE_SUCCESS =
    "Student deleted successfully.";

        public const string STUDENT_NOT_FOUND =
    "Student not found.";

        public const string STUDENT_UPDATE_SUCCESS =
        "Student updated successfully.";

    }
}
