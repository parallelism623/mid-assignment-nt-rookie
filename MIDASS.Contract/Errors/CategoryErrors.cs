
using MIDASS.Contract.SharedKernel;

namespace MIDASS.Contract.Errors;

public static class CategoryErrorMessages
{
    public const string CategoryNotFound = "Category not found";
    public const string CategoryCanNotDelete = "Cannot delete due to existing books specified in this category";
    public const string CategoryNameExists = "Category already exists";
}

public static class CategoryErrors
{
    public static Error CategoryNotFound => new ("CategoryNotFound", CategoryErrorMessages.CategoryNotFound);
    public static Error CategoryCanNotDelete => new("CategoryCanNotDelete", CategoryErrorMessages.CategoryCanNotDelete);
    public static Error CategoryNameExists = new("CategoryNameExists", CategoryErrorMessages.CategoryNameExists);
}