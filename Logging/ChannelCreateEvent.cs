using DisCatSharp;
using DisCatSharp.Entities;
using DisCatSharp.EventArgs;
using System.Linq;
using System.Threading.Tasks;
using static BotName.Database.Models.LoggingModel;

namespace BotName.Logging
{
    public class ChannelCreateEvent
    {
        public static async Task ChannelCreated(DiscordClient client, ChannelCreateEventArgs args)
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

            ulong channelId = await Database.Database.Logging.GetLogChannel(args.Guild.Id, LogType.ChannelCreate);
            if (channelId != 0)
            {

                var auditLog = args.Guild.GetAuditLogsAsync(1, actionType: AuditLogActionType.ChannelCreate).Result[0];

                await args.Guild.GetChannel(channelId).SendMessageAsync(new DiscordEmbedBuilder()
                {
                    Title = $"New {channelType} created: #{args.Channel.Name}",
                    Description = Util.AddLint($"{channelType} ID: {args.Channel.Id}\nCreation Date: {args.Channel.CreationTimestamp}" +
                    $"{(auditLog.Reason.Any() ? $"Reason: {auditLog.Reason}" : "")}")
                }.WithFooter($"Created by {auditLog.UserResponsible.UsernameWithDiscriminator}", auditLog.UserResponsible.AvatarUrl));
            }

            else return;
        }
    }
}
