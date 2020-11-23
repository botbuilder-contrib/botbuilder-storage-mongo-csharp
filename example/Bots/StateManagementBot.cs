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
            await turnContext.SendActivityAsync("Hi!");
            await turnContext.SendActivityAsync("Please say `hi` to get started.");
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            // convo related
            var progressData = await ConversationState
                .CreateProperty<Marklar<ProgressData>>("ConvoPropOne")
                .GetAsync(turnContext, () => new Marklar<ProgressData>(new ProgressData()));

            var someNumber = await ConversationState
                .CreateProperty<Marklar<int>>("ConvoPropTwo")
                .GetAsync(turnContext, () => new Marklar<int>(42));

            // user related
            var someString = await ConversationState
                .CreateProperty<Marklar<string>>("UserPropOne")
                .GetAsync(turnContext, () => new Marklar<string>(null));


            switch (progressData.Value.Current)
            {
                case Progress.First:
                    await turnContext.SendActivityAsync($"What is your name?");
                    progressData.Value.AdvanceIf(() => true);
                    break;

                case Progress.Second:
                    someString.Value = turnContext.Activity.Text?.Trim();
                    progressData.Value.AdvanceIf(() => string.IsNullOrWhiteSpace(someString.Value));

                    break;

                case Progress.Third:
                    await turnContext.SendActivityAsync($"Got your name `{someString.Value}`!");
                    await turnContext.SendActivityAsync($"What now?");
                    progressData.Value.AdvanceIf(() => true);
                    break;

                default:
                    await turnContext.SendActivityAsync($"Well... that was awkward.");
                    progressData.Value.Current = Progress.First;
                    
                    break;
            }
        }
    }
}

