namespace PRN222.CourseManagement.Service.MessageHelper
{
    public class DepartmentMessages
    {
        // Common
        public const string NotFound = "Department not found.";

        // Create / Update
        public const string NameEmpty = "Department name cannot be empty.";
        public const string NameTooShort = "Department name must be at least 3 characters long.";
        public const string NameExists = "Department name already exists.";

        // Delete rules
        public const string HasStudents = "Cannot delete department because it has students.";
        public const string HasCourses = "Cannot delete department because it has courses.";

        // Success
        public const string Created = "Department created successfully.";
        public const string Updated = "Department updated successfully.";
        public const string Deleted = "Department deleted successfully.";
    }
}
