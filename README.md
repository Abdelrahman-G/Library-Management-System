# Library Management System

## Main features

- CRUD operations for publishers, authors, categories, books, physical book copies, members, roles, and system users
- Multiple authors and categories per book
- Ordered book authors through `AuthorOrder`
- Hierarchical categories through a nullable parent-category relationship
- Physical-copy availability and location tracking
- Book checkout, return, active-loan listing, and member borrowing history
- JWT authentication and role-based authorization
- Administrator, Librarian, and Staff roles
- Token-version validation for logout and session invalidation
- User activity logging
- Book search by title, author, and category
- Book filtering by derived `In` or `Out` availability

## Architecture

The API uses three primary application layers:

```text
Controllers → Services → Entity Framework Core / SQL Server
                    ↘ DTOs and result types
```

- **Controllers** define routes, validate authentication/authorization, translate service results into HTTP responses, and do not contain database logic.
- **Services** contain application rules, queries, entity updates, and transaction handling.
- **DTOs** define API request and response contracts so database entities are not exposed directly.
- **Models** represent the relational database entities.
- **LibraryDbContext** maps entities and relationships and acts as the Entity Framework Core gateway to SQL Server.
- **Result types** allow services to report expected outcomes such as `NotFound`, invalid references, conflicts, and successful operations without depending on HTTP types.

This structure was selected because it is easy to explain, test, and maintain while remaining appropriately sized for the challenge.

## Database design choices

### Books and physical copies

`Book` stores shared bibliographic metadata such as title, ISBN, summary, edition, publication year, language, publisher, and cover-image URL.

`BookCopy` represents an individual physical copy. This separation allows one title to have multiple copies with independent locations and statuses. Borrowing transactions therefore reference a specific `BookCopy`, not only a title.

Book-level availability is derived from its copies:

- `In`: at least one copy has `Available` status.
- `Out`: the book has one or more copies, but none are available.
- A book with no physical copies is excluded from both results.

### Authors and categories

Books and authors have a many-to-many relationship through `BookAuthor`. The junction entity also stores `AuthorOrder` so authors can be displayed in the intended order.

Books and categories have a many-to-many relationship through `BookCategory`.

`Category.ParentCategoryId` is a nullable self-referencing foreign key. A null value represents a root category; otherwise, it points to the category's parent and supports a hierarchy such as `Technology → Programming`.

### Publishers

A publisher can publish many books, while each book references one publisher. Publisher deletion is restricted while books still reference it.

### Members and system users

Members are library borrowers. System users are employees who operate the API. They are separate entities because a borrower is not automatically an authenticated administrator, librarian, or staff member.

System users and roles have a many-to-many relationship through `SystemUserRole`.

### Borrowing transactions

A borrowing transaction connects:

- One member
- One physical book copy
- The system user who issued the copy
- The system user who received the return, when returned

Checkout and return use database transactions. Atomic conditional updates prevent two concurrent requests from borrowing or returning the same copy successfully.

### Activity logs

Activity logs record who performed a successful state-changing action, what action occurred, the affected entity type and identifier, and when it happened. Passwords, password hashes, JWTs, request bodies, failed validation, and routine GET requests are not logged.

Checkout and return logs are committed inside the same database transactions as their business operations.

## Authentication and authorization

Passwords are never stored directly. They are hashed and verified with ASP.NET Core `PasswordHasher<SystemUser>`.

Successful login produces a signed JWT containing the user's identifier, username, email, roles, and token version. Role claims are used by `[Authorize]` attributes on the controllers.

The token version is checked against the database on every authenticated request. It is incremented when:

- The user logs out
- The user's role assignments change
- An administrator terminates the user's sessions

This invalidates all previously issued JWTs for that user without storing the JWTs themselves. Refresh tokens are intentionally outside the scope of this challenge.

### Role permissions

| Capability | Administrator | Librarian | Staff |
|---|:---:|:---:|:---:|
| Read catalog and copy information | Yes | Yes | Yes |
| Create, update, and delete catalog data | Yes | Yes | No |
| Read, create, and update members | Yes | Yes | Yes |
| Delete members | Yes | Yes | No |
| Checkout, return, and view borrowing records | Yes | Yes | Yes |
| Manage system users and roles | Yes | No | No |
| Terminate user sessions | Yes | No | No |
| View activity logs | Yes | No | No |

## Important endpoints

### Authentication

| Method | Endpoint | Purpose |
|---|---|---|
| `POST` | `/api/auth/login` | Validate credentials and issue a JWT |
| `GET` | `/api/auth/me` | Return the authenticated user's claims |
| `POST` | `/api/auth/logout` | Increment the user's token version and invalidate existing JWTs |

### Books

