using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace StateManagementBot
{
    public class StateManagementBot : ActivityHandler
    {
        public BotState ConversationState { get; private set; }
        public BotState UserState { get; private set; }

        public StateManagementBot(Microsoft.Bot.Builder.ConversationState conversationState, UserState userState)
        {
            ConversationState = conversationState;
            UserState = userState;
        }

        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            await base.OnTurnAsync(turnContext, cancellationToken);

            // Save any state changes that might have occurred during the turn.
            await ConversationState.SaveChangesAsync(turnContext, false, cancellationToken);
            await UserState.SaveChangesAsync(turnContext, false, cancellationToken);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            await turnContext.SendActivityAsync("Welcome to State Bot Sample. Type anything to get started.");
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            // convo related
            var marklar = await ConversationState
                .CreateProperty<Marklar>(nameof(Marklar))
                .GetAsync(turnContext, () => new Marklar("Chippy"));


            var progressData = await ConversationState
                .CreateProperty<ProgressData>(nameof(ProgressData))
                .GetAsync(turnContext, () => new ProgressData());
           
           // user related
            var userName = await UserState
                .CreateProperty<string>("UserName")
                .GetAsync(turnContext, null);


            switch (progressData.Progress)
            {
                case Progress.First:
                    await turnContext.SendActivityAsync($"What is your name?");
                    progressData.Progress = Progress.Second;
                    break;

                case Progress.Second:
                    userName = turnContext.Activity.Text?.Trim();
                    progressData.Progress = string.IsNullOrWhiteSpace(userName) ? Progress.First : Progress.Third;

                    break;

                case Progress.Third:
                    await turnContext.SendActivityAsync($"Got your name `{userName}`!");
                    await turnContext.SendActivityAsync($"What now?");
                    progressData.Progress = Progress.Done;
                    break;

                default:
                    await turnContext.SendActivityAsync($"Well... that was awkward.");
                    break;
            }
        }
    }
}

