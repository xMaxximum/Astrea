using DisCatSharp;
using DisCatSharp.ApplicationCommands;
using DisCatSharp.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BotName.SlashCommands.Moderation
{
    public class BanCommand : ApplicationCommandsModule
    {
        [SlashCommand("ban", "Bans an user")]
        public static async Task Ban(InteractionContext context, [Option("user", "The user to ban")] DiscordUser user, [Option("delete-message-days", "how many messages to delete")] int days = 0, [Option("reason", "The reason for the ban")] string reason = null)
        {
            if (context.Client.GetGuildAsync(context.Guild.Id).Result.GetMemberAsync(context.Client.CurrentUser.Id).Result.Permissions.HasPermission(Permissions.BanMembers))
            {
                if (context.Member.Permissions.HasFlag(Permissions.BanMembers))
                {
                    if (await user.IsInGuild(context.Guild))
                    {
                        var member = await context.Guild.GetMemberAsync(user.Id);
                        var bot = await context.Guild.GetMemberAsync(context.Client.CurrentUser.Id);


                        if (bot.Hierarchy > member.Hierarchy && context.Member.Hierarchy > member.Hierarchy)
                        {
                            await context.Guild.BanMemberAsync((DiscordMember)user, days, reason);

                            await context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                            {
                                Content = $"Successfully banned {user.UsernameWithDiscriminator} for '{reason}' and deleted {days} days worth of messages!",
                                IsEphemeral = false
                            });
                        }

                        else if (!(bot.Hierarchy > member.Hierarchy))
                        {
                            await context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                            {
                                Content = "The bot isn't high enough in the role hierarchy to ban this user!",
                                IsEphemeral = true
                            });
                        }

                        else if (!(context.Member.Hierarchy > member.Hierarchy)) 
                        {
                            await context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                            {
                                Content = "You aren't high enough in the role hierarchy to ban this user!",
                                IsEphemeral = true
                            });
                        }

                    }

                    else
                    {
                        Console.WriteLine(context.Interaction.Data.Options.First().Value.ToString());
                        ulong.TryParse(context.Interaction.Data.Options.First().Value.ToString(), out var memberId);
                        await context.Guild.BanMemberAsync(memberId, days, reason);

                        await context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                        {
                            Content = $"Successfully banned {user.UsernameWithDiscriminator} for '{reason}' and deleted {days} days worth of messages!",
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
                    IsEphemeral = true,
                });
            }
        }
    }
}
