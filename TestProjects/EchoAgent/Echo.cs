using A2A;

namespace EchoAgent
{
    public class Echo
    {
        public void Attach(ITaskManager taskManager)
        {
            taskManager.OnMessageReceived = ProcessMessageAsync;
            taskManager.OnAgentCardQuery = GetAgentCardAsync;
        }

        private async Task<Message> ProcessMessageAsync(MessageSendParams messageSendParams, CancellationToken ct)
        {
            var request = messageSendParams.Message.Parts.OfType<TextPart>().First().Text;
            var response = $"Echo: {request}";

            return await Task.FromResult<Message>(new Message()
            {
                Role = MessageRole.Agent,
                Parts = [new TextPart() { Text = response }]
            });
        }

        private async Task<AgentCard> GetAgentCardAsync(string agentUrl, CancellationToken cancellationToken)
        {
            return await Task.FromResult<AgentCard>(new AgentCard()
            {
                Name = "Echo Agent",
                Description = "A simple demonstration agent that echoes back any message it receives",
                Url = agentUrl,
                Capabilities = new AgentCapabilities() { Streaming = true },
                Skills =
                [
                    new AgentSkill
                {
                    Name = "Echo",
                    Description = "Echoes back the received message"
                }
                ]
            });
        }
    }
}
