namespace MIDASM.Contract.Helpers;

public static class FileHelper
{
    public static async Task<string> GetMailTemplateFile(string mailTemplate)
    {
        string contentRoot = Directory.GetCurrentDirectory();
        var templatePath = Path.Combine(contentRoot, "EmailTemplates", mailTemplate);
        try
        {
            return await File.ReadAllTextAsync(templatePath);
        }
        catch
        {
            return string.Empty;

        }   
    }
}
