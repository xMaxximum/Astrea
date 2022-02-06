using DisCatSharp.ApplicationCommands;
using System;
using DisCatSharp;
using DisCatSharp.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                await context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(new DiscordEmbedBuilder()
                {
                    Title = $"A new suggestion by {context.Member.DisplayName}",
                    Description = description,
                }.WithFooter($"Author: {context.Member.UsernameWithDiscriminator}", context.Member.AvatarUrl)));

                var message = await context.GetOriginalResponseAsync();

                await Database.Database.AddSuggestionAsync(new Database.Models.SuggestionModel()
                {
                    GuildSuggestionId = await Database.Database.GetGuildSuggestionsAsync(context.Guild.Id),
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
                await Database.Database.EditSuggestionAsync(new Database.Models.SuggestionModel()
                {
                    Description = description
                }, suggestionId, context.Member.Id, context.Guild.Id);
            }
        }
    }
}
