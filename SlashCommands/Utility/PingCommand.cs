using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DisCatSharp;
using DisCatSharp.ApplicationCommands;
using DisCatSharp.Entities;

namespace BotName.SlashCommands.Utility
{
    public class PingCommand : ApplicationCommandsModule
    {
        [SlashCommand("ping", "Returns latency")]
        public async Task Ping(InteractionContext context)
        {
            await context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .AsEphemeral(true).WithContent($"{context.Client.Ping}"));
        }
    }
}
