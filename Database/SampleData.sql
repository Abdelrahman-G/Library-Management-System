USE LibraryManagementDb;
GO

IF NOT EXISTS (SELECT 1 FROM Roles WHERE RoleName = 'Administrator')
    INSERT INTO Roles (RoleName) VALUES ('Administrator');
IF NOT EXISTS (SELECT 1 FROM Roles WHERE RoleName = 'Librarian')
    INSERT INTO Roles (RoleName) VALUES ('Librarian');
IF NOT EXISTS (SELECT 1 FROM Roles WHERE RoleName = 'Staff')
    INSERT INTO Roles (RoleName) VALUES ('Staff');

IF NOT EXISTS (SELECT 1 FROM Publishers WHERE PublisherName = 'Manning Publications')
    INSERT INTO Publishers (PublisherName) VALUES ('Manning Publications');

IF NOT EXISTS (SELECT 1 FROM Authors WHERE AuthorName = 'Jon Skeet')
    INSERT INTO Authors (AuthorName) VALUES ('Jon Skeet');
IF NOT EXISTS (SELECT 1 FROM Authors WHERE AuthorName = 'Robert C. Martin')
    INSERT INTO Authors (AuthorName) VALUES ('Robert C. Martin');

IF NOT EXISTS (SELECT 1 FROM Categories WHERE CategoryName = 'Technology' AND ParentCategoryId IS NULL)
    INSERT INTO Categories (CategoryName, ParentCategoryId) VALUES ('Technology', NULL);

DECLARE @TechnologyId int = (
    SELECT TOP (1) CategoryId FROM Categories
    WHERE CategoryName = 'Technology' AND ParentCategoryId IS NULL
);

IF NOT EXISTS (SELECT 1 FROM Categories WHERE CategoryName = 'Programming' AND ParentCategoryId = @TechnologyId)
    INSERT INTO Categories (CategoryName, ParentCategoryId) VALUES ('Programming', @TechnologyId);

IF NOT EXISTS (SELECT 1 FROM Members WHERE MembershipNumber = 'MEM-001')
BEGIN
    INSERT INTO Members
        (MembershipNumber, FirstName, LastName, Email, PhoneNumber, DateOfBirth, Address, JoinDate)
    VALUES
        ('MEM-001', 'Nour', 'Hassan', 'nour.hassan@example.com', '01000000001', '1998-05-12', 'Cairo', CAST(GETDATE() AS date));
END;

DECLARE @PublisherId int = (SELECT TOP (1) PublisherId FROM Publishers WHERE PublisherName = 'Manning Publications');
DECLARE @AuthorId int = (SELECT TOP (1) AuthorId FROM Authors WHERE AuthorName = 'Jon Skeet');
DECLARE @ProgrammingId int = (SELECT TOP (1) CategoryId FROM Categories WHERE CategoryName = 'Programming' AND ParentCategoryId = @TechnologyId);

IF NOT EXISTS (SELECT 1 FROM Books WHERE Isbn = '9781617294532')
BEGIN
    INSERT INTO Books
        (Title, Isbn, Summary, CoverImageUrl, Edition, PublicationYear, Language, PublisherId)
    VALUES
        ('C# in Depth', '9781617294532', 'A detailed guide to modern C#.', NULL, 4, 2019, 'English', @PublisherId);
END;

DECLARE @BookId int = (SELECT TOP (1) BookId FROM Books WHERE Isbn = '9781617294532');

IF NOT EXISTS (SELECT 1 FROM BookAuthors WHERE BookId = @BookId AND AuthorId = @AuthorId)
    INSERT INTO BookAuthors (BookId, AuthorId, AuthorOrder) VALUES (@BookId, @AuthorId, 1);

IF NOT EXISTS (SELECT 1 FROM BookCategories WHERE BookId = @BookId AND CategoryId = @ProgrammingId)
    INSERT INTO BookCategories (BookId, CategoryId) VALUES (@BookId, @ProgrammingId);

IF NOT EXISTS (SELECT 1 FROM BookCopies WHERE BookId = @BookId AND Location = 'Shelf A-01')
    INSERT INTO BookCopies (BookId, Status, Location) VALUES (@BookId, 0, 'Shelf A-01');
GO
