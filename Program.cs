﻿using DisCatSharp;
using DisCatSharp.ApplicationCommands;
using DisCatSharp.CommandsNext;
using DisCatSharp.EventArgs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace BotName
{
    class Program
    {
        static void Main(string[] args)
        {
            // dotnet publish -c Release -r linux-arm64 --self-contained=true -p:PublishSingleFile=true -p:GenerateRuntimeConfigurationFiles=true
            System.IO.Directory.CreateDirectory("temp");
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
            LogLevel logLevel = LogLevel.Information;
            logLevel = ConfigurationManager.ConnectionStrings["Loglevel"].ConnectionString switch
            {
                "Trace" => LogLevel.Trace,
                "Debug" => LogLevel.Debug,
                "Information" => LogLevel.Information,
                _ => LogLevel.Information,
            };

            var discord = new DiscordShardedClient(new DiscordConfiguration()
            {
                Token = ConfigurationManager.ConnectionStrings["Token"].ConnectionString,
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.All,
                LogTimestampFormat = "MMM dd yyyy - hh:mm:ss tt",
                MinimumLogLevel = logLevel
            });

            var services = new ServiceCollection().BuildServiceProvider();

            var commands = discord.UseCommandsNextAsync(new CommandsNextConfiguration()
            {
                StringPrefixes = new[] { "," },
                EnableMentionPrefix = true,
                ServiceProvider = services
            });

            commands.Result.RegisterCommands(Assembly.GetExecutingAssembly());


            var appCommands = discord.UseApplicationCommandsAsync();

            #region Eventlistener Registration
            discord.ClientErrored += async (client, args) => 
            {
                await client.GetGuildAsync(869686963828035644).Result.GetChannel(954735094231339099).GetWebhooksAsync().Result[0].ExecuteAsync(new DisCatSharp.Entities.DiscordWebhookBuilder()
                {
                    Content = $"<@724702329693274114>, Error happened! {DateTime.Now}",
                }.AddEmbed(new DisCatSharp.Entities.DiscordEmbedBuilder()
                {
                    Title = $"{args.EventName}: {args.Exception.Message}",
                    Description = args.Exception.StackTrace
                } ));
            };

            discord.SocketErrored += async (client, args) =>
            {
                await client.GetGuildAsync(869686963828035644).Result.GetChannel(954735094231339099).GetWebhooksAsync().Result[0].ExecuteAsync(new DisCatSharp.Entities.DiscordWebhookBuilder()
                {
                    Content = $"<@724702329693274114>, Error happened! {DateTime.Now}",
                }.AddEmbed(new DisCatSharp.Entities.DiscordEmbedBuilder()
                {
                    Title = args.Exception.Message,
                    Description = args.Exception.StackTrace
                }));
            };

            discord.GuildCreated += async (s, e) =>
            {
                discord.Logger.Log(LogLevel.Information, "Joined guild '{guildName}' ID: {guildId}", e.Guild.Name, e.Guild.Id);

                await Database.Database.Logging.AddLogServer(e.Guild.Id);

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
            
            discord.MessageDeleted += Logging.MessageDeleteEvent.MessageDeleted;
            discord.MessageUpdated += Logging.MessageUpdateEvent.MessageUpdated;
            discord.MessagesBulkDeleted += Logging.MessageBulkDeleteEvent.MessageBulkDeleted;
            discord.ChannelCreated += Logging.ChannelCreateEvent.ChannelCreated;
            discord.ChannelDeleted += Logging.ChannelDeleteEvent.ChannelDeleted;
            discord.ChannelUpdated += Logging.ChannelUpdateEvent.ChannelUpdated;
            discord.ThreadCreated += Logging.ThreadCreateEvent.ThreadCreated;
            discord.ThreadUpdated += Logging.ThreadUpdateEvent.ThreadUpdated;
            discord.ThreadDeleted += Logging.ThreadDeleteEvent.ThreadDeleted;
            discord.GuildBanAdded += Logging.GuildBanAddEvent.GuildBanAdded;
            discord.GuildBanRemoved += Logging.GuildBanRemoveEvent.GuildBanRemoved;
            discord.GuildEmojisUpdated += Logging.GuildEmojiUpdateEvent.GuildEmojiUpdated;
            discord.GuildStickersUpdated += Logging.GuildStickerUpdateEvent.GuildStickerUpdated;
            discord.GuildRoleCreated += Logging.GuildRoleCreate.RoleCreated;
            discord.GuildRoleUpdated += Logging.GuildRoleUpdate.RoleUpdated;
            discord.GuildRoleDeleted += Logging.GuildRoleDelete.RoleDeleted;
            discord.InviteCreated += Logging.GuildInviteCreate.InviteCreated;
            discord.InviteDeleted += Logging.GuildInviteDelete.InviteDeleted;

            discord.GuildDownloadCompleted += (client, e) => Client_GuildDownloadCompleted(client, e);
            #endregion Eventlistener Registration

            await discord.StartAsync();
            await Task.Delay(-1);
        }
    }
}
