using DisCatSharp;
using DisCatSharp.ApplicationCommands;
using DisCatSharp.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BotName.SlashCommands.Utility
{
    public class AboutCommand : ApplicationCommandsModule
    {
        [SlashCommand("about", "Returns info about the bot")]
        public static async Task About(InteractionContext context)
        {
            await context.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

            var currentApplication = context.Client.CurrentApplication;
            PerformanceCounter cpuCounter;

            // WORKS ONLY ON WINDOWS
            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");

            var cpu = cpuCounter.NextValue();
            Thread.Sleep(500);
            cpu = cpuCounter.NextValue();
            var ram = 0.0;
            using (Process proc = Process.GetCurrentProcess())
            {
                // The proc.PrivateMemorySize64 will returns the private memory usage in byte.
                // Would like to Convert it to Megabyte? divide it by 2^20
                ram = proc.PrivateMemorySize64 / (1024 * 1024);
            }


            var embed = new DiscordEmbedBuilder()
            {
                Title = $"Information about {context.Client.CurrentUser.Username}"
            };

            embed.AddField("General Information:", $"Developers: {string.Join(", ", currentApplication.Owners.ToList().Select(x => $"{x.Mention} (`{x.UsernameWithDiscriminator}`)"))}");
            embed.AddField("Statistics:", $"Guilds: {context.Client.Guilds.Count}\nCPU usage: {cpu}%\nRAM Usage: {ram}MB");

            await context.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed));
        }
    }
}
