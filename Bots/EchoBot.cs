// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with EchoBot .NET Template version v4.17.1

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EchoBot.Services;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace EchoBot.Bots;

public class EchoBot : ActivityHandler
{
    private readonly IIntentRecognizerService _intentRecognizerService;

    public EchoBot(IIntentRecognizerService intentRecognizerService)
    {
        _intentRecognizerService = intentRecognizerService;
    }

    protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
    {
        var intent = await _intentRecognizerService.RecognizeAsync(turnContext.Activity.Text);
        var reply = DetermineReply(intent);
        await turnContext.SendActivityAsync(reply, cancellationToken);
    }

    protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
    {
        var welcomeText = "Hello and welcome!";
        foreach (var member in membersAdded)
        {
            if (member.Id != turnContext.Activity.Recipient.Id)
            {
                await turnContext.SendActivityAsync(MessageFactory.Text(welcomeText, welcomeText), cancellationToken);
            }
        }
    }

    private static IMessageActivity DetermineReply(Intent intent)
    {
        return intent switch
        {
            Intent.RequestTime => MessageFactory.Text($"Het is nu {DateTime.Now:HH:mm:ss}"),
            Intent.None => MessageFactory.SuggestedActions(new[] { "Hoe laat is het?", "Wat is de tijd?" }, "Ik begreep je niet helemaal. Probeer een van de opties hieronder."),
            _ => throw new ArgumentOutOfRangeException(nameof(intent), intent, null)
        };
    }
}