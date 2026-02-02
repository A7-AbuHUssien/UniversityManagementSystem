
GO/* =============================================
   1) CREATE DATABASE
============================================= */
CREATE DATABASE UniversityDB;
GO

USE UniversityDB;
GO

/* =============================================
   2) IDENTITY TABLES (No Dependencies)
============================================= */

-- AspNetRoles
CREATE TABLE [dbo].[AspNetRoles](
    [Id] UNIQUEIDENTIFIER NOT NULL,
    [Name] NVARCHAR(256) NULL,
    [NormalizedName] NVARCHAR(256) NULL,
    [ConcurrencyStamp] NVARCHAR(MAX) NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE UNIQUE INDEX [RoleNameIndex]
ON [dbo].[AspNetRoles]([NormalizedName])
WHERE [NormalizedName] IS NOT NULL;
GO


-- AspNetUsers
CREATE TABLE [dbo].[AspNetUsers](
    [Id] UNIQUEIDENTIFIER NOT NULL,
    [UserName] NVARCHAR(256) NULL,
    [NormalizedUserName] NVARCHAR(256) NULL,
    [Email] NVARCHAR(256) NULL,
    [NormalizedEmail] NVARCHAR(256) NULL,
    [EmailConfirmed] BIT NOT NULL,
    [PasswordHash] NVARCHAR(MAX) NULL,
    [SecurityStamp] NVARCHAR(MAX) NULL,
    [ConcurrencyStamp] NVARCHAR(MAX) NULL,
    [PhoneNumber] NVARCHAR(MAX) NULL,
    [PhoneNumberConfirmed] BIT NOT NULL,
    [TwoFactorEnabled] BIT NOT NULL,
    [LockoutEnd] DATETIMEOFFSET(7) NULL,
    [LockoutEnabled] BIT NOT NULL,
    [AccessFailedCount] INT NOT NULL,

    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE INDEX [EmailIndex]
ON [dbo].[AspNetUsers]([NormalizedEmail]);
GO

CREATE UNIQUE INDEX [UserNameIndex]
ON [dbo].[AspNetUsers]([NormalizedUserName])
WHERE [NormalizedUserName] IS NOT NULL;
GO


/* =============================================
   3) Departments
============================================= */

CREATE TABLE [dbo].[Departments](
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Name] NVARCHAR(100) NOT NULL,
    [Code] NVARCHAR(10) NOT NULL,
    [StudentsCount] INT NOT NULL,
    [CreatedAt] DATETIME2(7) NOT NULL,
    [UpdatedAt] DATETIME2(7) NULL,

    CONSTRAINT [PK_Departments] PRIMARY KEY CLUSTERED ([Id])
);
GO

CREATE UNIQUE INDEX [IX_Departments_Code]
ON [dbo].[Departments]([Code]);
GO


/* =============================================
   4) Semesters
============================================= */

CREATE TABLE [dbo].[Semesters](
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Name] NVARCHAR(MAX) NOT NULL,
    [Year] INT NOT NULL,
    [IsActive] BIT NOT NULL,
    [IsRegistrationOpen] BIT NOT NULL,
    [StartDate] DATE NOT NULL,
    [EndDate] DATE NOT NULL,
    [CreatedAt] DATETIME2(7) NOT NULL,
    [UpdatedAt] DATETIME2(7) NULL,

    CONSTRAINT [PK_Semesters] PRIMARY KEY CLUSTERED ([Id])
);
GO


/* =============================================
   5) Instructors
============================================= */

CREATE TABLE [dbo].[Instructors](
    [Id] INT IDENTITY(1,1) NOT NULL,
    [FirstName] NVARCHAR(50) NOT NULL,
    [LastName] NVARCHAR(50) NOT NULL,
    [PersonalEmail] NVARCHAR(150) NOT NULL,
    [Specialization] NVARCHAR(100) NOT NULL,
    [DepartmentId] INT NOT NULL,
    [ApplicationUserId] UNIQUEIDENTIFIER NOT NULL,
    [CreatedAt] DATETIME2(7) NOT NULL,
    [UpdatedAt] DATETIME2(7) NULL,

    CONSTRAINT [PK_Instructors] PRIMARY KEY CLUSTERED ([Id])
);
GO

