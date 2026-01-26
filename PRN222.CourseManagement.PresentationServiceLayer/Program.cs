using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PRN222.CourseManagement.Repository.IRepository.ICourseRepository;
using PRN222.CourseManagement.Repository.IRepository.IDepartmentRepository;
using PRN222.CourseManagement.Repository.IRepository.IEnrollmentRepository;
using PRN222.CourseManagement.Repository.IRepository.IGenericRepository;
using PRN222.CourseManagement.Repository.IRepository.StudentRepository;
using PRN222.CourseManagement.Repository.IUnitOfWork;
using PRN222.CourseManagement.Repository.Models;
using PRN222.CourseManagement.Repository.Repository.CourseRepositroy;
using PRN222.CourseManagement.Repository.Repository.DepartmentRepository;
using PRN222.CourseManagement.Repository.Repository.EnrollmentRepository;
using PRN222.CourseManagement.Repository.Repository.GenericRepository;
using PRN222.CourseManagement.Repository.Repository.StudentRepositroy;
using PRN222.CourseManagement.Repository.UnitOfWork;
using PRN222.CourseManagement.Service.IService;
using PRN222.CourseManagement.Service.Service;

namespace PRN222.CourseManagement.PresentationServiceLayer
{
    public  static class Program
    {
        static void Main(string[] args)
        {
            // ===== 1. Khởi tạo DI =====
            var services = new ServiceCollection();
            ConfigureServices(services);

            using var provider = services.BuildServiceProvider();
            using var scope = provider.CreateScope();

            // ===== 2. Resolve services =====
            var studentService = scope.ServiceProvider.GetRequiredService<IStudentService>();
            var courseService = scope.ServiceProvider.GetRequiredService<ICourseService>();
            var enrollmentService = scope.ServiceProvider.GetRequiredService<IEnrollmentService>();



            // ===== 3. Menu =====
            while (true)
            {
                Console.WriteLine("\n=== COURSE MANAGEMENT ===");
                Console.WriteLine("1. Create student");
                Console.WriteLine("2. Create course");
                Console.WriteLine("3. Enroll student");
                Console.WriteLine("4. Assign grade");
                Console.WriteLine("0. Exit");
                Console.Write("Choose: ");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        CreateStudent(studentService);
                        break;
                    case "2":
                        CreateCourse(courseService);
                        break;
                    case "3":
                        EnrollStudent(enrollmentService);
                        break;
                    case "4":
                        AssignGrade(enrollmentService);
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Invalid choice!");
                        break;
                }
            }
        }

        // ================= DEPENDENCY INJECTION =================
        static void ConfigureServices(IServiceCollection services)
        {
            // ===== DbContext =====
            services.AddDbContext<CourseManagementDbContext>(options =>
                options.UseSqlServer(
                    "Server=.;Database=CourseManagement;Trusted_Connection=True;TrustServerCertificate=True"
                ));

            // ===== Repository =====
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IStudentRepository, StudentRepository>();
            services.AddScoped<IEnrollementRepository, EnrollmentRepository>();
            services.AddScoped<ICourseRepository, CourseRepository>();
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();

            // ===== Service =====
            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<ICourseService, CourseService>();
            services.AddScoped<IEnrollmentService, EnrollmentService>();
            services.AddScoped<IDepartmentService, DepartmentService>();
        }

        // ================= FEATURES =================
        static void CreateStudent(IStudentService service)
        {
            Console.Write("Student code: ");
            string code = Console.ReadLine();   

            Console.Write("Full name: ");
            string name = Console.ReadLine();

            Console.Write("Email: ");
            string email = Console.ReadLine();

            int deptId;
            while (true)
            {
                Console.Write("Department Id: ");
                if (int.TryParse(Console.ReadLine(), out deptId))
                    break;
            }

            var result = service.Create(new Student
            {
                StudentCode = code,
                FullName = name,
                Email = email,
                DepartmentId = deptId
            });

            PrintResult(result);
        }

        static void CreateCourse(ICourseService service)
        {
            Console.Write("Course code: ");
            string code = Console.ReadLine();

            Console.Write("Course title: ");
            string title = Console.ReadLine();

            int credits;
            while (true)
            {
                Console.Write("Credits: ");
                if (int.TryParse(Console.ReadLine(), out credits))
                    break;
            }

            int deptId;
            while (true)
            {
                Console.Write("Department Id: ");
                if (int.TryParse(Console.ReadLine(), out deptId))
                    break;
            }


            var result = service.Create(new Course
            {
                CourseCode = code,
                Title = title,
                Credits = credits,
                DepartmentId = deptId
            });

            PrintResult(result);
        }

        static void EnrollStudent(IEnrollmentService service)
        {
            int studentId;
            while (true)
            {
                Console.Write("Student Id: ");
                if (int.TryParse(Console.ReadLine(), out studentId))
                    break;
            }

            int courseId;
            while (true)
            {
                Console.Write("Course Id: ");
                if (int.TryParse(Console.ReadLine(), out courseId))
                    break;
            }


            var result = service.Create(new Enrollment
            {
                StudentId = studentId,
                CourseId = courseId,
                EnrollDate = DateTime.Now
            });

            PrintResult(result);
        }

        static void AssignGrade(IEnrollmentService service)
        {
            int studentId;
            while (true)
            {
                Console.Write("Student Id: ");
                if (int.TryParse(Console.ReadLine(), out studentId))
                    break;
            }

            int courseId;
            while (true)
            {
                Console.Write("Course Id: ");
                if (int.TryParse(Console.ReadLine(), out courseId))
                    break;
            }
            Console.Write("Grade: ");
            decimal grade = decimal.Parse(Console.ReadLine());

            var enrollment = new Enrollment
            {
                StudentId = studentId,
                CourseId = courseId,
                Grade = grade,
                EnrollDate= DateTime.Now
            };

            var result = service.Update(enrollment);
            PrintResult(result);
        }

        static void PrintResult(dynamic result)
        {
            if (result.IsSuccess)
                Console.WriteLine("Success: " + result.Message);
            else
                Console.WriteLine("Failed: " + result.Message);
        }
    }
}
