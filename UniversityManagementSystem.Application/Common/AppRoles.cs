namespace UniversityManagementSystem.Application.Common;

public class AppRoles
{
    public const string SUPER_ADMIN = "Super_Admin";
    public const string ADMIN = "Admin";
    public const string OPERATION = "Operation";
    public const string INSTRUCTOR = "Instructor";
    public const string STUDENT = "Student";
    public static List<string> AllRoles => new() { SUPER_ADMIN,ADMIN, OPERATION, INSTRUCTOR,STUDENT };
}