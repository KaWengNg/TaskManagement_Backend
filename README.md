# Project overview
##### This is a simple project for task management system which allows to create, retrieve, edit, and delete the task. It is build using the .NetCore with .Net Framework version 6.0 for the backend and Vite tools to build the Web front-end using TypeScript and React.  

## Prerequisites (required SDK versions, Node.js version, etc.)

## Setup instructions for both backend and frontend

## How to run the application locally
##### The application can be run locally using the visual studio code (front-end) and visual studio IDE (back-end) with necessary libraries being installed.

## How to run tests (if implemented)
##### I created the xunit test project that can be run seperated by referencing to the main project.

## Assumptions or design decisions you made
##### Assumming when project is growing it will need to have service layer to handle the business logic, and by considering the unit test, I will separate the services to different layer in order to test via Dependency Injection.

## List of bonus features implemented :
##### 1. Data Annotation for input validation.
##### 2. Integrate Swagger for automatic APIdocumentation.
##### 3. Implement logging with Serilog for key operations.
##### 4. Write tests using xUnit for one service method.
##### 5. Implement separation of concerns with Controller, Service, Data Access Layers.
##### 6. Configure CORS for development
##### 7. Use Data Transfer Objects with AutoMapper.
##### 8. Use RateLimit to limit incoming requests per IP.