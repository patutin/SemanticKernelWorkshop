using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.DotNet.Interactive;
using InteractiveKernel = Microsoft.DotNet.Interactive.Kernel;
public static class Settings
{
    private const string DefaultConfigFile = "config/settings.json";
    private const string DefaultModel = "gpt-35-turbo";
    private const string ModelKey = "model";
    private const string SecretKey = "apikey";
    private const string OrgKey = "org";
    private const string EndpointKey = "endpoint";

    public static (string model, string azureEndpoint, string apiKey, string orgId) LoadFromFile(
        string configFile = DefaultConfigFile,
        string model = DefaultModel)
    {
        try
        {
            string json = File.ReadAllText(configFile);
            var settingsList = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(json);

            if (settingsList == null || settingsList.Count == 0)
            {
                Console.WriteLine("No configurations found in the file.");
                throw new Exception("No configurations found");
            }

            var config = settingsList.FirstOrDefault(_ => _[ModelKey] == model);

            if (config == null)
            {
                Console.WriteLine($"Configuration for model '{model}' not found.");
                throw new Exception($"Configuration for model '{model}' not found");
            }

            string loadedModel = GetValueOrDefault(config, ModelKey);
            string azureEndpoint = GetValueOrDefault(config, EndpointKey);
            string apiKey = GetValueOrDefault(config, SecretKey);
            string orgId = GetValueOrDefault(config, OrgKey);

            if (orgId == "none")
            {
                orgId = "";
            }

            return (loadedModel, azureEndpoint, apiKey, orgId);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Something went wrong: {e.Message}");
            return ("", "", "", "");
        }
    }

    private static string GetValueOrDefault(Dictionary<string, string> config, string key)
    {
        return config.TryGetValue(key, out var value) ? value : "";
    }
}