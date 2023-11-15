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
    private const string ModelKey = "model";
    private const string SecretKey = "apikey";
    private const string OrgKey = "org";

    public static (string model, string apiKey, string orgId) LoadFromFile(string configFile = DefaultConfigFile)
    {
        if (!File.Exists(configFile))
        {
            Console.WriteLine("Configuration not found: " + configFile);
            throw new Exception("Configuration not found");
        }

        try
        {
            var config = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(configFile));
            string model = config[ModelKey];
            string apiKey = config[SecretKey];
            string orgId = config[OrgKey];

            if (orgId == "none")
            {
                orgId = "";
            }

            return (model, apiKey, orgId);
        }
        catch (Exception e)
        {
            Console.WriteLine("Something went wrong: " + e.Message);
            return ("", "", "");
        }
    }
}