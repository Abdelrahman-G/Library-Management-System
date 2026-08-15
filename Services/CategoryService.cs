using Library_Management_System.DTOs.Categories;
using Library_Management_System.Models;
using Library_Management_System.Services.Interfaces;
using Library_Management_System.Services.Results;
using LibraryManagement.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Library_Management_System.Services;

public class CategoryService : ICategoryService
{
    private readonly LibraryDbContext _context;
    private readonly IActivityLogService _activityLog;

    public CategoryService(LibraryDbContext context, IActivityLogService activityLog)
    {
        _context = context;
        _activityLog = activityLog;
    }

    public async Task<IReadOnlyList<CategoryResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await Query().OrderBy(category => category.CategoryName).ToListAsync(cancellationToken);
    }

    public async Task<CategoryResponse?> GetByIdAsync(int categoryId, CancellationToken cancellationToken = default)
    {
        return await Query().FirstOrDefaultAsync(category => category.CategoryId == categoryId, cancellationToken);
    }

    public async Task<CategoryResponse?> CreateAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        if (request.ParentCategoryId.HasValue &&
            !await _context.Categories.AnyAsync(category => category.CategoryId == request.ParentCategoryId.Value, cancellationToken))
            return null;

        var category = new Category
        {
            CategoryName = request.CategoryName.Trim(),
            ParentCategoryId = request.ParentCategoryId
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync(cancellationToken);
        _activityLog.Add(ActivityLogActions.CategoryCreated, nameof(Category), category.CategoryId);
        await _context.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(category.CategoryId, cancellationToken);
    }

    public async Task<UpdateResult> UpdateAsync(int categoryId, UpdateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        var category = await _context.Categories.FindAsync(new object[] { categoryId }, cancellationToken);
        if (category is null) return UpdateResult.NotFound;

        if (request.ParentCategoryId == categoryId) return UpdateResult.InvalidReference;
        if (request.ParentCategoryId.HasValue &&
            !await _context.Categories.AnyAsync(parent => parent.CategoryId == request.ParentCategoryId.Value, cancellationToken))
            return UpdateResult.InvalidReference;

        category.CategoryName = request.CategoryName.Trim();
        category.ParentCategoryId = request.ParentCategoryId;
        _activityLog.Add(ActivityLogActions.CategoryUpdated, nameof(Category), categoryId);
        await _context.SaveChangesAsync(cancellationToken);
        return UpdateResult.Success;
    }

    public async Task<DeleteResult> DeleteAsync(int categoryId, CancellationToken cancellationToken = default)
    {
        var category = await _context.Categories.FindAsync(new object[] { categoryId }, cancellationToken);
        if (category is null) return DeleteResult.NotFound;

        var hasDependencies = await _context.Categories.AnyAsync(child => child.ParentCategoryId == categoryId, cancellationToken)
            || await _context.BookCategories.AnyAsync(link => link.CategoryId == categoryId, cancellationToken);
        if (hasDependencies) return DeleteResult.HasDependencies;

        _context.Categories.Remove(category);
        _activityLog.Add(ActivityLogActions.CategoryDeleted, nameof(Category), categoryId);
        await _context.SaveChangesAsync(cancellationToken);
        return DeleteResult.Success;
    }

    private IQueryable<CategoryResponse> Query()
    {
        return _context.Categories.AsNoTracking().Select(category => new CategoryResponse
        {
            CategoryId = category.CategoryId,
            CategoryName = category.CategoryName,
            ParentCategoryId = category.ParentCategoryId,
            ParentCategoryName = category.ParentCategory == null ? null : category.ParentCategory.CategoryName,
            ChildCategoryCount = category.ChildCategories.Count,
            BookCount = category.BookCategories.Count
        });
    }
}

