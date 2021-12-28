using DisCatSharp;
using DisCatSharp.ApplicationCommands;
using DisCatSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotName.SlashCommands
{
    public class KickCommand : ApplicationCommandsModule
    {
        [SlashCommand("kick", "Kicks an user")]
        public async Task Kick(InteractionContext context, [Option("user", "The user to kick")] DiscordUser user, [Option("reason", "Provide the reason for the kick")] string reason = null)
        {
            if (context.Client.GetGuildAsync(context.Guild.Id).Result.GetMemberAsync(context.Client.CurrentUser.Id).Result.Permissions.HasPermission(Permissions.KickMembers)) {
                if (context.Member.Permissions.HasPermission(Permissions.KickMembers))
                {
                    var member = await context.Guild.GetMemberAsync(user.Id);
                    var bot = await context.Guild.GetMemberAsync(context.Client.CurrentUser.Id);

                    if (bot.Hierarchy > member.Hierarchy && context.Member.Hierarchy > member.Hierarchy)
                    {
                        await member.RemoveAsync(reason);

                        await context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                        {
                            Content = $"Successfully kicked {user.UsernameWithDiscriminator} for reason '{reason}'!",
                            IsEphemeral = false
                        });
                    }

                    else
                    {
                        await context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                        {
                            Content = "The bot or you aren't high enough in the role hierarchy to kick this user!",
                            IsEphemeral = true
                        });
                    }
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
