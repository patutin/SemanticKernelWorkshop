using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.AI.OpenAI;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.TemplateEngine;
using Microsoft.SemanticKernel.TemplateEngine.Basic;
using Microsoft.DotNet.Interactive;
using InteractiveKernel = Microsoft.DotNet.Interactive.Kernel;
using System.ComponentModel;

public class CustomPlugin
{
    private readonly IKernel _kernel;
    private readonly IPromptTemplateFactory _promptTemplateFactory;
    private readonly string _serviceId;

    public CustomPlugin(IKernel kernel, IPromptTemplateFactory promptTemplateFactory = null, string serviceId = "gpt35TurboServiceId")
    {
        _kernel = kernel;
        _promptTemplateFactory = promptTemplateFactory ?? new BasicPromptTemplateFactory();
        _serviceId = serviceId;
    }

    [SKFunction, Description("Initiates a chat with the ChatBot.")]
    public async Task<string> ChatAsync(SKContext context)
    {
        var prompt = @"
        Today is: {{time.Date}}
        Current time is: {{time.Time}}

        ChatBot can have a conversation with you about any topic.
            It can give detailed answer or say 'I don't know' if it does not have an answer.

            {{$history}}
            User: {{$message}}
            ChatBot:";

        var renderedPrompt = await _promptTemplateFactory.Create(prompt, new PromptTemplateConfig()).RenderAsync(context);

        var skFunction = _kernel.CreateSemanticFunction(
            promptTemplate: renderedPrompt,
            functionName: nameof(ChatAsync),
            pluginName: nameof(CustomPlugin),
            description: "Complete the prompt.");

        var requestSettings = new OpenAIRequestSettings
        {
            Temperature = 0.7,
            ServiceId = _serviceId
        };

        var result = await skFunction.InvokeAsync(context: context, requestSettings: requestSettings);
        var resultAsString = result.GetValue<string>();
        // Append the new interaction to the chat history
        string history = context.Variables["history"];
        history += $"\nUser: {context.Variables["message"]}\nSuggestions: {resultAsString}\n";
        return history;
    }

    [SKFunction, Description("Initiates a translation task with the ChatBot by requesting the translation of a given text to a specified language.")]
    public async Task<string> TranslateAsync(SKContext context)
    {
        var prompt = @"You are the best linguist. Please translate the given TEXT to {{$lang}}. TEXT: {{$Input}}";

        var renderedPrompt = await _promptTemplateFactory.Create(prompt, new PromptTemplateConfig()).RenderAsync(context);

        var skFunction = _kernel.CreateSemanticFunction(
            promptTemplate: renderedPrompt,
            functionName: nameof(TranslateAsync),
            pluginName: nameof(CustomPlugin),
            description: "Complete the prompt.");

        var requestSettings = new OpenAIRequestSettings
        {
            Temperature = 0.1,
            ServiceId = _serviceId
        };

        var result = await skFunction.InvokeAsync(context: context, requestSettings: requestSettings);
        return result.GetValue<string>();
    }
}