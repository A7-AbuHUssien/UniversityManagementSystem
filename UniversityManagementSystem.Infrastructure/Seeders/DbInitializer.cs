using UniversityManagementSystem.Application.Entities;
using Microsoft.EntityFrameworkCore;
using UniversityManagementSystem.Infrastructure.DataAccess;

namespace UniversityManagementSystem.Infrastructure.Seeders;

public static class DbInitializer
{
    public static async Task SeedAsync(AppDbContext context)
    {
        // 1. الأقسام (Departments) - إضافة أقسام أكثر للتنوع
        if (!await context.Departments.AnyAsync())
        {
            await context.Departments.AddRangeAsync(
                new Department { Name = "Computer Science", Code = "CS" },
                new Department { Name = "Information Technology", Code = "IT" },
                new Department { Name = "Software Engineering", Code = "SE" },
                new Department { Name = "General Engineering", Code = "ENG" }
            );
            await context.SaveChangesAsync();
        }

        var csDept = await context.Departments.FirstAsync(d => d.Code == "CS");
        var seDept = await context.Departments.FirstAsync(d => d.Code == "SE");

        if (!await context.Instructors.AnyAsync())
        {
            await context.Instructors.AddRangeAsync(
                new Instructor { FirstName = "Khaled", LastName = "Mansour", Email = "khaled@uni.edu", Specialization = "Algorithms", DepartmentId = csDept.Id },
                new Instructor { FirstName = "Mona", LastName = "Zaki", Email = "mona@uni.edu", Specialization = "Databases", DepartmentId = csDept.Id },
                new Instructor { FirstName = "Ibrahim", LastName = "Salem", Email = "ibrahim@uni.edu", Specialization = "Project Management", DepartmentId = seDept.Id },
                new Instructor { FirstName = "Laila", LastName = "Hassan", Email = "laila@uni.edu", Specialization = "Cyber Security", DepartmentId = csDept.Id }
            );
            await context.SaveChangesAsync();
        }

        var instAlgorithms = await context.Instructors.FirstAsync(i => i.FirstName == "Khaled");
        var instDB = await context.Instructors.FirstAsync(i => i.FirstName == "Mona");

        // 3. المواد (Courses) - تصميم "فخاخ" للاختبار
        if (!await context.Courses.AnyAsync())
        {
            // سلسلة متطلبات: CS101 -> CS102 -> CS201
            var cs101 = new Course { Title = "Intro to Programming", CourseCode = "CS101", Credits = 3, MaxCapacity = 40, DepartmentId = csDept.Id, InstructorId = instAlgorithms.Id, Day = "Sunday", Hour = 9 };
            await context.Courses.AddAsync(cs101);
            await context.SaveChangesAsync();
            var cs102 = new Course { Title = "Object Oriented Programming", CourseCode = "CS102", Credits = 3, MaxCapacity = 30, DepartmentId = csDept.Id, InstructorId = instAlgorithms.Id, PrerequisiteId = cs101.Id, Day = "Tuesday", Hour = 11 };
            await context.Courses.AddAsync(cs102);
            await context.SaveChangesAsync();
            var cs201 = new Course { Title = "Data Structures", CourseCode = "CS201", Credits = 3, MaxCapacity = 25, DepartmentId = csDept.Id, InstructorId = instAlgorithms.Id, PrerequisiteId = cs102.Id, Day = "Thursday", Hour = 13 };
            await context.Courses.AddAsync(cs201);
            
            await context.Courses.AddAsync(new Course { Title = "Database Systems", CourseCode = "CS103", Credits = 3, MaxCapacity = 30, DepartmentId = csDept.Id, InstructorId = instDB.Id, Day = "Sunday", Hour = 9 });
            await context.Courses.AddAsync(new Course { Title = "Advanced Security", CourseCode = "CS404", Credits = 3, MaxCapacity = 2, DepartmentId = csDept.Id, InstructorId = instDB.Id, Day = "Wednesday", Hour = 10 });
            await context.Courses.AddAsync(new Course { Title = "Technical Writing", CourseCode = "ENG201", Credits = 2, MaxCapacity = 50, DepartmentId = seDept.Id, InstructorId = instAlgorithms.Id, Day = "Monday", Hour = 8 });

            await context.SaveChangesAsync();
        }

        // 4. الطلاب (Students) - إضافة بروفايلات مختلفة
        if (!await context.Students.AnyAsync())
        {
            await context.Students.AddRangeAsync(
                new Student { FirstName = "Bu", LastName = "Hussien", Email = "bu.hussien@uni.edu", Phone = "0501111111", DateOfBirth = new DateOnly(2003, 1, 1), DepartmentId = csDept.Id },
                new Student { FirstName = "Omar", LastName = "Zaki", Email = "omar@uni.edu", Phone = "0502222222", DateOfBirth = new DateOnly(2004, 5, 10), DepartmentId = csDept.Id },
                new Student { FirstName = "Laila", LastName = "Fahmy", Email = "laila@uni.edu", Phone = "0503333333", DateOfBirth = new DateOnly(2002, 11, 20), DepartmentId = seDept.Id },
                new Student { FirstName = "Zayed", LastName = "Sultan", Email = "zayed@uni.edu", Phone = "0504444444", DateOfBirth = new DateOnly(2003, 3, 15), DepartmentId = csDept.Id }
            );
            await context.SaveChangesAsync();
        }

        // 5. نظام الدرجات (Grades)
        if (!await context.Grades.AnyAsync())
        {
            await context.Grades.AddRangeAsync(
                new Grade { Symbol = "A+", MinScore = 95, MaxScore = 100, GPAPoint = 4.0m, Description = "Excellent" },
                new Grade { Symbol = "A",  MinScore = 90, MaxScore = 94.99m, GPAPoint = 3.7m, Description = "Excellent" },
                new Grade { Symbol = "B+", MinScore = 85, MaxScore = 89.99m, GPAPoint = 3.3m, Description = "Very Good" },
                new Grade { Symbol = "B",  MinScore = 80, MaxScore = 84.99m, GPAPoint = 3.0m, Description = "Very Good" },
                new Grade { Symbol = "C+", MinScore = 75, MaxScore = 79.99m, GPAPoint = 2.5m, Description = "Good" },
                new Grade { Symbol = "C+", MinScore = 65, MaxScore = 74.99m, GPAPoint = 2.5m, Description = "Good" },
                new Grade { Symbol = "C",  MinScore = 60, MaxScore = 64.99m, GPAPoint = 2.0m, Description = "Weak" },
                new Grade { Symbol = "F",  MinScore = 0,  MaxScore = 59.99m, GPAPoint = 0.0m, Description = "Fail" }
            );
            await context.SaveChangesAsync();
        }

        // 6. الأترام (Semesters)
        if (!await context.Semesters.AnyAsync())
        {
            await context.Semesters.AddRangeAsync(
                new Semester { Name = "Fall", Year = 2025, IsActive = false, StartDate = new DateOnly(2025, 9, 1), EndDate = new DateOnly(2026, 1, 15) },
                new Semester { Name = "Spring", Year = 2026, IsActive = true, StartDate = new DateOnly(2026, 2, 1), EndDate = new DateOnly(2026, 6, 15) },
                new Semester { Name = "Summer", Year = 2026, IsActive = false, StartDate = new DateOnly(2026, 7, 1), EndDate = new DateOnly(2026, 8, 30) }
            );
            await context.SaveChangesAsync();
        }
    }
}