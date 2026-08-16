USE LibraryManagementDb;
GO

-- Reviewer login: admin / admin

IF NOT EXISTS (SELECT 1 FROM Roles WHERE RoleName = 'Administrator')
    INSERT INTO Roles (RoleName) VALUES ('Administrator');

IF NOT EXISTS (SELECT 1 FROM Roles WHERE RoleName = 'Librarian')
    INSERT INTO Roles (RoleName) VALUES ('Librarian');

IF NOT EXISTS (SELECT 1 FROM Roles WHERE RoleName = 'Staff')
    INSERT INTO Roles (RoleName) VALUES ('Staff');

IF NOT EXISTS (SELECT 1 FROM SystemUsers WHERE Username = 'admin')
BEGIN
    INSERT INTO SystemUsers
        (Username, Email, PasswordHash, IsActive, CreatedAt, TokenVersion)
    VALUES
        ('admin', 'admin@library.test',
         'AQAAAAIAAYagAAAAEVJldmlld2VyQWRtaW5TYWx059Q/oltkuOjtINJ44dTIQfG1Lp+1fmyTPtMoSAHw2E4=',
         1, SYSUTCDATETIME(), 0);
END;

IF NOT EXISTS
(
    SELECT 1
    FROM SystemUserRoles
    JOIN SystemUsers ON SystemUserRoles.SystemUserId = SystemUsers.SystemUserId
    JOIN Roles ON SystemUserRoles.RoleId = Roles.RoleId
    WHERE SystemUsers.Username = 'admin'
      AND Roles.RoleName = 'Administrator'
)
BEGIN
    INSERT INTO SystemUserRoles (SystemUserId, RoleId)
    SELECT SystemUsers.SystemUserId, Roles.RoleId
    FROM SystemUsers CROSS JOIN Roles
    WHERE SystemUsers.Username = 'admin'
      AND Roles.RoleName = 'Administrator';
END;

-- Sample data

INSERT INTO Publishers (PublisherName)
VALUES
    (N'دار نهضة مصر'),
    (N'المركز الثقافي العربي'),
    (N'دار دون');

INSERT INTO Authors (AuthorName)
VALUES
    (N'علاء الأسواني'),
    (N'يوسف زيدان'),
    (N'أحمد مراد'),
    (N'إحسان عبد القدوس');

INSERT INTO Categories (CategoryName, ParentCategoryId)
VALUES
    (N'الدراسات الإنسانية', NULL),
    (N'الرواية المعاصرة', NULL);

INSERT INTO Categories (CategoryName, ParentCategoryId)
VALUES
    (N'علم النفس',
     (SELECT TOP 1 CategoryId FROM Categories
      WHERE CategoryName = N'الدراسات الإنسانية' ORDER BY CategoryId DESC)),
    (N'الغموض والتشويق',
     (SELECT TOP 1 CategoryId FROM Categories
      WHERE CategoryName = N'الرواية المعاصرة' ORDER BY CategoryId DESC)),
    (N'الرواية التاريخية الحديثة',
     (SELECT TOP 1 CategoryId FROM Categories
      WHERE CategoryName = N'الرواية المعاصرة' ORDER BY CategoryId DESC));

INSERT INTO Members
    (MembershipNumber, FirstName, LastName, Email, PhoneNumber,
     DateOfBirth, Address, JoinDate)
VALUES
    ('MEM-101', 'Omar', 'Saleh', 'omar.saleh@example.com', '01000000101',
     '1996-04-18', 'Cairo', '2026-01-12'),
    ('MEM-102', 'Salma', 'Adel', 'salma.adel@example.com', '01000000102',
     '1999-11-02', 'Giza', '2026-02-08'),
    ('MEM-103', 'Youssef', 'Ibrahim', 'youssef.ibrahim@example.com', '01000000103',
     '2001-06-25', 'Alexandria', '2026-03-15'),
    ('MEM-104', 'Mona', 'Ahmed', 'mona.ahmed@example.com', '01000000104',
     '1997-09-14', 'Mansoura', '2026-04-20');

INSERT INTO Books
    (Title, Isbn, Summary, CoverImageUrl, Edition,
     PublicationYear, Language, PublisherId)
SELECT
    N'عمارة يعقوبيان', '9789774248627', N'رواية اجتماعية معاصرة.', NULL,
    1, 2002, 'Arabic', PublisherId
FROM Publishers
WHERE PublisherName = N'دار نهضة مصر';

INSERT INTO Books
    (Title, Isbn, Summary, CoverImageUrl, Edition,
     PublicationYear, Language, PublisherId)
SELECT
    N'عزازيل', '9789953876030', N'رواية تاريخية وفكرية.', NULL,
    1, 2008, 'Arabic', PublisherId
FROM Publishers
WHERE PublisherName = N'المركز الثقافي العربي';

INSERT INTO Books
    (Title, Isbn, Summary, CoverImageUrl, Edition,
     PublicationYear, Language, PublisherId)
