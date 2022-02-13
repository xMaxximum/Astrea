using DisCatSharp;
using DisCatSharp.ApplicationCommands;
using DisCatSharp.Entities;
using System;
using System.Threading.Tasks;

namespace BotName.SlashCommands.Moderation
{
    public class PurgeCommand : ApplicationCommandsModule
    {
        [SlashCommand("purge", "Purge messages"), BannedCheck]
        public static async Task Purge(InteractionContext context, [Option("amount", "Amount of messages to purge")] int amount, [Option("reason", "Reason for deleting the messages")] string reason = null, [Option("hidden", "Should the result message be hidden?")] bool hidden = false)
        {
            if (context.Client.GetGuildAsync(context.Guild.Id).Result.GetMemberAsync(context.Client.CurrentUser.Id).Result.Permissions.HasPermission(Permissions.ManageMessages))
            {
                if (context.Member.Permissions.HasFlag(Permissions.ManageMessages))
                {
                    if (amount > 1000)
                    {
                        await context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                        {
                            Content = "A single purge can't delete more than 1000 messages at a time!",
                            IsEphemeral = hidden
                        });
                    }


                    if (amount < 100)
                    {
                        for (int i = amount; i >= 100; i -= 100)
                        {
                            amount -= 100;

                            var kek = await context.Channel.GetMessagesAsync(100);
                            await context.Channel.DeleteMessagesAsync(kek, reason);
                        }

                        if (amount > 0)
                        {
                            var oof = await context.Channel.GetMessagesAsync(amount);
                            await context.Channel.DeleteMessagesAsync(oof, reason);
                        }
                    }

                    else
                    { 
                        var messages = await context.Channel.GetMessagesAsync(amount);
                        await context.Channel.DeleteMessagesAsync(messages, reason);
                    }

                    await context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    {
                        Content = $"Successfully deleted {amount} messages!",
                        IsEphemeral = hidden
                    });
                }

                else
                {
                    await context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    {
                        Content = "You don't have the required permissions to perform this task!",
                        IsEphemeral = hidden,
                    });
                }
            }

            else
            {
                await context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                {
                    Content = "The bot doesn't have the required permissions to perform this task!",
                    IsEphemeral = hidden,
                });
            }
        }
    }
}
