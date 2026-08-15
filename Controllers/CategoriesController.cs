using Library_Management_System.DTOs.Categories;
using Library_Management_System.Services.Interfaces;
using Library_Management_System.Services.Results;
using Library_Management_System.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library_Management_System.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _service;
    public CategoriesController(ICategoryService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CategoryResponse>>> GetAll(CancellationToken token) => Ok(await _service.GetAllAsync(token));

    [HttpGet("{categoryId:int}")]
    public async Task<ActionResult<CategoryResponse>> GetById(int categoryId, CancellationToken token)
    {
        var category = await _service.GetByIdAsync(categoryId, token);
        return category is null ? NotFound() : Ok(category);
    }

    [Authorize(Roles = RoleNames.AdministratorOrLibrarian)]
    [HttpPost]
    public async Task<ActionResult<CategoryResponse>> Create(CreateCategoryRequest request, CancellationToken token)
    {
        var category = await _service.CreateAsync(request, token);
        if (category is null) return BadRequest(new { message = "ParentCategoryId does not identify an existing category." });
        return CreatedAtAction(nameof(GetById), new { categoryId = category.CategoryId }, category);
    }

    [Authorize(Roles = RoleNames.AdministratorOrLibrarian)]
    [HttpPut("{categoryId:int}")]
    public async Task<IActionResult> Update(int categoryId, UpdateCategoryRequest request, CancellationToken token)
    {
        return await _service.UpdateAsync(categoryId, request, token) switch
        {
            UpdateResult.Success => NoContent(),
            UpdateResult.NotFound => NotFound(),
            UpdateResult.InvalidReference => BadRequest(new { message = "ParentCategoryId is invalid." }),
            _ => throw new InvalidOperationException()
        };
    }

    [Authorize(Roles = RoleNames.AdministratorOrLibrarian)]
    [HttpDelete("{categoryId:int}")]
    public async Task<IActionResult> Delete(int categoryId, CancellationToken token)
    {
        return await _service.DeleteAsync(categoryId, token) switch
        {
            DeleteResult.Success => NoContent(),
            DeleteResult.NotFound => NotFound(),
            DeleteResult.HasDependencies => Conflict(new { message = "The category cannot be deleted while child categories or books reference it." }),
            _ => throw new InvalidOperationException()
        };
    }
}

