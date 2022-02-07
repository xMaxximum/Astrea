using DisCatSharp;
using DisCatSharp.ApplicationCommands;
using DisCatSharp.Entities;
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
                var guildSuggestionId = await Database.Database.GetGuildSuggestionsAsync(context.Guild.Id);

                await context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(new DiscordEmbedBuilder()
                {
                    Title = $"[{guildSuggestionId}] A new suggestion by {context.Member.DisplayName}",
                    Description = description,
                }.WithFooter($"Author: {context.Member.UsernameWithDiscriminator}", context.Member.AvatarUrl)));

                var message = await context.GetOriginalResponseAsync();

                await Database.Database.AddSuggestionAsync(new Database.Models.SuggestionModel()
                {
                    GuildSuggestionId = guildSuggestionId,
                    ChannelId = context.Channel.Id,
                    GuildId = context.Guild.Id,
                    MessageId = message.Id,
                    Description = description,
                    UserId = context.Member.Id
                });
            }

            [SlashCommand("edit", "Edits a suggestion you made")]
            public async Task UpdateSuggestion(InteractionContext context, [Option("suggestion-id", "ID of the suggestion you want to edit")] int suggestionId, [Option("description", "the description of the suggestion")] string description)
            {

                if (await Database.Database.IsSuggestionOwner(suggestionId, context.Member.Id))
                {
                    await Database.Database.EditSuggestionAsync(description, suggestionId, context.Member.Id, context.Guild.Id);

                    var dbRequest = await Database.Database.GetSuggestionMsgOfGuildAsync(context.Guild.Id, suggestionId);

                    var channel = context.Guild.GetChannel(dbRequest.ChannelId);

                    var message = await channel.GetMessageAsync(dbRequest.MessageId);

                    await message.ModifyAsync(new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
                    {
                        Title = message.Embeds[0].Title,
                        Description = description
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
        }
    }
}
