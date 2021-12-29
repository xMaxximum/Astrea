using DisCatSharp;
using DisCatSharp.ApplicationCommands;
using DisCatSharp.ApplicationCommands.Attributes;
using DisCatSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BotName.SlashCommands.Utility
{
    public class HelpCommand : ApplicationCommandsModule
    {
        [SlashCommand("help", "Displays commands or more infos regarding a command")]
        public static async Task Help(InteractionContext context, [Autocomplete(typeof(AutocompleteCommandProvider))] [Option("command", "The command you'd like more info about", true)] string command = null, [Option("hidden", "Do you want the response to be hidden?")] bool hidden = false)
        {
            var appCommandModule = typeof(ApplicationCommandsModule);
            var slashCommands = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => appCommandModule.IsAssignableFrom(t) && !t.IsNested).ToList();

            if (command == null)
            {
                var response = new DiscordInteractionResponseBuilder()
                {
                }.AddEmbed(new DiscordEmbedBuilder()
                {
                    Title = "Commands",
                    Description = "Commands of the Bot"
                }.AddField("Commands", string.Join(", ", slashCommands.Select(x => x.Name.ToLower().Replace("command", string.Empty)))));

                await context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, response);
            }

            else
            {

            }


        }
    }

    public class AutocompleteCommandProvider : IAutocompleteProvider
    {
        public async Task<IEnumerable<DiscordApplicationCommandAutocompleteChoice>> Provider(AutocompleteContext ctx)
        {
            var commands = ctx.Client.GetApplicationCommands().RegisteredCommands.Where(key => key.Key == ctx.Guild.Id).First().Value.ToList();


            var options = new List<DiscordApplicationCommandAutocompleteChoice>();

            foreach (var command in commands)
            {
                options.Add(new DiscordApplicationCommandAutocompleteChoice(command.Name, command.Name));
            }

            return await Task.FromResult(options.Where(x => x.Name.StartsWith(ctx.FocusedOption.Value.ToString().ToLower())));
        }
    }
}
