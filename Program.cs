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
        private static Task Client_GuildDownloadCompleted(DiscordClient sender, GuildDownloadCompletedEventArgs guildDownloadCompletedEventArgs)
        {
            _ = Task.Run(async () =>
            {

                var appCommandModule = typeof(ApplicationCommandsModule);
                var slashCommands = Assembly.GetExecutingAssembly().GetTypes()
                    .Where(t => appCommandModule.IsAssignableFrom(t) && !t.IsNested).ToList();

                var ac = sender.GetApplicationCommands();
                sender.Logger.Log(LogLevel.Information, "Guilds: {guildCount}", guildDownloadCompletedEventArgs.Guilds.Count);
                foreach (var command in slashCommands)
                {
                    foreach (var guildId in guildDownloadCompletedEventArgs.Guilds.Keys)
                    {
                        ac.RegisterGuildCommands(command, guildId);
                    }
                }
                await ac.RefreshCommandsAsync();


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
                discord.Logger.Log(LogLevel.Information, "Joined guild '{guildName}' ID: {guildId}", e.Guild.Name, e.Guild.Id);

                var appCommandModule = typeof(ApplicationCommandsModule);
                var slashCommands = Assembly.GetExecutingAssembly().GetTypes()
                    .Where(t => appCommandModule.IsAssignableFrom(t) && !t.IsNested).ToList();
                var ac = s.GetApplicationCommands();
                foreach (var command in slashCommands)
                {
                    ac.RegisterGuildCommands(command, e.Guild.Id);
                }
                await ac.RefreshCommandsAsync();
            };

            discord.GuildDeleted += (s, e) =>
            {
                discord.Logger.Log(LogLevel.Information, "Left guild '{guildName}' ({guildID})", e.Guild.Name, e.Guild.Id);
                return Task.CompletedTask;
            };

            discord.GuildDownloadCompleted += (client, e) => Client_GuildDownloadCompleted(client, e);


            await discord.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}
