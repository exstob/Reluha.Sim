using Sim.Domain.UiSchematic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace Sim.Infrastructure;

public interface IRepository
{
    public UiSchemeModel GetUiScheme(string fileName);
}

public class Repository: IRepository
{
    public UiSchemeModel GetUiScheme(string fileName = "SerialConnections.json") 
    {
        //var assembly = Assembly.GetExecutingAssembly();
        //var resourceName = assembly.GetName().Name + ".Properties." + "SerialConnections.json";
        //try
        //{
        //    using (Stream stream = assembly.GetManifestResourceStream(resourceName))
        //    using (StreamReader reader = new StreamReader(stream))
        //    {
        //        string jsonContent = reader.ReadToEnd();
        //        return JsonSerializer.Deserialize<UiSchemeModel>(jsonContent);
        //    }
        //}
        //catch 
        //{
        //    Console.Write("Error accessing resource!");
        //}

        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = $"{assembly.GetName().Name}.LocalStorage.{fileName}";

        var names = Assembly
            .GetExecutingAssembly()
        .GetManifestResourceNames();

        //using var streamReader = new StreamReader(resourceName, Encoding.UTF8);

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
}
