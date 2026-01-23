using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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

namespace PRN222.CourseManagementPresentation
{
    public class Program
    {
        static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
  .AddJsonFile("appsettings.json")
  .Build();
            var services = new ServiceCollection();

            services.AddDbContext<CourseManagementDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IStudentRepository, StudentRepository>();
            services.AddScoped<IEnrollementRepository, EnrollmentRepository>();
            services.AddScoped<ICourseRepository, CourseRepository>();
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();



            var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();



            while (true)
            {
                Console.Clear();
                Console.WriteLine("====================================");
                Console.WriteLine("   COURSE MANAGEMENT SYSTEM");
                Console.WriteLine("====================================");
                Console.WriteLine("1. Display all students");
                Console.WriteLine("2. Display courses by department");
                Console.WriteLine("3. Display courses of a student");
                Console.WriteLine("4. Enroll student into course");
                Console.WriteLine("5. Update student information");
                Console.WriteLine("6. Delete a course");
                Console.WriteLine("7. Display enrollment report");
                Console.WriteLine("0. Exit");
                Console.WriteLine("------------------------------------");
                Console.Write("Select an option: ");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.Clear();
                        Console.WriteLine("=== All Students ===");
                        foreach (var s in unitOfWork.studentRepository.GetAll())
                        {
                            Console.WriteLine($"{s.StudentId} - {s.FullName} - {s.Email}");
                        }
                        Pause();
                        break;

                    case "2":
                        Console.Write("Enter Department Id: ");
                        int depId = int.Parse(Console.ReadLine()!);
                        var coursesByDep = unitOfWork.courseRepository.GetAll()
                                                       .Where(c => c.DepartmentId == depId);

                        Console.WriteLine("Courses:");
                        foreach (var c in coursesByDep)
                        {
                            Console.WriteLine($"{c.CourseId} - {c.Title}");
                        }
                        Pause();
                        break;

                    case "3":
                        Console.Write("Enter Student Id: ");
                        int stuId = int.Parse(Console.ReadLine()!);

                        var enrolledCourses =
                            from e in unitOfWork.enrollementRepository.GetAll()
                            join c in unitOfWork.courseRepository.GetAll() on e.CourseId equals c.CourseId
                            where e.StudentId == stuId
                            select c;

                        Console.WriteLine("Courses of Student:");
                        foreach (var c in enrolledCourses)
                        {
                            Console.WriteLine($"{c.CourseId} - {c.Title}");
                        }
                        Pause();
                        break;

                    case "4":
                        Console.Write("Student Id: ");
                        int sId = int.Parse(Console.ReadLine()!);
                        Console.Write("Course Id: ");
                        int cId = int.Parse(Console.ReadLine()!);

                        unitOfWork.enrollementRepository.Add(new Enrollment
                        {
                            StudentId = sId,
                            CourseId = cId,
                            EnrollDate = DateTime.Now
                        });
                        unitOfWork.SaveChangeAsync();

                        Console.WriteLine("Enrolled successfully!");
                        Pause();
                        break;

                    case "5":
                        Console.Write("Student Id: ");
                        int updateId = int.Parse(Console.ReadLine()!);

                        var student = unitOfWork.studentRepository.GetById(updateId);
                        if (student != null)
                        {
                            Console.Write("New Full Name: ");
                            student.FullName = Console.ReadLine()!;
                            Console.Write("New Email: ");
                            student.Email = Console.ReadLine();

                            unitOfWork.studentRepository.Update(student);
                            unitOfWork.SaveChangeAsync();
                            Console.WriteLine("Updated successfully!");
                        }
                        Pause();
                        break;

                    case "6":
                        Console.Write("Course Id to delete: ");
                        int delId = int.Parse(Console.ReadLine()!);

                        unitOfWork.courseRepository.Delete(delId);
                        unitOfWork.SaveChangeAsync();

                        Console.WriteLine("Deleted successfully!");
                        Pause();
                        break;

                    case "7":
                        Console.WriteLine("=== Enrollment Report ===");
                        var report =
                            from e in unitOfWork.enrollementRepository.GetAll()
                            join s in unitOfWork.studentRepository.GetAll() on e.StudentId equals s.StudentId
                            join c in unitOfWork.courseRepository.GetAll() on e.CourseId equals c.CourseId
                            select new { s.FullName, c.Title, e.EnrollDate, e.Grade };

                        foreach (var r in report)
                        {
                            Console.WriteLine($"{r.FullName}            | {r.Title}            | {r.EnrollDate:d}                 | {r.Grade}");
                        }
                        Pause();
                        break;

                    case "0":
                        return;

                    default:
                        Console.WriteLine("Invalid option!");
                        Pause();
                        break;
                }
            }

            static void Pause()
            {
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }

        }
    }
}
