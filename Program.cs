using System;
using System.Reflection;
using System.Threading.Tasks;
using BotName.Commands;
using BotName.SlashCommands;
using BotName.SlashCommands.Moderation;
using BotName.SlashCommands.Utility;
using DisCatSharp;
using DisCatSharp.ApplicationCommands;
using DisCatSharp.CommandsNext;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BotName
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            var discord = new DiscordClient(new DiscordConfiguration()
            {
                Token = "ODY5Njg2NDk3MjAzMzM1MTk5.YQB0qg.5mjfUYOro6G2A77dOErJ1DIJqGI",
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.All,
                LogTimestampFormat = "MMM dd yyyy - hh:mm:ss tt",
                MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Information
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
                appCommands.RegisterCommands<PingCommand>(e.Guild.Id);
                appCommands.RegisterCommands<KickCommand>(e.Guild.Id);
                appCommands.RegisterCommands<BanCommand>(e.Guild.Id);
                appCommands.RegisterCommands<UserInfoCommand>(e.Guild.Id);
                await appCommands.RefreshCommandsAsync();
            };

            discord.GuildDeleted += async (s, e) => 
            {
                discord.Logger.Log(LogLevel.Information, $"Left guild '{e.Guild.Name}'");
            };
            

            await discord.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}
