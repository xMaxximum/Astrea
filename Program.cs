using System;
using System.Reflection;
using System.Threading.Tasks;
using BotName.Commands;
using BotName.SlashCommands.Utility;
using DisCatSharp;
using DisCatSharp.ApplicationCommands;
using DisCatSharp.CommandsNext;
using Microsoft.Extensions.DependencyInjection;

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
                Token = "ODY5Njg2NDk3MjAzMzM1MTk5.YQB0qg.aP0JCYgkYQ_kCIML5RJAJdbjcwI",
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.All,
                LogTimestampFormat = "MMM dd yyyy - hh:mm:ss tt",
                MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Information
            });

            var commands = discord.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefixes = new[] { "!" },
                EnableMentionPrefix = true
            });

            commands.RegisterCommands(Assembly.GetExecutingAssembly());




            var appCommands = discord.UseApplicationCommands();

            appCommands.RegisterCommands<PingCommand>(869686963828035644);


            await discord.ConnectAsync();
            await Task.Delay(-1);
        }

    }
}
