using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AI.OpenAI;

namespace EpamSemanticKernel.WorkshopTasks
{
	// Disable a specific warning
#pragma warning disable SKEXP0020

	internal class Example1
	{
		public static async Task RunAsync()
		{
			Console.WriteLine("");
			// Create a kernel builder to build the chatbot kernel
			var builder = new KernelBuilder();

			// Load settings for GPT-3.5 Turbo
			var (model1, azureEndpoint1, apiKey1, gpt35TurboServiceId) = Settings.LoadFromFile(model: "gpt-35-turbo");
			builder.AddAzureOpenAIChatCompletion(model1, model1, azureEndpoint1, apiKey1, serviceId: gpt35TurboServiceId);

			// Load settings for GPT-4
			var (model2, azureEndpoint2, apiKey2, gpt4ServiceId) = Settings.LoadFromFile(model: "gpt-4-32k");
			builder.AddAzureOpenAIChatCompletion(model2, model2, azureEndpoint2, apiKey2, serviceId: gpt4ServiceId);

			// Load settings for Hugging Face Text Generation
			var (model, _, apiKey, _) = Settings.LoadFromFile(model: "google/flan-t5-xxl");
			builder.AddHuggingFaceTextGeneration(model, apiKey: apiKey);

			// Build the chatbot kernel
			var kernel = builder.Build();

			// Configure OpenAI prompt execution settings
			var aiRequestSettings = new OpenAIPromptExecutionSettings
			{
				ExtensionData = new Dictionary<string, object> { { "api-version", "2023-03-15-preview" } },
				ServiceId = gpt35TurboServiceId
			};

			// Define user input for short intent
			var input = "I want to find top-10 books about world history";

			// Define a prompt template for short intents
			string skPrompt = @"ChatBot: How can I help you?
User: {{$input}}

---------------------------------------------

Return data requested by user: ";

			// Create a function from the short intent prompt
			var getShortIntentFunction = kernel.CreateFunctionFromPrompt(skPrompt, aiRequestSettings);

			// Invoke the function with user input
			var intentResult = await kernel.InvokeAsync(getShortIntentFunction, new KernelArguments(input));

			// Print the intent result
			Console.WriteLine(intentResult);

			// Define a prompt template for a conversation
			skPrompt = @"
ChatBot can have a conversation with you about any topic.
It can give a detailed answer or say 'I don't know' if it does not have an answer.

{{$history}}
User: {{$userInput}}
ChatBot:";

			// Create a chat history
			var chatHistory = new ChatHistory("You are a librarian, an expert about books");

			// Get the last message from the chat history
			var message = chatHistory.Last();
			Console.WriteLine($"{message.Role}: {message.Content}");

			// Add a user message to the chat history
			chatHistory.AddUserMessage("Hi, I'm looking for book suggestions");

			// Get the chat completion service for GPT-3.5 Turbo
			IChatCompletionService chatService = kernel.GetRequiredService<IChatCompletionService>(gpt35TurboServiceId);

			// Get a reply from the chat service
			var reply = await chatService.GetChatMessageContentAsync(chatHistory);

			// Print the reply
			Console.WriteLine(reply);

			// Get the last message from the chat history
			message = chatHistory.Last();
			Console.WriteLine($"{message.Role}: {message.Content}");

			// Add the system message to the chat history
			chatHistory.AddSystemMessage(reply);

			// Get the last message from the chat history
			message = chatHistory.Last();
			Console.WriteLine($"{message.Role}: {message.Content}");

			// Define a function for chat interactions
			Func<string, Task> Chat = async (string input) =>
			{
				// Save the new message in the context variables
				chatHistory.AddUserMessage(input);

				// Get a reply from the chat service
				var reply = await chatService.GetChatMessageContentAsync(chatHistory);

				// Add the system message to the chat history
				chatHistory.AddSystemMessage(reply);

				// Print the reply
				Console.WriteLine(reply);
			};

			// Perform multiple chat interactions
			await Chat("I would like a non-fiction book suggestion about Greece history. Please only list one book.");
			await Chat("That sounds interesting, what are some of the topics I will learn about?");
			await Chat("Which topic from the ones you listed do you think is the most popular?");

			// Print all messages in the chat history
			foreach (var msg in chatHistory)
			{
				Console.WriteLine($"{msg.Role}: {msg.Content}");
			}

			// Define user input for short intents
			input = "I want to find top-10 books about world history";

			// Create a prompt template for short intents
			skPrompt = @"ChatBot: How can I help you?
User: {{$input}}

---------------------------------------------

Return data requested by user: ";

			// Create a function from the short intent prompt
			getShortIntentFunction = kernel.CreateFunctionFromPrompt(skPrompt);

			// Invoke the function with user input
			intentResult = await kernel.InvokeAsync(getShortIntentFunction, new KernelArguments(input));

			// Print the intent result
			Console.WriteLine(intentResult);
		}
	}
}