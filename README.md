# StorageHub Documentation

Welcome to the official documentation for the StorageHub. This document provides comprehensive information on how to integrate and use the StorageHub.

## Overview

StorageHub is a comprehensive storage unit management system built using modern web technologies, including React.js for the frontend and .NET Core for the backend. The project aims to simplify the process of managing and tracking storage units for businesses and individuals. By leveraging the power of React and .NET Core, StorageHub provides users with a robust and scalable solution for organizing, monitoring, and optimizing storage spaces.

## Getting started

### Prerequisites

* Node https://nodejs.org/en/
* NPM
* Docker https://www.docker.com/
 * Install Docker Desktop for MAC: https://docs.docker.com/docker-for-mac/install/
 * Install Docker Desktop for Windows: https://docs.docker.com/docker-for-windows/install/
* Docker Compose
* Microsoft SQL Server
* .NET 8.0 sdk

### Setup

To get started, download the project zip to your local computer. Unzip the project folder.

#### StorageHub .Net backend
```bash
# Move to project directory
$ cd storagehub-backend
# Build docker images
$ docker-compose build
# Start docker containers
$ docker-compose up -d
# Create StorageHub database
$ sqlcmd -S host.docker.internal,5050 -U sa -P P@ssw0rd123 -Q "CREATE DATABASE StorageHub;"
# Add database migration
$ cd src
$ cd StorageHub.Api
$ dotnet ef migrations add initialCreate
$ dotnet ef database update 
# Seed the database with initial data
$ dotnet run seeddata
```

#### StorageHub React fronend
```bash
# Move to project directory
$ cd storagehubFront
# Install the required npm modules
$ npm install
```

#### Running The App

Make sure that the Api is already running after docker-compose up. You will find the swagger UI on url: https://localhost:5001/swagger/index.html

```bash
# Start application
$ npm start
```

### Project Structure

Storagehub .net 

#### .Net backend structure

| Name     | Description        |
|--------------|------------------|
| src      | Contains all the project files          |
| src/StorageHub.Api      | This directory contains the StorageHub Controller folder and any relevant API routes. It serves as the entry point for handling HTTP requests and directing them to the appropriate controller actions.         |
| src/StorageHub.Api/properties      | Contains the launchsettings.json file          |
| src/StorageHub.Api/Controllers      | This directory contains controller files responsible for handling incoming HTTP requests, processing data, and generating appropriate responses. These controllers define the API endpoints and the corresponding actions to be taken when these endpoints are accessed.         |
| src/StorageHub.Api/Dto      | Within this directory are Data Transfer Objects (DTOs) used to clarify the structure of request payloads sent to the API endpoints. These DTOs help standardize data communication between the client and server by defining the format of data being sent or received.          |
| src/StorageHub.Api/Helper     | This directory contains helper files that provide additional functionality or processing logic to streamline the handling of incoming requests within the API controllers. Helpers may include utility functions, middleware, or other resources that aid in request processing and response generation.          |
| src/StorageHub.ServiceLibrary      | This directory serves as the module file for the StorageHub service library, encompassing the logic and functionality related to the application's core services. It encapsulates services, data access, and business logic essential for the application's operation.         |
| src/StorageHub.ServiceLibrary/Data      | Contains the datacontext file, that is necessary for Entity         |
| src/StorageHub.ServiceLibrary/Entities      |  This directory contains entity classes representing various data structures or database tables used within the application. These entities define the schema and relationships between different data entities, providing a structured representation of the application's data model.        |
| src/StorageHub.ServiceLibrary/Extensions     | Here, you'll find extensions that provide additional functionality or features to the core services within the application. These extensions may include utility methods, helper functions, or other modules that extend the capabilities of the application.          |
| src/StorageHub.ServiceLibrary/Migrations      | Within this directory are files related to database migrations, which manage changes to the database schema over time. Migrations help ensure that the database structure evolves alongside the application's requirements, enabling seamless updates and data management.          |
| src/StorageHub.ServiceLibrary/Repositories      | This directory contains repositories responsible for interacting with the database and performing CRUD (Create, Read, Update, Delete) operations on data entities. Repositories abstract the data access layer, providing a clean separation between data access logic and the rest of the application's business logic.         |

#### React frontend structure

| Name     | Description        |
|--------------|------------------|
| src      | This directory serves as the root of the frontend source code. It contains all the files and directories related to the React application.          |
| src/assets      | Within this directory, you'll find static assets such as images, fonts, or other media files used in the frontend application. These assets can be referenced and included in components or pages as needed.        |
| src/components      | This directory houses reusable UI components that are used across different parts of the application. Components encapsulate UI elements and functionality, promoting code reusability and maintainability.          |
| src/helpers      |  Here, you'll find helper functions or utility modules that provide common functionality or tasks used throughout the application. Helpers can include functions for data formatting, validation, or other miscellaneous tasks.         |
| src/hooks      | This directory contains custom React hooks, which are reusable functions that enable component logic to be shared across different components. Hooks allow for the encapsulation of stateful logic and side effects within functional components.         |
| src/pages      | Within this directory are individual page components representing different views or routes of the application. Each page component typically corresponds to a specific URL route and encapsulates the UI and logic for that particular page.          |
| src/services     | This directory houses helper functions for interaction with the backend          |


## Documentation

### 1. Application urls

#### StorageHub Api 

The StorageHub Api starts on https://localhost:5051. This also includes Swagger UI api documentation which is on url: https://localhost:5051/swagger/index.html.

#### StorageHub Frontend

The StorageHub frontend starts on url http://localhost:3000 by default.


### 2. StorageHub Application Documentation

#### Homepage
##### /
The homepage serves as the entry point of the application. Homepage describes some of the key features of the application.

#### Register page
##### /register
The register page allows new users to create an account or register for the platform by providing their personal information, such as username, email address, and password.

#### Login Page
##### /login
The login page provides a form for existing users to log in to their accounts by entering their credentials, username and password.

#### Update Password Page
##### /update-password
On this page it is possible to update users password by providing 

#### Storage Page
##### /storages
The storage page displays all of the current users main storage units, allowing users to view details, manage their storage units, or perform update,delete and create actions. Also it is possible to search for storages(the search does not include only main storages it searches from all of the users storages).

#### Profile Page
##### /user
The profile page presents a user's profile information, including personal details, stats and action buttons(delete user, and update username).

#### Create Storage Page
##### /create-storage
The create storage page enables users to create new storage units providing necessary details: title, serialnumber, sizes, image, and access control.

#### Update Storage Page
##### /update-storage/:id
The update storage page enables users to update storage units providing necessary details: title, serialnumber, sizes, image, and access control.

#### Admin Login Page
##### /admin-login
The admin login page provides access for administrators to log in to the administrative dashboard or backend system. Normal users cannot log into admin.

#### Admin UsersList Page
##### /admin-users
The admin users list page displays a list of users registered on the platform, allowing administrators to view user details, delete users, or search for users.

#### Admin User Details Page
##### /admin-user/:id
This page displays specific users details and key stats.

#### Admin All Storages Page
##### /admin-storages
The storage page displays all of the main storage units, allowing the admin to view storage details, manage the storage units: perform update,delete. Also it is possible to search for storages(the search does not include only main storages it searches from all of the storages).

### 3. StorageHub Api documentation

Please look for Api documentation on backend url https://localhost:5051/swagger/index.html

### Future Enhancements

The application is missing styling for mobile views. But this is something that I am definetly working on adding.


## Conclusion

This project was a good chance to sharpen my software development skills. I think the project turned out great, but I see some ways that it could be improved. If you have any questions or issues, please contact me hendrik.metsallik@gmail.com
