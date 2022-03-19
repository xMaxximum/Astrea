using DisCatSharp;
using DisCatSharp.ApplicationCommands;
using DisCatSharp.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BotName.SlashCommands.Utility
{
    public class AboutCommand : ApplicationCommandsModule
    {
        [SlashCommand("about", "Returns info about the bot"), BannedCheck]
        public static async Task About(InteractionContext context)
        {
            await context.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

            var currentApplication = context.Client.CurrentApplication;

            float cpu;
           
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                PerformanceCounter cpuCounter;
                cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");

                cpu = cpuCounter.NextValue();
                Thread.Sleep(500);
                cpu = cpuCounter.NextValue();
            }
            else
            {
                var startCpuUsage = Process.GetCurrentProcess().TotalProcessorTime;
                var endCpuUsage = Process.GetCurrentProcess().TotalProcessorTime;

                var cpuUsedMs = (endCpuUsage - startCpuUsage).TotalMilliseconds;
                var cpuUsageTotal = cpuUsedMs / (Environment.ProcessorCount * 1000);

                cpu = (float)(cpuUsageTotal * 100);
            }
            string ram;
            using (Process proc = Process.GetCurrentProcess())
            {
                // The proc.PrivateMemorySize64 will returns the private memory usage in byte.
                // Would like to Convert it to Megabyte? divide it by 2^20
                ram = Util.FormatBytes(proc.PrivateMemorySize64); // / (1024 * 1024);
                
            }

            var embed = new DiscordEmbedBuilder()
            {
                Title = $"Information about {context.Client.CurrentUser.Username}"
            };

            embed.AddField(new DiscordEmbedField("General Information:", $"Developers: {string.Join(", ", currentApplication.Owners.ToList().Select(x => $"{x.Mention} (`{x.UsernameWithDiscriminator}`)"))}"));
            embed.AddField(new DiscordEmbedField("Statistics:", $"Guilds: {context.Client.Guilds.Count}\nUptime: { DateTime.Now - Process.GetCurrentProcess().StartTime}\n" +
                $" OS: {Environment.OSVersion.VersionString}\n" +
                $"CPU usage: {cpu}%\nRAM Usage: {ram}"));
            await context.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed));
        }
    }
}