CREATE UNIQUE INDEX [IX_Instructors_ApplicationUserId]
ON [dbo].[Instructors]([ApplicationUserId]);

CREATE UNIQUE INDEX [IX_Instructors_PersonalEmail]
ON [dbo].[Instructors]([PersonalEmail]);

CREATE INDEX [IX_Instructors_DepartmentId]
ON [dbo].[Instructors]([DepartmentId]);

ALTER TABLE [dbo].[Instructors] ADD CONSTRAINT
FK_Instructors_AspNetUsers
FOREIGN KEY (ApplicationUserId)
REFERENCES AspNetUsers(Id)
ON DELETE CASCADE;

ALTER TABLE [dbo].[Instructors] ADD CONSTRAINT
FK_Instructors_Departments
FOREIGN KEY (DepartmentId)
REFERENCES Departments(Id);
GO


/* =============================================
   6) Students
============================================= */

CREATE TABLE [dbo].[Students](
    [Id] INT IDENTITY(1,1) NOT NULL,
    [FirstName] NVARCHAR(50) NOT NULL,
    [LastName] NVARCHAR(50) NOT NULL,
    [PersonalEmail] NVARCHAR(150) NOT NULL,
    [Phone] NVARCHAR(20) NOT NULL,
    [DateOfBirth] DATE NOT NULL,
    [DepartmentId] INT NOT NULL,
    [IsDeleted] BIT NOT NULL,
    [DeletedAt] DATETIME2(7) NULL,
    [ApplicationUserId] UNIQUEIDENTIFIER NOT NULL,
    [CreatedAt] DATETIME2(7) NOT NULL,
    [UpdatedAt] DATETIME2(7) NULL,

    CONSTRAINT [PK_Students] PRIMARY KEY CLUSTERED ([Id])
);
GO

CREATE UNIQUE INDEX [IX_Students_ApplicationUserId]
ON [dbo].[Students]([ApplicationUserId]);

CREATE UNIQUE INDEX [IX_Students_PersonalEmail]
ON [dbo].[Students]([PersonalEmail]);

CREATE INDEX [IX_Students_DepartmentId]
ON [dbo].[Students]([DepartmentId]);

ALTER TABLE [dbo].[Students] ADD CONSTRAINT
FK_Students_AspNetUsers
FOREIGN KEY (ApplicationUserId)
REFERENCES AspNetUsers(Id)
ON DELETE CASCADE;

ALTER TABLE [dbo].[Students] ADD CONSTRAINT
FK_Students_Departments
FOREIGN KEY (DepartmentId)
REFERENCES Departments(Id);
GO


/* =============================================
   7) Courses
============================================= */

CREATE TABLE [dbo].[Courses](
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Title] NVARCHAR(200) NOT NULL,
    [CourseCode] NVARCHAR(15) NOT NULL,
    [Credits] INT NOT NULL DEFAULT 3,
    [MaxCapacity] INT NOT NULL,
    [CurrentCapacity] INT NOT NULL,
    [DepartmentId] INT NOT NULL,
    [InstructorId] INT NOT NULL,
    [Day] INT NOT NULL,
    [Hour] INT NOT NULL,
    [PrerequisiteId] INT NULL,
    [IsActive] BIT NOT NULL,
    [CreatedAt] DATETIME2(7) NOT NULL,
    [UpdatedAt] DATETIME2(7) NULL,

    CONSTRAINT [PK_Courses] PRIMARY KEY CLUSTERED ([Id])
);
GO

CREATE UNIQUE INDEX [IX_Courses_CourseCode]
ON [dbo].[Courses]([CourseCode]);

CREATE INDEX [IX_Courses_DepartmentId]
ON [dbo].[Courses]([DepartmentId]);

CREATE INDEX [IX_Courses_InstructorId]
ON [dbo].[Courses]([InstructorId]);

CREATE INDEX [IX_Courses_PrerequisiteId]
ON [dbo].[Courses]([PrerequisiteId]);

ALTER TABLE [dbo].[Courses] ADD CONSTRAINT
FK_Courses_Departments
FOREIGN KEY (DepartmentId)
REFERENCES Departments(Id);

ALTER TABLE [dbo].[Courses] ADD CONSTRAINT
FK_Courses_Instructors
FOREIGN KEY (InstructorId)
REFERENCES Instructors(Id);

