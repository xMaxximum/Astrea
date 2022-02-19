using DisCatSharp;
using DisCatSharp.Entities;
using DisCatSharp.EventArgs;
using System.Threading.Tasks;
using static BotName.Database.Models.LoggingModel;

namespace BotName.Logging
{
    public class ThreadCreateEvent
    {
        public static async Task ThreadCreated(DiscordClient client, ThreadCreateEventArgs args)
        {
            ulong channelId = await Database.Database.Logging.GetLogChannel(args.Guild.Id, LogType.ThreadCreate);
            if (channelId != 0)
            {
                var member = await args.Guild.GetMemberAsync(args.Thread.OwnerId);

                await args.Guild.GetChannel(channelId).SendMessageAsync(new DiscordEmbedBuilder()
                {
                    Title = $"New thread created: {args.Thread.Name} in: #{args.Parent.Name}",
                    Description = Util.AddLint($"Thread ID: {args.Thread.Id}\nParent channel ID: {args.Parent.Id}\n" +
                    $"Auto archive duration: {args.Thread.ThreadMetadata.AutoArchiveDuration}\n" +
                    $"Creation Date: {args.Thread.CreationTimestamp}")
                }.WithFooter($"Created by {member.UsernameWithDiscriminator}", member.AvatarUrl));
            }

            else return;
        }
    }
}
