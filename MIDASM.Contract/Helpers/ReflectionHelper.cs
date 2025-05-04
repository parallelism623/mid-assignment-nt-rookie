
namespace MIDASM.Contract.Helpers;

public static class ReflectionHelper
{
    public static List<string> GetDeclareProperties<T>()
    {

        return typeof(T).GetProperties(System.Reflection.BindingFlags.Instance
                                            | System.Reflection.BindingFlags.Public
                                            | System.Reflection.BindingFlags.DeclaredOnly)
                                            .Select(c => c.Name).ToList();
    }    

    public static Dictionary<string, object> GetDictionaryValueDeclareProperties<T>(this T obj)
    {
        var dictionary = new Dictionary<string, object>();
        foreach (var it in typeof(T).GetProperties().Where(p => p.DeclaringType == typeof(T)))
        {
            dictionary.Add(it.Name, it.GetValue(obj) ?? default!);
        }
        return dictionary;
    }
}
