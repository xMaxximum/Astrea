using System.Threading.Tasks;
using DisCatSharp;
using DisCatSharp.ApplicationCommands;
using DisCatSharp.CommandsNext.Attributes;
using DisCatSharp.Entities;

namespace BotName.SlashCommands.Utility
{
    public class PingCommand : ApplicationCommandsModule
    {
        [SlashCommand("ping", "Returns latency"), BannedCheck]
        public static async Task Ping(InteractionContext context)
        {
            await context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .AsEphemeral(true).WithContent($"{context.Client.Ping}ms"));
        }

    }
}
