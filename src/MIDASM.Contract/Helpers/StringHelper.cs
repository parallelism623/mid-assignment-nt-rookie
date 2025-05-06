
namespace MIDASM.Contract.Helpers;

public static class StringHelper
{
    public static string ReplacePlaceholders(string template, params string[] values)
    {
        foreach (var value in values)
        {
            int start = template.IndexOf('{');
            if (start == -1) break;

            int end = template.IndexOf('}', start + 1);
            if (end == -1) break;

            string placeholder = template.Substring(start, end - start + 1);
            template = template.Replace(placeholder, value);
        }
        return template;
    }
    public static string SerializePropertiesChanges(Dictionary<string, (string?, string?)> changes)
    {
        var lines = changes.Select(kvp =>
        {
            var key = kvp.Key;
            var oldVal = kvp.Value.Item1 ?? "null";
            var newVal = kvp.Value.Item2 ?? "null";
            return $" - {key}: \"{oldVal}\" → \"{newVal}\"";
        });

        return string.Join("||", lines);
    }
}
