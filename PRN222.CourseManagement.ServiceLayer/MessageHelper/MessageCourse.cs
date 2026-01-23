using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN222.CourseManagement.Service.MessageHelper
{
    public interface MessageCourse
    {
        // ===== SUCCESS =====
        public const string COURSE_CREATE_SUCCESS =
            "Course created successfully.";

        public const string COURSE_UPDATE_SUCCESS =
            "Course updated successfully.";

        public const string COURSE_DELETE_SUCCESS =
            "Course deleted successfully.";

        // ===== NOT FOUND =====
        public const string COURSE_NOT_FOUND =
            "Course not found.";

        // ===== BUSINESS RULE ERRORS =====

        // BR11: CourseCode must be unique
        public const string COURSE_CODE_DUPLICATED =
            "Course code must be unique.";

        // BR12: Course must belong to exactly one department
        public const string COURSE_DEPARTMENT_REQUIRED =
            "Course must belong to exactly one department.";

        // BR13: Course credits must be between 1 and 6
        public const string COURSE_CREDITS_INVALID =
            "Course credits must be between 1 and 6.";

        // BR14: A course cannot be deleted if students are enrolled
        public const string COURSE_HAS_ENROLLMENTS =
            "Cannot delete course because students are enrolled.";

        // BR15: A course cannot be updated if it is inactive or archived
        public const string COURSE_INACTIVE_OR_ARCHIVED =
            "Cannot update course because it is inactive or archived.";
    }
}
