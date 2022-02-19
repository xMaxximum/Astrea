using DisCatSharp;
using DisCatSharp.Entities;
using DisCatSharp.EventArgs;
using System.Linq;
using System.Threading.Tasks;
using static BotName.Database.Models.LoggingModel;

namespace BotName.Logging
{
    public class ChannelDeleteEvent
    {
        public static async Task ChannelDeleted(DiscordClient client, ChannelDeleteEventArgs args)
        {
            string channelType = "???";

            switch (args.Channel.Type)
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

            ulong channelId = await Database.Database.Logging.GetLogChannel(args.Guild.Id, LogType.ChannelDelete);
            if (channelId != 0)
            { 
                var auditLog = args.Guild.GetAuditLogsAsync(1, actionType: AuditLogActionType.ChannelDelete).Result[0];

                await args.Guild.GetChannel(channelId).SendMessageAsync(new DiscordEmbedBuilder()
                {
                    Title = $"{channelType} deleted: '{args.Channel.Name}'",
                    Description = Util.AddLint($"{channelType} ID: {args.Channel.Id}\nCreation Date: {args.Channel.CreationTimestamp}" +
                    $"Deletion Date: {auditLog.CreationTimestamp}{(auditLog.Reason.Any() ? $"\nReason: {auditLog.Reason}" : "")}")
                }.WithFooter($"Deleted by {auditLog.UserResponsible.UsernameWithDiscriminator}", auditLog.UserResponsible.AvatarUrl));
            }

            else return;
        }
    }
}
