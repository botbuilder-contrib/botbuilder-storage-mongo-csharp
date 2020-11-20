using System;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Builder.TraceExtensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace StateManagementBot
{
    public class ErrorLogginAdapter : BotFrameworkHttpAdapter
    {
        public ErrorLogginAdapter(IConfiguration configuration, ILogger<BotFrameworkHttpAdapter> logger, ConversationState conversationState = null)
            : base(configuration, logger)
        {
            OnTurnError = async (turnContext, exception) =>
            {
                logger.LogError(exception, $"[OnTurnError] unhandled error : {exception.Message}");

                await turnContext.SendActivityAsync("Server side crash. Start convo again.");

                if (conversationState != null)
                {
                    try
                    {
                        await conversationState.DeleteAsync(turnContext);
                    }
                    catch (Exception e)
                    {
                        logger.LogError(e, $"Failed deleting conversation state {e.Message}");
                    }
                }

                // Send a trace activity, which will be displayed in the Bot Framework Emulator
                await turnContext.TraceActivityAsync("OnTurnError Trace", exception.Message, "https://www.botframework.com/schemas/error", "TurnError");
            };
        }
    }
}
