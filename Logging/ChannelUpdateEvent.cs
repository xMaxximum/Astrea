using DisCatSharp;
using DisCatSharp.Entities;
using DisCatSharp.EventArgs;
using System.Linq;
using System.Threading.Tasks;
using static BotName.Database.Models.LoggingModel;

namespace BotName.Logging
{
    public class ChannelUpdateEvent
    {
        public static async Task ChannelUpdated(DiscordClient client, ChannelUpdateEventArgs args)
        {
            string channelType = "???";

            switch (args.ChannelAfter.Type)
            {
                case ChannelType.Text:
                    channelType = "channel";
                    break;
                case ChannelType.Private:
                    return;
                case ChannelType.Voice:
                    channelType = "voice channel";
                    break;
                case ChannelType.Category:
                    channelType = "category";
                    break;
                case ChannelType.Unknown:
                    break;
                default:
                    break;
            }

            ulong channelId = await Database.Database.Logging.GetLogChannel(args.Guild.Id, LogType.ChannelUpdate);
            if (channelId != 0)
            {
                var auditLog = args.Guild.GetAuditLogsAsync(1, actionType: AuditLogActionType.ChannelUpdate).Result[0];

                await args.Guild.GetChannel(channelId).SendMessageAsync(new DiscordEmbedBuilder()
                {
                    Title = $"{channelType} updated: #{args.ChannelBefore.Name}",
                    Description = Util.AddLint($"New name: {args.ChannelAfter.Name}\n{channelType} ID: {args.ChannelAfter.Id}\nCreation Date: {args.ChannelAfter.CreationTimestamp}" +
                    $"{(auditLog.Reason.Any() ? $"\nReason: {auditLog.Reason}" : "")}")
                }.WithFooter($"Updated by {auditLog.UserResponsible.UsernameWithDiscriminator}", auditLog.UserResponsible.AvatarUrl));

            }

            else return;
        }
    }
}
