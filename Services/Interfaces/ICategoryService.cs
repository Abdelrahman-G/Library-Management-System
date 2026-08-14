using Library_Management_System.DTOs.Categories;
using Library_Management_System.Services.Results;

namespace Library_Management_System.Services.Interfaces;

public interface ICategoryService
{
    Task<IReadOnlyList<CategoryResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<CategoryResponse?> GetByIdAsync(int categoryId, CancellationToken cancellationToken = default);
    Task<CategoryResponse?> CreateAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default);
    Task<UpdateResult> UpdateAsync(int categoryId, UpdateCategoryRequest request, CancellationToken cancellationToken = default);
    Task<DeleteResult> DeleteAsync(int categoryId, CancellationToken cancellationToken = default);
}

