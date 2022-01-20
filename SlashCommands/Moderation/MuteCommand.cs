using DisCatSharp.ApplicationCommands;
using DisCatSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DisCatSharp;
using System.Threading.Tasks;

namespace BotName.SlashCommands.Moderation
{
    public class MuteCommand : ApplicationCommandsModule
    {
        [SlashCommand("mute", "Times out a user for a given duration")]
        public static async Task Mute(InteractionContext context, [Option("user", "The user to mute")] DiscordUser user, [Option("duration", "Sets the duration of the mute (10s,5m,1d-28d)")] string duration = "5m", [Option("reason", "The reason for the mute")] string reason = null)
        {
            if(context.Client.GetGuildAsync(context.Guild.Id).Result.GetMemberAsync(context.Client.CurrentUser.Id).Result.Permissions.HasFlag(Permissions.ManageMessages))
            {
                if (context.Member.Permissions.HasFlag(Permissions.ManageMessages))
                {

                    var member = await context.Guild.GetMemberAsync(user.Id);
                    var bot = await context.Guild.GetMemberAsync(context.Client.CurrentUser.Id);

                    if (bot.Hierarchy > member.Hierarchy && context.Member.Hierarchy > member.Hierarchy)
                    {

                        var now = DateTime.Now;
                        var time = Util.ConvertStringToTime(duration, now);

                        if (now == time)
                        {
                            await context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                            {
                                Content = $"The duration needs to be in the form: 60s (60 seconds) / 5m (5 minutes) / 1h (1 hour) / 1d (1 day) / 1w (1 week). Keep in mind that the max amount of time you can mute somebody for are 28 days!",
                                IsEphemeral = false
                            });
                            return;
                        }
                        else if (Util.WeekDiff(now, time) > 4)
                        {
                            await context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                            {
                                Content = $"The duration of the mute can't be longer than 28 days!",
                                IsEphemeral = false
                            });
                            return;
                        }

                        else
                        {
                            await member.TimeoutAsync(time, reason);

                            await context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                            {
                                Content = $"Successfully muted {member.DisplayName} for {duration}!",
                                IsEphemeral = false
                            });
                        }
                    }

                    else if (!(bot.Hierarchy > member.Hierarchy))
                    {
                        await context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                        {
                            Content = "The bot isn't high enough in the role hierarchy to timeout this user!",
                            IsEphemeral = false
                        });
                    }

                    else if (!(context.Member.Hierarchy > member.Hierarchy))
                    {
                        await context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                        {
                            Content = "You aren't high enough in the role hierarchy to timeout this user!",
                            IsEphemeral = false
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
