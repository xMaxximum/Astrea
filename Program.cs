using BotName.SlashCommands.Utility;
using DisCatSharp;
using DisCatSharp.ApplicationCommands;
using DisCatSharp.CommandsNext;
using DisCatSharp.EventArgs;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace BotName
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        #region addSlashCommands
        private static async Task<Task> Client_GuildDownloadCompleted(DiscordClient sender, GuildDownloadCompletedEventArgs guildDownloadCompletedEventArgs, ApplicationCommandsExtension slash)
        {
            _ = Task.Run(async () =>
            {


                sender.Logger.Log(LogLevel.Debug, "Cleaning Global Commands");
                await slash.CleanGlobalCommandsAsync();
                sender.Logger.Log(LogLevel.Debug, "Cleaning guild commands");
                await slash.CleanGuildCommandsAsync();
                var appCommandModule = typeof(ApplicationCommandsModule);
                var slashCommands = Assembly.GetExecutingAssembly().GetTypes()
                    .Where(t => appCommandModule.IsAssignableFrom(t) && !t.IsNested).ToList();

                sender.Logger.Log(LogLevel.Information, $"Guilds: {guildDownloadCompletedEventArgs.Guilds.Count}");
                foreach (var command in slashCommands)
                {
                    foreach (var guildId in guildDownloadCompletedEventArgs.Guilds.Keys)
                    {
                        slash.RegisterCommands(command, guildId);
                    }
                }

                await slash.RefreshCommandsAsync();
            });
            return Task.CompletedTask;
        }
        #endregion addSlashCommands


        static async Task MainAsync()
        {
            var discord = new DiscordClient(new DiscordConfiguration()
            {
                Token = "ODY5Njg2NDk3MjAzMzM1MTk5.YQB0qg.5mjfUYOro6G2A77dOErJ1DIJqGI",
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.All,
                LogTimestampFormat = "MMM dd yyyy - hh:mm:ss tt",
                MinimumLogLevel = LogLevel.Information
            });


            var commands = discord.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefixes = new[] { "," },
                EnableMentionPrefix = true,
            });

            commands.RegisterCommands(Assembly.GetExecutingAssembly());


            var appCommands = discord.UseApplicationCommands();

            discord.GuildCreated += async (s, e) =>
            {
                discord.Logger.Log(LogLevel.Information, $"Joined guild '{e.Guild.Name}'");

                var appCommandModule = typeof(ApplicationCommandsModule);
                var slashCommands = Assembly.GetExecutingAssembly().GetTypes()
                    .Where(t => appCommandModule.IsAssignableFrom(t) && !t.IsNested).ToList();
                foreach (var command in slashCommands)
                {
                    appCommands.RegisterCommands(command, e.Guild.Id);

                }
                await appCommands.RefreshCommandsAsync();
            };

            discord.GuildDeleted += async (s, e) =>
            {
                discord.Logger.Log(LogLevel.Information, $"Left guild '{e.Guild.Name}'");
            };

            discord.GuildDownloadCompleted += (client, e) => Client_GuildDownloadCompleted(client, e, appCommands);

            discord.MessageCreated += async (client, e) =>
            {
                if ((bool)(e.Message?.MentionedUsers.Contains(client.CurrentUser)))
                {
                    await client.SendMessageAsync(e.Channel, "Im alive");
                }
            };

            await discord.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}
