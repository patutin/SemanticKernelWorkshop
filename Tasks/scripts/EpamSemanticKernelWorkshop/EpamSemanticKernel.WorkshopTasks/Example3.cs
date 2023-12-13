using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.AI.OpenAI;
using Microsoft.SemanticKernel.Plugins.Core;

namespace EpamSemanticKernel.WorkshopTasks
{
#pragma warning disable SKEXP0050
	internal class Example3
	{
		public static async Task RunAsync()
		{
			var builder = new KernelBuilder();

			// Loading models and setting up OpenAI chat completion services
			var (model1, azureEndpoint1, apiKey1, gpt35TurboServiceId) = Settings.LoadFromFile(model: "gpt-35-turbo");
			builder.AddAzureOpenAIChatCompletion(model1, model1, azureEndpoint1, apiKey1, serviceId: gpt35TurboServiceId);

			var (model2, azureEndpoint2, apiKey2, gpt4ServiceId) = Settings.LoadFromFile(model: "gpt-4-32k");
			builder.AddAzureOpenAIChatCompletion(model2, model2, azureEndpoint2, apiKey2, serviceId: gpt4ServiceId);

			var kernel = builder.Build();

			// Set execution settings for OpenAI service
			var arguments = new KernelArguments
			{
				ExecutionSettings = new OpenAIPromptExecutionSettings
				{
					ServiceId = gpt35TurboServiceId,
					Temperature = 0
				}
			};

			// Set specific arguments for the findBooks function
			arguments.Add("BooksNumber", "10");
			arguments.Add("YearFrom", "1900");
			arguments.Add("YearTo", "2000");

			var timeFunctions = kernel.ImportPluginFromObject(new TimePlugin(), "time");

			arguments.Add("input", "100");

			var result = await kernel.InvokeAsync(timeFunctions["daysAgo"], arguments);

			Console.WriteLine(result);

			var customPluginFunctions = kernel.ImportPluginFromObject(new CustomPlugin(kernel, serviceId: gpt4ServiceId));
			Console.WriteLine($"{string.Join("\n", customPluginFunctions.Select(_ => $"[{_.Name}] : {_.Description}"))}");

			Func<string, string, Task> TranslateAsync = async (string input, string lang) =>
			{
				// Save new message in the context variables
				arguments["input"] = input;
				arguments["lang"] = lang;

				var answer = await customPluginFunctions["Translate"].InvokeAsync(kernel, arguments);
			};

			arguments["history"] = string.Empty;

			Func<string, Task> ChatAsync = async (string input) =>
			{
				// Save new message in the context variables
				arguments["message"] = input;

				var answer = await customPluginFunctions["Chat"].InvokeAsync(kernel, arguments);
			};

			var sourceInput = "Hi, I'm looking for the best historical book suggestions top-{{$BooksNumber}} from {{$YearFrom}} to {{$YearTo}}";

			await TranslateAsync(sourceInput, "Italian");
			await TranslateAsync(sourceInput, "Russian");
			await TranslateAsync(sourceInput, "Chinese");

			await ChatAsync(sourceInput);
			await ChatAsync("And from these books find top-10 the most interesting facts");
			await ChatAsync("Describe he main idea of selected facts");

			Console.WriteLine($"[Chat History]: {arguments["history"]}");
		}
	}
}