using Library_Management_System.Models;

using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Api.Data;

public class LibraryDbContext : DbContext
{
    public LibraryDbContext(DbContextOptions<LibraryDbContext> options)
        : base(options)
    {
    }

    public DbSet<Book> Books => Set<Book>();
    public DbSet<Author> Authors => Set<Author>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Publisher> Publishers => Set<Publisher>();
    public DbSet<BookAuthor> BookAuthors => Set<BookAuthor>();
    public DbSet<BookCategory> BookCategories => Set<BookCategory>();
    public DbSet<BookCopy> BookCopies => Set<BookCopy>();
    public DbSet<Member> Members => Set<Member>();
    public DbSet<SystemUser> SystemUsers => Set<SystemUser>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<SystemUserRole> SystemUserRoles => Set<SystemUserRole>();
    public DbSet<BorrowingTransaction> BorrowingTransactions => Set<BorrowingTransaction>();
    public DbSet<UserActivityLog> UserActivityLogs => Set<UserActivityLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(book => book.BookId);

            entity.Property(book => book.Title)
                .HasMaxLength(255)
                .IsRequired();

            entity.Property(book => book.Isbn)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(book => book.Summary)
                .HasColumnType("nvarchar(max)");

            entity.Property(book => book.CoverImageUrl)
                .HasMaxLength(500);

            entity.Property(book => book.Language)
                .HasMaxLength(50)
                .IsRequired();

            entity.HasOne(book => book.Publisher)
                .WithMany(publisher => publisher.Books)
                .HasForeignKey(book => book.PublisherId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Publisher>(entity =>
        {
            entity.HasKey(publisher => publisher.PublisherId);

            entity.Property(publisher => publisher.PublisherName)
                .HasMaxLength(255).IsRequired();
        });

        modelBuilder.Entity<Author>(entity =>
        {
            entity.HasKey(author => author.AuthorId);

            entity.Property(author => author.AuthorName)
                .HasMaxLength(255).IsRequired();
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(category => category.CategoryId);

            entity.Property(category => category.CategoryName)
                .HasMaxLength(100).IsRequired();

            entity.HasOne(category => category.ParentCategory)
                .WithMany(category => category.ChildCategories)
                .HasForeignKey(category => category.ParentCategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<BookAuthor>(entity =>
        {
            entity.HasKey(bookAuthor => new
            {
                bookAuthor.BookId,
                bookAuthor.AuthorId
            });

            entity.HasOne(bookAuthor => bookAuthor.Book)
                .WithMany(book => book.Authors)
                .HasForeignKey(bookAuthor => bookAuthor.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(bookAuthor => bookAuthor.Author)
                .WithMany(author => author.BookAuthors)
                .HasForeignKey(bookAuthor => bookAuthor.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<BookCategory>(entity =>
        {
            entity.HasKey(bookCategory => new
            {
                bookCategory.BookId,
                bookCategory.CategoryId
            });

            entity.HasOne(bookCategory => bookCategory.Book)
                .WithMany(book => book.Categories)
                .HasForeignKey(bookCategory => bookCategory.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(bookCategory => bookCategory.Category)
                .WithMany(category => category.BookCategories)
                .HasForeignKey(bookCategory => bookCategory.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<BookCopy>(entity =>
        {
            entity.HasKey(bookCopy => bookCopy.BookCopyId);

            entity.Property(bookCopy => bookCopy.Status)
                .HasConversion<int>();

            entity.Property(bookCopy => bookCopy.Location)
                .HasMaxLength(100);

            entity.HasOne(bookCopy => bookCopy.Book)
                .WithMany(book => book.Copies)
                .HasForeignKey(bookCopy => bookCopy.BookId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Member>(entity =>
        {
            entity.HasKey(member => member.MemberId);

            entity.Property(member => member.MembershipNumber)
                .HasMaxLength(50)
                .IsRequired();

            entity.HasIndex(member => member.MembershipNumber)
                .IsUnique();

            entity.Property(member => member.FirstName)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(member => member.LastName)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(member => member.PhoneNumber)
                .HasMaxLength(30);

            entity.Property(member => member.Email)
                .HasMaxLength(255);

            entity.Property(member => member.Address)
                .HasMaxLength(500);

            entity.Property(member => member.JoinDate)
                .HasColumnType("date");
        });

        modelBuilder.Entity<SystemUser>(entity =>
        {
            entity.HasKey(user => user.SystemUserId);

            entity.Property(user => user.Username)
                .HasMaxLength(100)
                .IsRequired();

            entity.HasIndex(user => user.Username)
                .IsUnique();

            entity.Property(user => user.Email)
                .HasMaxLength(255)
                .IsRequired();

            entity.Property(user => user.PasswordHash)
                .HasMaxLength(500)
                .IsRequired();

            entity.Property(user => user.CreatedAt)
                .HasColumnType("datetime2");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(role => role.RoleId);

            entity.Property(role => role.RoleName)
                .HasMaxLength(50)
                .IsRequired();

            entity.HasIndex(role => role.RoleName)
                .IsUnique();
        });

        modelBuilder.Entity<SystemUserRole>(entity =>
        {
            entity.HasKey(userRole => new
            {
                userRole.SystemUserId,
                userRole.RoleId
            });

            entity.HasOne(userRole => userRole.SystemUser)
                .WithMany(user => user.SystemUserRoles)
                .HasForeignKey(userRole => userRole.SystemUserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(userRole => userRole.Role)
                .WithMany(role => role.SystemUserRoles)
                .HasForeignKey(userRole => userRole.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<BorrowingTransaction>(entity =>
        {
            entity.HasKey(transaction => transaction.TransactionId);

            entity.Property(transaction => transaction.BorrowedAt)
                .HasColumnType("datetime2");

            entity.Property(transaction => transaction.DueAt)
                .HasColumnType("datetime2");

            entity.Property(transaction => transaction.ReturnedAt)
                .HasColumnType("datetime2");

            entity.HasOne(transaction => transaction.Member)
                .WithMany(member => member.BorrowingTransactions)
                .HasForeignKey(transaction => transaction.MemberId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(transaction => transaction.BookCopy)
                .WithMany(bookCopy => bookCopy.BorrowingTransactions)
                .HasForeignKey(transaction => transaction.BookCopyId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(transaction => transaction.IssuedByUser)
                .WithMany(user => user.IssuedTransactions)
                .HasForeignKey(transaction => transaction.IssuedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(transaction => transaction.ReceivedByUser)
                .WithMany(user => user.ReceivedTransactions)
                .HasForeignKey(transaction => transaction.ReceivedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<UserActivityLog>(entity =>
        {
            entity.HasKey(log => log.ActivityLogId);

            entity.Property(log => log.Action)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(log => log.CreatedAt)
                .HasColumnType("datetime2");

            entity.Property(log => log.TargetEntityType)
                .HasMaxLength(100);

            entity.Property(log => log.Notes)
                .HasColumnType("nvarchar(max)");

            entity.HasOne(log => log.SystemUser)
                .WithMany(user => user.ActivityLogs)
                .HasForeignKey(log => log.SystemUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