ALTER TABLE [dbo].[Courses] ADD CONSTRAINT
FK_Courses_Prerequisite
FOREIGN KEY (PrerequisiteId)
REFERENCES Courses(Id);
GO




/* =============================================
   8) Grades
============================================= */

CREATE TABLE [dbo].[Grades](
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Symbol] NVARCHAR(5) NOT NULL,
    [MinScore] DECIMAL(5,2) NOT NULL,
    [MaxScore] DECIMAL(5,2) NOT NULL,
    [Description] NVARCHAR(100) NOT NULL,
    [GPAPoint] DECIMAL(18,2) NOT NULL,
    [CreatedAt] DATETIME2(7) NOT NULL,
    [UpdatedAt] DATETIME2(7) NULL,

    CONSTRAINT [PK_Grades] PRIMARY KEY CLUSTERED ([Id])
);
GO


/* =============================================
   9) Enrollments
============================================= */

CREATE TABLE [dbo].[Enrollments](
    [Id] INT IDENTITY(1,1) NOT NULL,

    [StudentId] INT NOT NULL,
    [CourseId] INT NOT NULL,
    [SemesterId] INT NOT NULL,

    [EnrollmentDate] DATETIME2(7) NOT NULL DEFAULT GETDATE(),
    [Status] INT NOT NULL DEFAULT 1,

    [NumericScore] DECIMAL(5,2) NULL,
    [GradeId] INT NULL,

    [CreatedAt] DATETIME2(7) NOT NULL,
    [UpdatedAt] DATETIME2(7) NULL,

    CONSTRAINT [PK_Enrollments] PRIMARY KEY CLUSTERED ([Id])
);
GO

CREATE INDEX [IX_Enrollments_CourseId]
ON [dbo].[Enrollments]([CourseId]);

CREATE INDEX [IX_Enrollments_SemesterId]
ON [dbo].[Enrollments]([SemesterId]);

CREATE INDEX [IX_Enrollments_GradeId]
ON [dbo].[Enrollments]([GradeId]);

CREATE UNIQUE INDEX [IX_Enrollments_Student_Course_Semester]
ON [dbo].[Enrollments]([StudentId],[CourseId],[SemesterId]);
GO

ALTER TABLE [dbo].[Enrollments] ADD CONSTRAINT
FK_Enrollments_Students
FOREIGN KEY (StudentId)
REFERENCES Students(Id)
ON DELETE CASCADE;

ALTER TABLE [dbo].[Enrollments] ADD CONSTRAINT
FK_Enrollments_Courses
FOREIGN KEY (CourseId)
REFERENCES Courses(Id)
ON DELETE CASCADE;

ALTER TABLE [dbo].[Enrollments] ADD CONSTRAINT
FK_Enrollments_Semesters
FOREIGN KEY (SemesterId)
REFERENCES Semesters(Id)
ON DELETE CASCADE;

ALTER TABLE [dbo].[Enrollments] ADD CONSTRAINT
FK_Enrollments_Grades
FOREIGN KEY (GradeId)
REFERENCES Grades(Id);
GO


/* =============================================
   10) Identity Related Tables
============================================= */

-- User Roles
CREATE TABLE [dbo].[AspNetUserRoles](
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [RoleId] UNIQUEIDENTIFIER NOT NULL,

    CONSTRAINT [PK_AspNetUserRoles]
    PRIMARY KEY CLUSTERED ([UserId],[RoleId])
);
GO

CREATE INDEX [IX_AspNetUserRoles_RoleId]
ON [dbo].[AspNetUserRoles]([RoleId]);

ALTER TABLE [dbo].[AspNetUserRoles] ADD CONSTRAINT
FK_AspNetUserRoles_Users
FOREIGN KEY (UserId)
REFERENCES AspNetUsers(Id)
ON DELETE CASCADE;

ALTER TABLE [dbo].[AspNetUserRoles] ADD CONSTRAINT
FK_AspNetUserRoles_Roles
FOREIGN KEY (RoleId)
REFERENCES AspNetRoles(Id)
ON DELETE CASCADE;
GO