SELECT
    N'الفيل الأزرق', '9789770931066', N'رواية غموض وتشويق.', NULL,
    1, 2012, 'Arabic', PublisherId
FROM Publishers
WHERE PublisherName = N'دار دون';

INSERT INTO Books
    (Title, Isbn, Summary, CoverImageUrl, Edition,
     PublicationYear, Language, PublisherId)
SELECT
    N'لا أنام', '9789777950954', N'رواية نفسية واجتماعية.', NULL,
    1, 1969, 'Arabic', PublisherId
FROM Publishers
WHERE PublisherName = N'دار نهضة مصر';

INSERT INTO BookAuthors (BookId, AuthorId, AuthorOrder)
SELECT BookId, AuthorId, 1
FROM Books CROSS JOIN Authors
WHERE Title = N'عمارة يعقوبيان' AND AuthorName = N'علاء الأسواني';

INSERT INTO BookAuthors (BookId, AuthorId, AuthorOrder)
SELECT BookId, AuthorId, 1
FROM Books CROSS JOIN Authors
WHERE Title = N'عزازيل' AND AuthorName = N'يوسف زيدان';

INSERT INTO BookAuthors (BookId, AuthorId, AuthorOrder)
SELECT BookId, AuthorId, 1
FROM Books CROSS JOIN Authors
WHERE Title = N'الفيل الأزرق' AND AuthorName = N'أحمد مراد';

INSERT INTO BookAuthors (BookId, AuthorId, AuthorOrder)
SELECT BookId, AuthorId, 1
FROM Books CROSS JOIN Authors
WHERE Title = N'لا أنام' AND AuthorName = N'إحسان عبد القدوس';

INSERT INTO BookCategories (BookId, CategoryId)
SELECT BookId, CategoryId
FROM Books CROSS JOIN Categories
WHERE Title = N'عمارة يعقوبيان' AND CategoryName = N'الرواية المعاصرة';

INSERT INTO BookCategories (BookId, CategoryId)
SELECT BookId, CategoryId
FROM Books CROSS JOIN Categories
WHERE Title = N'عزازيل' AND CategoryName = N'الرواية التاريخية الحديثة';

INSERT INTO BookCategories (BookId, CategoryId)
SELECT BookId, CategoryId
FROM Books CROSS JOIN Categories
WHERE Title = N'الفيل الأزرق' AND CategoryName = N'الغموض والتشويق';

INSERT INTO BookCategories (BookId, CategoryId)
SELECT BookId, CategoryId
FROM Books CROSS JOIN Categories
WHERE Title = N'لا أنام' AND CategoryName = N'علم النفس';

INSERT INTO BookCopies (BookId, Status, Location)
SELECT BookId, 0, 'D-01-01' FROM Books WHERE Title = N'عمارة يعقوبيان';

INSERT INTO BookCopies (BookId, Status, Location)
SELECT BookId, 0, 'D-01-02' FROM Books WHERE Title = N'عمارة يعقوبيان';

INSERT INTO BookCopies (BookId, Status, Location)
SELECT BookId, 0, 'D-02-01' FROM Books WHERE Title = N'عزازيل';

INSERT INTO BookCopies (BookId, Status, Location)
SELECT BookId, 0, 'D-03-01' FROM Books WHERE Title = N'الفيل الأزرق';

INSERT INTO BookCopies (BookId, Status, Location)
SELECT BookId, 0, 'D-03-02' FROM Books WHERE Title = N'الفيل الأزرق';

INSERT INTO BookCopies (BookId, Status, Location)
SELECT BookId, 0, 'D-04-01' FROM Books WHERE Title = N'لا أنام';

-- Test queries

SELECT * FROM Publishers;
SELECT * FROM Authors;
SELECT * FROM Categories;
SELECT * FROM Books;
SELECT * FROM BookAuthors;
SELECT * FROM BookCategories;
SELECT * FROM BookCopies;
SELECT * FROM Members;
SELECT * FROM Roles;
SELECT * FROM SystemUsers;
SELECT * FROM SystemUserRoles;
SELECT * FROM BorrowingTransactions;
SELECT * FROM UserActivityLogs;

SELECT Books.Title, Publishers.PublisherName
FROM Books
JOIN Publishers ON Books.PublisherId = Publishers.PublisherId;

SELECT Books.Title, Authors.AuthorName
FROM BookAuthors
JOIN Books ON BookAuthors.BookId = Books.BookId
JOIN Authors ON BookAuthors.AuthorId = Authors.AuthorId;

SELECT Books.Title, Categories.CategoryName
FROM BookCategories
JOIN Books ON BookCategories.BookId = Books.BookId
JOIN Categories ON BookCategories.CategoryId = Categories.CategoryId;

SELECT
    Books.Title,
    COUNT(BookCopies.BookCopyId) AS TotalCopies,
    SUM(CASE WHEN BookCopies.Status = 0 THEN 1 ELSE 0 END) AS AvailableCopies
FROM Books
LEFT JOIN BookCopies ON Books.BookId = BookCopies.BookId
GROUP BY Books.BookId, Books.Title;
GO
