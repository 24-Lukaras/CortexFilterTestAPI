using Azure.AI.OpenAI;
using Microsoft.Extensions.Options;
using OpenAI.Chat;
using System.ClientModel;

namespace CortexFilterTestAPI.Ai
{
    public class AzureChatClientProvider
    {
        private readonly OpenAiSettings _settings;
        public AzureChatClientProvider(IOptions<OpenAiSettings> options)
        {
            _settings = options.Value;
        }

        public ChatClient GetClient()
        {
            AzureOpenAIClient azureClient = new AzureOpenAIClient(new Uri(_settings.Endpoint), new ApiKeyCredential(_settings.Key));
            var client = azureClient.GetChatClient(_settings.DefaultModel);
            return client;
        }
    }
}