| Method | Endpoint | Purpose |
|---|---|---|
| `GET` | `/api/books` | Return all books |
| `GET` | `/api/books/{bookId}` | Return one book |
| `GET` | `/api/books/search?name=&author=&category=` | Search using optional filters |
| `GET` | `/api/books/by-status/In` | Return books with an available copy |
| `GET` | `/api/books/by-status/Out` | Return books with copies but no available copy |
| `GET` | `/api/books/{bookId}/availability` | Return total and available copy counts |
| `GET` | `/api/books/{bookId}/copies` | Return every physical copy of a book |
| `POST` | `/api/books` | Create a book |
| `PUT` | `/api/books/{bookId}` | Update a book |
| `DELETE` | `/api/books/{bookId}` | Delete a book when no copies depend on it |

Publishers, authors, categories, book copies, members, roles, and system users expose corresponding RESTful CRUD endpoints.

### Borrowing

| Method | Endpoint | Purpose |
|---|---|---|
| `GET` | `/api/borrowings` | Return borrowing history |
| `GET` | `/api/borrowings/active` | Return active borrowing transactions |
| `GET` | `/api/borrowings/member/{memberId}` | Return one member's borrowing history |
| `POST` | `/api/borrowings/checkout` | Borrow a specific available copy |
| `POST` | `/api/borrowings/{transactionId}/return` | Return the borrowed copy |

### Administration

| Method | Endpoint | Purpose |
|---|---|---|
| `POST` | `/api/systemusers/{systemUserId}/terminate-sessions` | Invalidate all tokens belonging to a user |
| `GET` | `/api/activity-logs` | View activity logs |
| `GET` | `/api/activity-logs?systemUserId=` | Filter logs by the system user who performed the action |

## Setup

### Prerequisites

- .NET 8 SDK
- SQL Server
- Visual Studio 2022, VS Code, or another .NET-compatible IDE
- Optional: SQL Server Management Studio and Postman

### 1. Configure SQL Server

Set the `LibraryDatabase` connection string through user secrets or local configuration. Example:

```json
{
  "ConnectionStrings": {
    "LibraryDatabase": "Server=YOUR_SERVER;Database=LibraryManagementDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### 2. Configure JWT settings

Configure the issuer, audience, expiration, and a Base64-encoded signing key containing at least 32 random bytes:

```json
{
  "Jwt": {
    "Issuer": "LibraryManagement.Api",
    "Audience": "LibraryManagement.Client",
    "Key": "YOUR_BASE64_ENCODED_RANDOM_KEY",
    "ExpirationMinutes": 30
  }
}
```

Do not commit real connection credentials or production signing keys to the public repository. For local development, use .NET user secrets:

```powershell
dotnet user-secrets set "ConnectionStrings:LibraryDatabase" "YOUR_CONNECTION_STRING"
dotnet user-secrets set "Jwt:Key" "YOUR_BASE64_ENCODED_RANDOM_KEY"
```

### 3. Create the database

From Visual Studio's Package Manager Console:

```powershell
Update-Database
```

Or, when the Entity Framework CLI tool is installed:

```powershell
dotnet ef database update
```

The migrations create the complete schema and add token-version support.

### 4. Add sample data

Execute [`Database/SampleData.sql`](Database/SampleData.sql) inside `LibraryManagementDb`. The script safely inserts example roles, publishers, authors, hierarchical categories, a member, a book, junction records, and a physical copy.

The API intentionally has no anonymous registration endpoint. The first administrator should be created through a controlled bootstrap or seed process using a password hash generated by `IPasswordHasher<SystemUser>`. After bootstrap, system-user management is restricted to administrators.

### 5. Run the API

```powershell
dotnet run
```

The HTTPS Visual Studio profile normally starts at `https://localhost:7246`, with Swagger available at:

```text
https://localhost:7246/swagger
```

The exact URL is displayed in the application output and may vary by launch profile.

## Testing with Swagger or Postman

1. Call `POST /api/auth/login`.
2. Copy `accessToken` from the response.
3. In Swagger, select **Authorize** and paste the token.
4. In Postman, select **Bearer Token** authorization and paste the same token.
5. Test endpoints according to the authenticated user's role.

After logout, a role assignment change, or administrator session termination, the previous token should return `401 Unauthorized`.

## Repository contents

```text
Controllers/       HTTP routes and response mapping
Data/              DbContext and Entity Framework migrations
Database/          SQL sample-data scripts
DTOs/              Request and response contracts
Enums/             Book-copy and availability states
Models/            Database entities
Services/          Business logic and database queries
Authorization/     Central role-name constants
ERD/               Location for the exported entity-relationship diagram
```

## Assumptions and scope

- All stored timestamps are UTC.
- Cover images are represented by URLs rather than binary database data.
- ISBN is stored as text because it is an identifier, not a number used in arithmetic.
- Members and authenticated system users are intentionally separate.
- Borrowing operates on a selected physical copy.
- Deletes are rejected when dependent records would make deletion unsafe.
- JWT refresh-token rotation and per-device sessions are not included.
- Search results use the SQL Server database collation for case sensitivity.