-- User Logins
CREATE TABLE [dbo].[AspNetUserLogins](
    [LoginProvider] NVARCHAR(450) NOT NULL,
    [ProviderKey] NVARCHAR(450) NOT NULL,
    [ProviderDisplayName] NVARCHAR(MAX) NULL,
    [UserId] UNIQUEIDENTIFIER NOT NULL,

    CONSTRAINT [PK_AspNetUserLogins]
    PRIMARY KEY CLUSTERED ([LoginProvider],[ProviderKey])
);
GO

CREATE INDEX [IX_AspNetUserLogins_UserId]
ON [dbo].[AspNetUserLogins]([UserId]);

ALTER TABLE [dbo].[AspNetUserLogins] ADD CONSTRAINT
FK_AspNetUserLogins_Users
FOREIGN KEY (UserId)
REFERENCES AspNetUsers(Id)
ON DELETE CASCADE;
GO


-- User Claims
CREATE TABLE [dbo].[AspNetUserClaims](
    [Id] INT IDENTITY(1,1) NOT NULL,
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [ClaimType] NVARCHAR(MAX) NULL,
    [ClaimValue] NVARCHAR(MAX) NULL,

    CONSTRAINT [PK_AspNetUserClaims]
    PRIMARY KEY CLUSTERED ([Id])
);
GO

CREATE INDEX [IX_AspNetUserClaims_UserId]
ON [dbo].[AspNetUserClaims]([UserId]);

ALTER TABLE [dbo].[AspNetUserClaims] ADD CONSTRAINT
FK_AspNetUserClaims_Users
FOREIGN KEY (UserId)
REFERENCES AspNetUsers(Id)
ON DELETE CASCADE;
GO


-- Role Claims
CREATE TABLE [dbo].[AspNetRoleClaims](
    [Id] INT IDENTITY(1,1) NOT NULL,
    [RoleId] UNIQUEIDENTIFIER NOT NULL,
    [ClaimType] NVARCHAR(MAX) NULL,
    [ClaimValue] NVARCHAR(MAX) NULL,

    CONSTRAINT [PK_AspNetRoleClaims]
    PRIMARY KEY CLUSTERED ([Id])
);
GO

CREATE INDEX [IX_AspNetRoleClaims_RoleId]
ON [dbo].[AspNetRoleClaims]([RoleId]);

ALTER TABLE [dbo].[AspNetRoleClaims] ADD CONSTRAINT
FK_AspNetRoleClaims_Roles
FOREIGN KEY (RoleId)
REFERENCES AspNetRoles(Id)
ON DELETE CASCADE;
GO


-- User Tokens
CREATE TABLE [dbo].[AspNetUserTokens](
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [LoginProvider] NVARCHAR(450) NOT NULL,
    [Name] NVARCHAR(450) NOT NULL,
    [Value] NVARCHAR(MAX) NULL,

    CONSTRAINT [PK_AspNetUserTokens]
    PRIMARY KEY CLUSTERED ([UserId],[LoginProvider],[Name])
);
GO

ALTER TABLE [dbo].[AspNetUserTokens] ADD CONSTRAINT
FK_AspNetUserTokens_Users
FOREIGN KEY (UserId)
REFERENCES AspNetUsers(Id)
ON DELETE CASCADE;
GO


/* =============================================
   11) Refresh Tokens
============================================= */

CREATE TABLE [dbo].[UserRefreshTokens](
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Token] NVARCHAR(500) NOT NULL,
    [ExpiryDate] DATETIME2(7) NOT NULL,
    [IsRevoked] BIT NOT NULL DEFAULT 0,
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [CreatedAt] DATETIME2(7) NOT NULL,
    [UpdatedAt] DATETIME2(7) NULL,

    CONSTRAINT [PK_UserRefreshTokens]
    PRIMARY KEY CLUSTERED ([Id])
);
GO

CREATE UNIQUE INDEX [IX_UserRefreshTokens_Token]
ON [dbo].[UserRefreshTokens]([Token]);

CREATE INDEX [IX_UserRefreshTokens_UserId]
ON [dbo].[UserRefreshTokens]([UserId]);

ALTER TABLE [dbo].[UserRefreshTokens] ADD CONSTRAINT
FK_UserRefreshTokens_Users
FOREIGN KEY (UserId)
REFERENCES AspNetUsers(Id)
ON DELETE CASCADE;
