using MIDASM.Domain.SharedKernel;

namespace MIDASM.Application.Commons.Errors;

public static class CategoryErrorMessages
{
    public const string CategoryNotFound = "Category not found";
    public const string CategoryCanNotDelete = "Cannot delete due to existing books for rent in this category";
    public const string CategoryNameExists = "Category already exists";
    public const string CategoryDeleteFail = "Category delete fail, please try again";
}

public static class CategoryErrors
{
    public static Error CategoryNotFound => new("CategoryNotFound", CategoryErrorMessages.CategoryNotFound);
    public static Error CategoryCanNotDelete => new("CategoryCanNotDelete", CategoryErrorMessages.CategoryCanNotDelete);
    public static Error CategoryNameExists => new("CategoryNameExists", CategoryErrorMessages.CategoryNameExists);
    public static Error CategoryDeleteFail => new("CategoryDeleteFail", CategoryErrorMessages.CategoryDeleteFail);
}