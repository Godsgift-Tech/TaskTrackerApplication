
# Task Tracker API

A **Task Tracker API** built with **C#, .NET, and MediatR**, implementing **CQRS**, **role-based authorization**, **caching**, and a clean layered architecture. This API allows users to manage their tasks while managers oversee task reports and management.

## **Table of Contents**

* [Features](#features)
* [Technology Stack](#technology-stack)
* [User Roles & Workflow](#user-roles--workflow)
* [API Endpoints](#api-endpoints)
* [CQRS Implementation](#cqrs-implementation)
* [Caching](#caching)
* [Validation](#validation)
* [Unit Tests](#unit-tests)
* [Getting Started](#getting-started)


## **Features**

* **User Authentication & Authorization**

  * JWT-based authentication
  * Registration and login endpoints
  * Role-based access (`User` and `Manager`)

* **Task Management**

  * Create, update, delete, and complete tasks
  * Each task includes: Title, Description, Due Date, Status (Pending, InProgress, Completed)
  * Tasks are automatically assigned to the creator

* **Role-Based Behavior**

  * **Users:** Can create, update, complete, and delete their own tasks
  * **Managers:** Can view all tasks, update any task, complete tasks, delete tasks, and generate task completion reports

* **Reporting**

  * Task completion reports showing totals of Completed, Pending, and InProgress tasks per user

* **CQRS & MediatR**

  * Commands: CreateTask, UpdateTask, CompleteTask, DeleteTask
  * Queries: GetTaskById, GetAllTasks, GetTaskCompletionReport

* **Caching**

  * Memory caching for task retrieval to improve performance
  * Cache is invalidated on updates, deletion, or task completion

* **Validation**

  * Input validation using **FluentValidation**
  * Validators for Create, Update, Complete, and Delete commands

* **Unit Tests**

  * Controller unit tests with mocked `IMediator` for all CRUD and report operations

## **Technology Stack**

* **Backend:** C# .NET 7, ASP.NET Core Web API
* **Pattern:** Clean Architecture, CQRS with MediatR
* **Authentication:** JWT Bearer Tokens
* **Caching:** In-Memory Cache
* **Validation:** FluentValidation
* **Unit Testing:** xUnit, Moq

## **User Roles & Workflow**

### **User Workflow**

1. Registers using the `POST /api/auth/register` endpoint
2. Logs in via `POST /api/auth/login` to receive a JWT token
3. Uses the token in the Authorization header to access protected task endpoints
4. Can create tasks, update their own tasks, complete them, and delete their own tasks

### **Manager Workflow**

1. Registers and logs in as above
2. Can view all tasks using `GET /api/tasks/all`
3. Can generate task completion reports for the team via `GET /api/tasks/reports/completion`
4. Can update, complete, or delete **any task**

## **API Endpoints**

| Method | Endpoint                        | Role          | Description                                |
| ------ | ------------------------------- | ------------- | ------------------------------------------ |
| POST   | `/api/auth/register`            | Public        | Register new user                          |
| POST   | `/api/auth/login`               | Public        | Login user and return JWT token            |
| POST   | `/api/tasks`                    | User, Manager | Create a new task                          |
| PUT    | `/api/tasks/{id}`               | User, Manager | Update a task (users can update their own) |
| PATCH  | `/api/tasks/{id}/complete`      | User, Manager | Mark task as completed                     |
| DELETE | `/api/tasks/{id}`               | User, Manager | Delete a task (users can delete their own) |
| GET    | `/api/tasks/{id}`               | Authenticated | Get task by ID (manager can get any task)  |
| GET    | `/api/tasks/all`                | Manager       | Get all tasks                              |
| GET    | `/api/tasks/reports/completion` | Manager       | Generate task completion report            |


## **CQRS Implementation**

* **Commands**

  * `CreateTaskCommand`
  * `UpdateTaskCommand`
  * `CompleteTaskCommand`
  * `DeleteTaskCommand`
* **Queries**

  * `GetTaskByIdQuery`
  * `GetAllTasksQuery`
  * `GetTaskCompletionReportQuery`
* **Handlers**

  * Each command and query has a corresponding handler implementing the logic, including role-based authorization, caching, and database operations

## **Caching**

* Implemented via `IMemoryCache`
* Keys are constructed based on task ID, user ID, and query parameters
* Cache invalidation occurs automatically on task update, deletion, or completion

## **Validation**

* Implemented via **FluentValidation**
* Examples:

  * `UpdateTaskCommandValidator`
  * `CompleteTaskCommandValidator`
  * `DeleteTaskCommandValidator`
* Validates all necessary fields, including IDs, title length, and due dates

## **Unit Tests**

* Written with **xUnit** and **Moq**
* Covers:

  * Task creation, update, completion, deletion
  * Task retrieval by ID
  * Task completion report
* Ensures handlers and controllers function as expected

---

## **Getting Started**

1. Clone the repository:

   ```bash
   git clone https://github.com/yourusername/task-tracker-api.git
   ```
2. Navigate to the project directory:

   ```bash
   cd TaskTracker.API
   ```
3. Update `appsettings.json` with your database connection string and JWT settings
4. Run the application:

   ```bash
   dotnet run
   ```
5. Use **Swagger** or Postman to test API endpoints

### **Authentication Flow Summary**

1. Users and managers register → receive credentials
2. Login → receive JWT token
3. Include token in `Authorization: Bearer <token>` header for all task endpoints
4. Handlers enforce **role-based permissions** automatically


This clearly documents:

* Roles and responsibilities
* Authentication & JWT usage
* CQRS structure
* Caching, validation, and unit testing
* All endpoints

