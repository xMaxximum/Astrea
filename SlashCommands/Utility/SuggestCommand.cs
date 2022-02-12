using BotName.Database.Models;
using DisCatSharp;
using DisCatSharp.ApplicationCommands;
using DisCatSharp.Entities;
using System;
using System.Threading.Tasks;

namespace BotName.SlashCommands.Utility
{
    public class SuggestCommand : ApplicationCommandsModule
    {
        [SlashCommandGroup("suggestion", "Commands used to deal with suggestions")]
        public class SuggestCommandGroup : ApplicationCommandsModule
        {
            [SlashCommand("add", "Creates a new suggestion")]
            public async Task AddSuggestion(InteractionContext context, [Option("description", "description of the suggestion")] string description)
            {
                var guildSuggestionId = await Database.Database.Suggestions.GetGuildSuggestionsAsync(context.Guild.Id);

                await context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(new DiscordEmbedBuilder()
                {
                    Title = $"[{guildSuggestionId}] A new suggestion",
                    Description = description + SuggestionModel.SuggestionState(0)
                }.WithFooter($"Author: {context.Member.UsernameWithDiscriminator}", context.Member.AvatarUrl)));

                var message = await context.GetOriginalResponseAsync();

                await Database.Database.Suggestions.AddSuggestionAsync(new SuggestionModel()
                {
                    GuildSuggestionId = guildSuggestionId,
                    ChannelId = context.Channel.Id,
                    GuildId = context.Guild.Id,
                    MessageId = message.Id,
                    Description = description + SuggestionModel.SuggestionState(0),
                    UserId = context.Member.Id,
                    State = 0
                });

                await message.CreateReactionAsync(DiscordEmoji.FromUnicode("👍"));
                await Task.Delay(500);
                await message.CreateReactionAsync(DiscordEmoji.FromUnicode("👎"));
            }

            [SlashCommand("edit", "Edits a suggestion you made")]
            public async Task UpdateSuggestion(InteractionContext context, [Option("suggestion-id", "ID of the suggestion you want to edit")] int suggestionId, [Option("description", "the description of the suggestion")] string description)
            {
                try
                {
                    if (await Database.Database.Suggestions.IsSuggestionOwner(suggestionId, context.Member.Id))
                    {
                        await Database.Database.Suggestions.EditSuggestionAsync(description, suggestionId, context.Member.Id, context.Guild.Id);

                        var dbRequest = await Database.Database.Suggestions.GetSuggestionMsgOfGuildAsync(context.Guild.Id, suggestionId);

                        var channel = context.Guild.GetChannel(dbRequest.ChannelId);

                        var message = await channel.GetMessageAsync(dbRequest.MessageId);

                        await message.ModifyAsync(new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
                        {
                            Title = message.Embeds[0].Title,
                            Description = description + SuggestionModel.SuggestionState(dbRequest.State)
                        }.WithFooter(message.Embeds[0].Footer.Text, context.Member.AvatarUrl)));

                        await context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                        {
                            Content = "Successfully edited your suggestion!"
                        });
                    }

                    else
                    {
                        await context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                        {
                            Content = "The suggestion you specified isn't made by you so you can't edit it or it doesn't exist!"
                        });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);

                    await context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    {
                        Content = "There was an error executing this command!"
                    });
                }
            }
            
            [SlashCommand("approve", "Approves a suggestion")]
            public async Task ApproveSuggestion(InteractionContext context, [Option("suggestion-id", "ID of the suggestion you want to approve")] int suggestionId, [Option("reason", "The reason of the approving")] string reason = null)
            {
                try
                {
                    if (context.Member.Permissions.HasFlag(Permissions.Administrator))
                    {
                        await Database.Database.Suggestions.EditSuggestionState(context.Guild.Id, suggestionId, 1);

                        var dbRequest = await Database.Database.Suggestions.GetSuggestionMsgOfGuildAsync(context.Guild.Id, suggestionId);

                        var channel = context.Guild.GetChannel(dbRequest.ChannelId);

                        var message = await channel.GetMessageAsync(dbRequest.MessageId);

                        await message.ModifyAsync(new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
                        {
                            Title = message.Embeds[0].Title,
                            Description = dbRequest.Description + SuggestionModel.SuggestionState(1, reason)
                        }.WithFooter(message.Embeds[0].Footer.Text, context.Member.AvatarUrl)));

                        await context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                        {
                            Content = $"Successfully approved suggestion [`{suggestionId}`] {(reason is not null ? $"for `{reason}`" : "")}!"
                        });
                    }
                    else
                    {
                        await context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                        {
                            Content = "You don't have the required permissions to execute this command!"
                        });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);

                    await context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    {
                        Content = "There was an error executing this command!"
                    });
                }
            }

            [SlashCommand("decline", "Declines a suggestion")]
            public async Task DeclineSuggestion(InteractionContext context, [Option("suggestion-id", "ID of the suggestion you want to approve")] int suggestionId, [Option("reason", "The reason of the approving")] string reason = null)
            {
                try
                {
                    if (context.Member.Permissions.HasFlag(Permissions.Administrator))
                    {
                        await Database.Database.Suggestions.EditSuggestionState(context.Guild.Id, suggestionId, 1);

                        var dbRequest = await Database.Database.Suggestions.GetSuggestionMsgOfGuildAsync(context.Guild.Id, suggestionId);

                        var channel = context.Guild.GetChannel(dbRequest.ChannelId);

                        var message = await channel.GetMessageAsync(dbRequest.MessageId);

                        await message.ModifyAsync(new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
                        {
                            Title = message.Embeds[0].Title,
                            Description = dbRequest.Description + SuggestionModel.SuggestionState(2, reason)
                        }.WithFooter(message.Embeds[0].Footer.Text, context.Member.AvatarUrl)));

                        await context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                        {
                            Content = $"Successfully approved suggestion [`{suggestionId}`] {(reason is not null ? $"for `{reason}`" : "")}!"
                        });
                    }
                    else
                    {
                        await context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                        {
                            Content = "You don't have the required permissions to execute this command!"
                        });
                    }
                }

                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);

                    await context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    {
                        Content = "There was an error executing this command!"
                    });
                }
            }

        }
    }
}
