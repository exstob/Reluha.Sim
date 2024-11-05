using Sim.Domain.UiSchematic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace Sim.Infrastructure;

public interface IRepository
{
    public UiSchemeModel GetUiScheme(string fileName);
    public string GetSchemeContent(string fileName);
    public List<string> GetUiSchemeNames();
}

public class Repository: IRepository
{
    public UiSchemeModel GetUiScheme(string fileName = "SerialConnections.json") 
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = $"{assembly.GetName().Name}.LocalStorage.{fileName}";

        try
        {
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    throw new FileNotFoundException("Resource not found: " + resourceName);
                }

                using (StreamReader reader = new StreamReader(stream))
                {
                    string jsonContent = reader.ReadToEnd();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var model = JsonSerializer.Deserialize<UiSchemeModel>(jsonContent, options);
                    return model;
                }
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error accessing resource: {ex.Message}");
            return null;
        }

    }

    public string GetSchemeContent(string fileName = "SerialConnections.json")
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = $"{assembly.GetName().Name}.LocalStorage.{fileName}";

        try
        {
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    throw new FileNotFoundException("Resource not found: " + resourceName);
                }

                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error accessing resource: {ex.Message}");
            return null;
        }

    }

    public List<string> GetUiSchemeNames() 
    {

        var assembly = Assembly.GetExecutingAssembly();
        var resourceNames = assembly.GetManifestResourceNames()
                                    .Where(r => r.Contains(".LocalStorage."))
                                    .Select(r => r.Split(".LocalStorage.").Last())
                                    .ToList();

        return resourceNames;
    }
}
