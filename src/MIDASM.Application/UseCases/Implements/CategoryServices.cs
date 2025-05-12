using MIDASM.Application.Commons.Errors;
using MIDASM.Application.Commons.Mapping;
using MIDASM.Application.Commons.Models.Categories;
using MIDASM.Application.Services.AuditLogServices;
using MIDASM.Application.Services.Authentication;
using MIDASM.Application.UseCases.Interfaces;
using MIDASM.Contract.Helpers;
using MIDASM.Contract.Messages.AuditLogMessage;
using MIDASM.Contract.Messages.Commands;
using MIDASM.Domain.Entities;
using MIDASM.Domain.Repositories;
using MIDASM.Domain.SharedKernel;

namespace MIDASM.Application.UseCases.Implements;

public class CategoryServices(ICategoryRepository categoryRepository,
    IBookRepository bookRepository,
    IUnitOfWork unitOfWork,
    IAuditLogger auditLogger,
    IExecutionContext executionContext)
    : ICategoryServices
{
    public async Task<Result<PaginationResult<CategoryResponse>>> GetCategoriesAsync(CategoriesQueryParameters queryParameters)
    {
        var query = categoryRepository.GetQueryable();
        var querySpecification = new CategoryByQueryParametersSpecification(queryParameters);

        query = querySpecification.GetQuery(query);

        var totalCount = await query.CountAsync();

        var data = await query
            .Skip(queryParameters.PageSize * (queryParameters.PageIndex - 1))
            .Take(queryParameters.PageSize)
            .Select(c => c.ToCategoryResponse())
            .ToListAsync();
        return PaginationResult<CategoryResponse>.Create(queryParameters.PageSize,
            queryParameters.PageIndex, totalCount, data);
    }
    public async Task<Result<PaginationResult<CategoryResponse>>> GetCategoriesAsync()
    {
        var query = categoryRepository.GetQueryable();

        var totalCount = await query.CountAsync();
        var data = await query
            .Select(c => c.ToCategoryResponse())
            .ToListAsync();
        return PaginationResult<CategoryResponse>.Create(totalCount, data);
    }
    public async Task<Result<CategoryResponse>> GetCategoryByIdAsync(Guid id)
    {
        var category = await categoryRepository.GetByIdAsync(id);

        return category?.ToCategoryResponse() ?? default!;
    }

    public async Task<Result<string>> CreateCategoryAsync(CategoryCreateRequest createRequest)
    {
        if (await IsCategoryNameExists(createRequest.Name))
        {
            return Result<string>.Failure(CategoryErrors.CategoryNameExists);
        }
        var category = Category.Create(createRequest.Name, createRequest.Description);

        await AddCategoryIntoStorageAsync(category);

        await HandleAuditLogCreateCategory(category);

        return CategoryCommandMessages.CategoryCreatedSuccess;
    }

    public async Task<Result<string>> UpdateCategoryAsync(CategoryUpdateRequest updateRequest)
    {
        var category = await categoryRepository.GetByIdAsync(updateRequest.Id);

        if (category == null)
        {
            return Result<string>.Failure(400, CategoryErrors.CategoryNotFound);
        }
        var oldCategory = Category.Copy(category);

        Category.Update(category, updateRequest.Name, updateRequest.Description);
        await categoryRepository.SaveChangesAsync();

        await HandleAuditLogUpdateCategory(category, oldCategory);
        return CategoryCommandMessages.CategoryUpdatedSuccess;
    }

    public async Task<Result<string>> DeleteCategoryAsync(Guid id)
    {
        var category = await categoryRepository.GetByIdAsync(id, c => c.Books!.Where(b => b.Quantity > b.Available));

        if (category == null)
        {
            return Result<string>.Failure(CategoryErrors.CategoryNotFound);
        }

        if (category.Books?.Count > 0)
        {
            return Result<string>.Failure(CategoryErrors.CategoryCanNotDelete);
        }
        try
        {
            await unitOfWork.BeginTransactionAsync();

            await DeleteBooksOfCategoryAsync(category);

            var oldCategory = Category.Copy(category);

            await SoftDeleteCategoryFromStorageAsync(category);

            await unitOfWork.CommitTransactionAsync();

            await HandleAuditLogDeleteCategory(category, oldCategory);

            return CategoryCommandMessages.CategoryDeletedSuccess;

        }
        catch
        {
            await unitOfWork.RollbackAsync();
            return Result<string>.Failure(CategoryErrors.CategoryDeleteFail);
        }
    }


    private async Task<bool> IsCategoryNameExists(string categoryName)
    {
        var category = await categoryRepository.GetByNameAsync(categoryName);
        return category != null;
    }
    private async Task DeleteBooksOfCategoryAsync(Category category)
    {
        var books = await bookRepository.GetQueryable().Where(b => b.CategoryId == category.Id).ToListAsync();
        books.ForEach(book => book.IsDeleted = true);
        bookRepository.UpdateRange(books);
        await bookRepository.SaveChangesAsync();
    }


    private async Task HandleAuditLogCreateCategory(Category category)
    {
        await auditLogger.LogAsync(
            category.Id.ToString(),
            nameof(Category),
            StringHelper.ReplacePlaceholders(
                AuditLogMessageTemplate.Create,
                executionContext.GetUserName(),
                nameof(Category).ToLower(),
                category.Name,
                category.CreatedAt.ToShortTime()
            ),
            GetChangingProperties(category)
            );
    }

    private async Task HandleAuditLogUpdateCategory(Category category, Category oldCategory)
    {
        var changingProperties = GetChangingProperties(category, oldCategory);
        await auditLogger.LogAsync(
            category.Id.ToString(),
            nameof(Category),
            StringHelper.ReplacePlaceholders(
                AuditLogMessageTemplate.Update,
                executionContext.GetUserName(),
                nameof(Category).ToLower(),
                category.Name,
                category.ModifiedAt!.ToShortTime(),
                StringHelper.SerializePropertiesChanges(changingProperties)
            ),
            changingProperties
            );
    }

    private async Task HandleAuditLogDeleteCategory(Category category, Category oldCategory)
    {
        await auditLogger.LogAsync(
            category.Id.ToString(),
            nameof(Category),
            StringHelper.ReplacePlaceholders(
                AuditLogMessageTemplate.Delete,
                executionContext.GetUserName(),
                nameof(Category).ToLower(),
                category.Name,
                category.ModifiedAt.ToShortTime()
            ),
            GetChangingProperties(category, oldCategory)
            );
    }

    private static Dictionary<string, (string?, string?)> GetChangingProperties(Category category, Category? oldCategory = default)
    {
        var newProperties = new Dictionary<string, (string?, string?)>();
        if (oldCategory == null || category.Name != oldCategory?.Name)
        {
            newProperties.Add(nameof(category.Name), (oldCategory?.Name, category.Name));
        }
        if (oldCategory == null || category.Description != oldCategory?.Description)
        {
            newProperties.Add(nameof(category.Description), (oldCategory?.Description, category.Description));
        }
        if (oldCategory == null || category.IsDeleted != oldCategory?.IsDeleted)
        {
            newProperties.Add(nameof(category.IsDeleted), (oldCategory?.IsDeleted.ToString(), category.IsDeleted.ToString()));
        }
        return newProperties;
    }


    private async Task AddCategoryIntoStorageAsync(Category category)
    {
        categoryRepository.Add(category);
        await categoryRepository.SaveChangesAsync();
    }

    private async Task SoftDeleteCategoryFromStorageAsync(Category category)
    {
        category.IsDeleted = true;
        categoryRepository.Update(category);
        await categoryRepository.SaveChangesAsync();

    }
}
