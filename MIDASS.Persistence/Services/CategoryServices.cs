using Mapster;
using Microsoft.EntityFrameworkCore;
using MIDASS.Application.Commons.Mapping;
using MIDASS.Application.Commons.Models.Categories;
using MIDASS.Application.UseCases;
using MIDASS.Contract.Errors;
using MIDASS.Contract.Messages.Commands;
using MIDASS.Contract.SharedKernel;
using MIDASS.Domain.Abstract;
using MIDASS.Domain.Entities;
using MIDASS.Domain.Repositories;
using MIDASS.Persistence.Specifications;
using System.Transactions;
namespace MIDASS.Persistence.Services;

public class CategoryServices(ICategoryRepository categoryRepository, IBookRepository bookRepository,
    ITransactionManager transactionManager)
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

    public async Task<Result<CategoryResponse>> GetCategoryByIdAsync(Guid id)
    {
        var category = await categoryRepository.GetByIdAsync(id);

        return category.Adapt<CategoryResponse>();
    }

    public async Task<Result<string>> CreateCategoryAsync(CategoryCreateRequest createRequest)
    {
        if (await IsCategoryNameExists(createRequest.Name))
        {
            return Result<string>.Failure(400, CategoryErrors.CategoryNameExists);
        }
        var category = Category.Create(createRequest.Name, createRequest.Description);
        categoryRepository.Add(category);
        await categoryRepository.SaveChangesAsync();

        return CategoryCommandMessages.CategoryCreatedSuccess;
    }

    public async Task<Result<string>> UpdateCategoryAsync(CategoryUpdateRequest updateRequest)
    {
        var category = await categoryRepository.GetByIdAsync(updateRequest.Id);

        if (category == null)
        {
            return Result<string>.Failure(400, CategoryErrors.CategoryNotFound);
        }

        Category.Update(category, updateRequest.Name, updateRequest.Description);

        await categoryRepository.SaveChangesAsync();

        return CategoryCommandMessages.CategoryUpdatedSuccess;
    }

    public async Task<Result<string>> DeleteCategoryAsync(Guid id)
    {
        var category = await categoryRepository.GetByIdAsync(id, c => c.Books!.Where(b => b.Quantity > b.Available));

        if (category == null)
        {
            return Result<string>.Failure(400, CategoryErrors.CategoryNotFound);
        }

        if (category.Books?.Count > 0)
        {
            return Result<string>.Failure(400, CategoryErrors.CategoryCanNotDelete);
        }
        try
        {
            await transactionManager.BeginTransactionAsync();
            await DeleteBookOfCategory(category);
            category.IsDeleted = true;
            categoryRepository.Update(category);
            await categoryRepository.SaveChangesAsync();

            await transactionManager.CommitTransactionAsync();
            transactionManager.DisposeTransaction();
            return CategoryCommandMessages.CategoryDeletedSuccess;
        }
        catch
        {
            await transactionManager.RollbackAsync();
            transactionManager.DisposeTransaction();
            return Result<string>.Failure(400, CategoryErrors.CategoryDeleteFail);
        }
    }


    private async Task<bool> IsCategoryNameExists(string categoryName)
    {
        var category = await categoryRepository.GetByNameAsync(categoryName);
        return category != null;
    }
    private async Task DeleteBookOfCategory(Category category)
    {
        var books = await bookRepository.GetQueryable().Where(b => b.CategoryId == category.Id).ToListAsync();
        books.ForEach(book => book.IsDeleted = true);
        bookRepository.UpdateRange(books);
        await bookRepository.SaveChangesAsync();
    }
}
