# Set Execution Policy to allow script execution (if needed)
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass

# Configurable variables
$solutionName = "EpamSemanticKernelWorkshop"
$solutionDirectory = Join-Path $PSScriptRoot $solutionName
$projectName = "EpamSemanticKernel.WorkshopTasks"
$dotnetVersion = "7.0"
$packageName = "Microsoft.SemanticKernel"
$packageVersion = "1.0.0-rc3"

Write-Host "Installed .NET versions:"
dotnet --list-sdks

if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    Write-Error ".NET version $dotnetVersion is not installed."
    exit 1
}

Write-Host "Creating a new solution and adding a console project..."
mkdir -force $solutionDirectory
Set-Location -Path $solutionDirectory
dotnet new sln -n $solutionName
dotnet new console -n $projectName --framework net7.0
dotnet sln add $projectName

Write-Host "Adding NuGet package $packageName --version $packageVersion to the console project..."
Set-Location -Path $projectName
dotnet add package $packageName --version $packageVersion
Set-Location -Path ..

$programContent = @"

// EPAM Semantic Kernel Workshop
Console.WriteLine("Hello, Semantic Kernel Workshop!");
Console.WriteLine("This is your initial program.");

"@

$programFilePath = "$solutionDirectory\$projectName\Program.cs"
$programContent | Set-Content -Path $programFilePath -Force

# Create Config folder and Settings.cs file
$configFolder = "$solutionDirectory\$projectName\Config"
New-Item -ItemType Directory -Path $configFolder -Force
$settingsContent = @"
using System.Text.Json;

public static class Settings
{
    private const string DefaultConfigFile = "config/settings.json";
    private const string DefaultModel = "gpt-35-turbo";
    private const string ModelKey = "model";
    private const string SecretKey = "apikey";
    private const string ServiceIdKey = "serviceid";
    private const string EndpointKey = "endpoint";

    public static (string model, string azureEndpoint, string apiKey, string serviceId) LoadFromFile(
        string configFile = DefaultConfigFile,
        string model = DefaultModel)
    {
        try
        {
            if (string.IsNullOrEmpty(configFile) || !File.Exists(configFile))
            {
                Console.WriteLine("Invalid or missing configuration file path.");
                throw new ArgumentException("Invalid or missing configuration file path", nameof(configFile));
            }

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

            // Validate and retrieve values
            string loadedModel = ValidateAndGet(config, ModelKey);
            string azureEndpoint = ValidateAndGet(config, EndpointKey);
            string apiKey = ValidateAndGet(config, SecretKey);
            string serviceId = ValidateAndGet(config, ServiceIdKey);

            if (serviceId == "none")
            {
                serviceId = "";
            }

            return (loadedModel, azureEndpoint, apiKey, serviceId);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Something went wrong: {e.Message}");
            return ("", "", "", "");
        }
    }

    private static string ValidateAndGet(Dictionary<string, string> config, string key)
    {
        if (!config.TryGetValue(key, out var value) || string.IsNullOrEmpty(value))
        {
            Console.WriteLine($"Invalid or missing value for key '{key}'.");
            throw new ArgumentException($"Invalid or missing value for key '{key}'", nameof(key));
        }

        return value;
    }
}
"@

$settingsFilePath = "$configFolder\Settings.cs"
$settingsContent | Set-Content -Path $settingsFilePath -Force

# Create settings.json file
$settingsJsonContent = @'
[
  {
    "model": "gpt-35-turbo",
    "apikey": "",
    "endpoint": "https://ai-proxy.lab.epam.com",
    "serviceid": "AzureGtp35TurboService"
  },
  {
    "model": "gpt-4-32k",
    "apikey": "",
    "endpoint": "https://ai-proxy.lab.epam.com",
    "serviceid": "AzureGtp4TurboService"
  },
  {
    "model": "google/flan-t5-xxl",
    "apikey": "",
    "endpoint": "none",
    "serviceid": "HuggingFaceService"
  }
]
'@

$settingsJsonFilePath = "$configFolder\settings.json"
$settingsJsonContent | Set-Content -Path $settingsJsonFilePath -Force

Write-Host "Solution and project created successfully."

# Run the application
Write-Host "Running the application..."
Set-Location -Path $solutionDirectory
dotnet run --project $projectName
