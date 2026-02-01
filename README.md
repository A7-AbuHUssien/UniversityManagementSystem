# University Management System API

A **University Management System** built using **ASP.NET Core Web API**.  
The project focuses on **separating concerns**, **business logic clarity**, and **realistic academic workflows** such as enrollment, grading, and course management.

This is a **backend-only system** intended to be consumed by any frontend (Web / Mobile).

---

## 🧩 What This Project Does

### 🎓 Academic Management
- Manage departments, semesters, and courses
- Control course registration periods (open / close enrollment)
- Handle student enrollments and drops
- Assign grades and generate transcripts
- Track student progress and schedules

### 👤 Users & Identity
- Authentication using JWT
- Role-based access (Admin / Instructor / Student)
- Student & instructor registration
- Profile update and password management

### 📊 Admin & Dashboard
- Dashboard statistics (load, critical courses, drop rates)
- User and department management
- System-wide academic control

---

## 🛠️ Tech Stack

- **Framework:** ASP.NET Core Web API
- **Language:** C#
- **Database:** SQL Server
- **ORM:** Entity Framework Core
- **Authentication:** JWT + Refresh Tokens
- **Architecture Style:** Layered Architecture
- **Patterns Used:** Repository, Unit of Work

---

## 📁 Project Structure (High Level)

- **UniversityManagementSystem.Api**
  - API endpoints
  - Middlewares & filters
  - Area-based endpoint grouping

- **UniversityManagementSystem.Application**
  - Business logic
  - DTOs
  - Service interfaces
  - Validation & business rules

- **UniversityManagementSystem.Infrastructure**
  - EF Core DbContext
  - Repositories & UnitOfWork
  - Migrations & data seeding

---

## 🧠 Design Notes (Honest)

- Follows a **Layered Architecture**
- Clear separation between:
  - API
  - Application logic
  - Infrastructure
- Business rules are explicitly handled using:
  - Validation classes
  - Business constraint services
- Not a full Clean Architecture
- No CQRS / MediatR / DDD

The focus is **clarity and correctness**, not abstraction for its own sake.


---

## ▶️ How to Run

1. Open `UniversityManagementSystem.sln`
2. Update the database connection string
3. Run `Update-Database`
4. Start the API project
5. Test endpoints using Swagger or Postman

---

## 📌 Project Purpose

This project was built to:
- Practice real backend system design
- Apply layered architecture correctly
- Handle non-trivial business logic
- Build a realistic academic management system

It is intended as a **portfolio and learning project**, not a production-ready university system.

---

## 📝 Notes

- No frontend included
- No automated tests yet
- API-first design
- Business logic is prioritized over UI concerns

