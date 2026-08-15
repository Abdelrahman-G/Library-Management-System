# Library Management System

## Main features

- CRUD operations for books, copies, authors, publishers, categories, members, roles, and system users
- Checkout, return, active borrowing, and member borrowing history
- Search by book title, author, and category
- Book filtering by `In` or `Out` availability
- JWT authentication with Administrator, Librarian, and Staff roles
- Activity logging for important operations

## Architecture

The application uses a simple controller-service structure. Controllers handle HTTP requests and authorization, services contain the application logic, and `LibraryDbContext` provides database access through Entity Framework Core. DTOs are used for request and response data instead of exposing database entities directly.

## Database design

Books and physical copies are stored separately. A book contains shared metadata, while each copy has its own location and availability. Borrowing transactions reference a specific copy and record the member and the employees who processed checkout and return.

Books can have multiple authors and categories through the `BookAuthor` and `BookCategory` junction tables. Categories support parent-child relationships through `ParentCategoryId`, and each book belongs to one publisher.

Members and system users are separate because members borrow books while system users operate the application. System users receive roles through the `SystemUserRole` junction table. Passwords are hashed, and JWT token versions allow logout, role changes, and session termination to invalidate older tokens.
