using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UniversityManagementSystem.Application.Common;
using UniversityManagementSystem.Application.Entities;
using UniversityManagementSystem.Infrastructure.DataAccess;

namespace UniversityManagementSystem.Infrastructure.Seeders;

public static class DbInitializer
{
    public static async Task SeedAsync(AppDbContext context, UserManager<IdentityUser<Guid>> userManager,
        RoleManager<IdentityRole<Guid>> roleManager)
    {
        // 1. الأدوار (Roles)
        if (!await roleManager.Roles.AnyAsync())
        {
            foreach (var role in AppRoles.AllRoles)
                await roleManager.CreateAsync(new IdentityRole<Guid> { Name = role, NormalizedName = role.ToUpper() });
        }

        // 2. الأقسام (Departments)
        if (!await context.Departments.AnyAsync())
        {
            await context.Departments.AddRangeAsync(
                new Department { Name = "Computer Science", Code = "CS" },
                new Department { Name = "Information Technology", Code = "IT" },
                new Department { Name = "Artificial Intelligence", Code = "AI" },
                new Department { Name = "Cyber Security", Code = "CYB" },
                new Department { Name = "Business Informatics", Code = "BI" }
            );
            await context.SaveChangesAsync();
        }

        var csDept = await context.Departments.FirstAsync(d => d.Code == "CS");
        var aiDept = await context.Departments.FirstAsync(d => d.Code == "AI");
        var cybDept = await context.Departments.FirstAsync(d => d.Code == "CYB");

        // 3. المدرسين (Instructors + Identity Users)
        if (!await context.Instructors.AnyAsync())
        {
            var instructors = new List<(string FName, string LName, string PEmail, string Specialization, int DeptId)>
            {
                ("Ahmed", "Zewail", "ahmed.zewail@gmail.com", "Quantum Computing", csDept.Id),
                ("Samy", "Amin", "samy.tech@yahoo.com", "Machine Learning", aiDept.Id),
                ("Mariam", "Fahmy", "m.fahmy@outlook.com", "Network Security", cybDept.Id),
                ("Hassan", "Ali", "hassan.ali@gmail.com", "Algorithms & Data Structures", csDept.Id),
                ("Nour", "Salem", "nour.s@university.com", "Digital Forensics", cybDept.Id)
            };

            foreach (var inst in instructors)
            {
                var academicEmail = $"{inst.FName.ToLower()}.{inst.LName.ToLower()}@uni.edu";
                var user = new IdentityUser<Guid>
                    { UserName = academicEmail, Email = academicEmail, EmailConfirmed = true };

                if (await userManager.FindByEmailAsync(academicEmail) == null)
                {
                    await userManager.CreateAsync(user, "InstPass123!");
                    await userManager.AddToRoleAsync(user, AppRoles.INSTRUCTOR);

                    await context.Instructors.AddAsync(new Instructor
                    {
                        FirstName = inst.FName, LastName = inst.LName,
                        PersonalEmail = inst.PEmail, Specialization = inst.Specialization,
                        DepartmentId = inst.DeptId, ApplicationUserId = user.Id
                    });
                }
            }

            await context.SaveChangesAsync();
        }

        // 4. الطلاب (Students + Identity Users) - 10 طلاب ببيانات متنوعة
        if (!await context.Students.AnyAsync())
        {
            var students = new List<(string FName, string LName, string PEmail, string Phone, int DeptId)>
            {
                ("Bu", "Hussien", "bu.hussien@gmail.com", "01012345678", csDept.Id),
                ("Omar", "Khaled", "omar.k@gmail.com", "01122334455", aiDept.Id),
                ("Laila", "Nabil", "laila.n@yahoo.com", "01233445566", cybDept.Id),
                ("Zeyad", "Sultan", "zeyad.s@gmail.com", "01544556677", csDept.Id),
                ("Sara", "Mostafa", "sara.m@outlook.com", "01099887766", aiDept.Id),
                ("Youssef", "Ezzat", "youssef.e@gmail.com", "01155667788", csDept.Id),
                ("Hoda", "Ibrahim", "hoda.i@yahoo.com", "01266778899", cybDept.Id),
                ("Kareem", "Adel", "kareem.a@gmail.com", "01011223344", csDept.Id),
                ("Jana", "Mahmoud", "jana.m@gmail.com", "01133445522", aiDept.Id),
                ("Mazen", "Tarek", "mazen.t@outlook.com", "01288776655", cybDept.Id)
            };

            foreach (var std in students)
            {
                var academicEmail = $"{std.FName.ToLower()}.{std.LName.ToLower()}2026@uni.edu";
                var user = new IdentityUser<Guid>
                    { UserName = academicEmail, Email = academicEmail, EmailConfirmed = true };

                if (await userManager.FindByEmailAsync(academicEmail) == null)
                {
                    await userManager.CreateAsync(user, "StdPass123!");
                    await userManager.AddToRoleAsync(user, AppRoles.STUDENT);

                    await context.Students.AddAsync(new Student
                    {
                        FirstName = std.FName, LastName = std.LName,
                        PersonalEmail = std.PEmail, Phone = std.Phone,
                        DateOfBirth = new DateOnly(2003, 5, 15),
                        DepartmentId = std.DeptId, ApplicationUserId = user.Id
                    });
                }
            }

            await context.SaveChangesAsync();
        }

        // 5. نظام الدرجات (Grades)
        if (!await context.Grades.AnyAsync())
        {
            await context.Grades.AddRangeAsync(
                new Grade { Symbol = "A+", MinScore = 95, MaxScore = 100, GPAPoint = 4.0m, Description = "Excellent" },
                new Grade
                {
                    Symbol = "A", MinScore = 90, MaxScore = 94.99m, GPAPoint = 3.7m, Description = "Excellent"
                },
                new Grade
                {
                    Symbol = "B+", MinScore = 85, MaxScore = 89.99m, GPAPoint = 3.3m, Description = "Very Good"
                },
                new Grade
                {
                    Symbol = "B", MinScore = 80, MaxScore = 84.99m, GPAPoint = 3.0m, Description = "Very Good"
                },
                new Grade { Symbol = "C+", MinScore = 75, MaxScore = 79.99m, GPAPoint = 2.5m, Description = "Good" },
                new Grade { Symbol = "F", MinScore = 0, MaxScore = 59.99m, GPAPoint = 0.0m, Description = "Fail" }
            );
            await context.SaveChangesAsync();
        }

        // 6. الأترام (Semesters)
        if (!await context.Semesters.AnyAsync())
        {
            await context.Semesters.AddAsync(new Semester
            {
                Name = "Spring", Year = 2026, IsActive = true, IsRegistrationOpen = true,
                StartDate = new DateOnly(2026, 2, 1), EndDate = new DateOnly(2026, 6, 15)
            });
            await context.SaveChangesAsync();
        }
        // 1.5 Create Admin User (The Big Boss)
        var adminEmail = "admin@university.edu";
        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var adminUser = new IdentityUser<Guid> 
            { 
                UserName = adminEmail, 
                Email = adminEmail, 
                EmailConfirmed = true 
            };

            var result = await userManager.CreateAsync(adminUser, "AdminPass123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, AppRoles.SUPER_ADMIN);
            }
        }
    }
}