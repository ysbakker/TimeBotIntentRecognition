using System;
using System.Text.Json;
using System.Threading.Tasks;
using Azure;
using Azure.AI.Language.Conversations;
using Azure.Core;
using Microsoft.Extensions.Configuration;

namespace EchoBot.Services;

public class IntentRecognizerService : IIntentRecognizerService
{
    private readonly IConfiguration _configuration;
    private ConversationAnalysisClient Client { get; }
    
    public IntentRecognizerService(IConfiguration configuration)
    {
        _configuration = configuration;
        var cluIsConfigured = !string.IsNullOrEmpty(configuration["CluProjectName"]) && !string.IsNullOrEmpty(configuration["CluDeploymentName"]) && !string.IsNullOrEmpty(configuration["CluAPIKey"]) && !string.IsNullOrEmpty(configuration["CluAPIHostName"]);
        if (!cluIsConfigured)
        {
            throw new ArgumentException("Clu is not configured");
        }
        
        var endpoint = new Uri($"https://{configuration["CluAPIHostName"]}");
        var credential = new AzureKeyCredential(configuration["CluAPIKey"]);

        Client = new ConversationAnalysisClient(endpoint, credential);
    }
    
    public async Task<Intent> RecognizeAsync(string text)
    {
        var data = new
        {
            analysisInput = new
            {
                conversationItem = new
                {
                    text,
                    id = "1",
                    participantId = "1",
                }
            },
            parameters = new
            {
                projectName = _configuration["CluProjectName"],
                deploymentName = _configuration["CluDeploymentName"],
                stringIndexType = "Utf16CodeUnit",
            },
            kind = "Conversation",
        };
        var result = await Client.AnalyzeConversationAsync(RequestContent.Create(data));
        if (result.ContentStream is null)
        {
            return Intent.None;
        }
        var response = JsonSerializer.Deserialize<AzureLanguageResponse>(result.ContentStream, new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true,
        });
        return response.Result.Prediction.TopIntent;
    }
}
