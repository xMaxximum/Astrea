using DisCatSharp;
using DisCatSharp.ApplicationCommands;
using DisCatSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotName.SlashCommands.Moderation
{
    public class UnbanCommand : ApplicationCommandsModule
    {
        [SlashCommand("unban", "Unbans a user"), BannedCheck]
        public static async Task Unban(InteractionContext context, [Option("user", "Set the user to unban")] DiscordUser user, [Option("reason", "Provide the reason for the unban")] string reason = null)
        {
            if (context.Client.GetGuildAsync(context.Guild.Id).Result.GetMemberAsync(context.Client.CurrentUser.Id).Result.Permissions.HasPermission(Permissions.BanMembers))
            {
                if (context.Member.Permissions.HasFlag(Permissions.BanMembers))
                {
                    await context.Guild.UnbanMemberAsync(user, reason);

                    await context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    {
                        Content = $"Successfully unbanned {user.UsernameWithDiscriminator} for '{reason}'!",
                        IsEphemeral = false
                    });
                }

                else
                {
                    await context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    {
                        Content = "You don't have the required permissions to perform this task!",
                        IsEphemeral = true
                    });
                }
            }

            else
            {
                await context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                {
                    Content = "The bot doesn't have the required permissions to perform this task!",
                    IsEphemeral = true
                });
            }
        }
    }
}
