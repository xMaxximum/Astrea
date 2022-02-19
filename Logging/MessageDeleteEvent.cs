using DisCatSharp;
using DisCatSharp.Entities;
using DisCatSharp.EventArgs;
using System.Threading.Tasks;
using static BotName.Database.Models.LoggingModel;

namespace BotName.Logging
{
    public class MessageDeleteEvent
    {
        public static async Task MessageDeleted(DiscordClient client, MessageDeleteEventArgs args)
        {
            ulong channelId = await Database.Database.Logging.GetLogChannel(args.Guild.Id, LogType.MessageDelete);
            if (channelId != 0)
            {
                var auditLog = args.Guild.GetAuditLogsAsync(1, actionType: AuditLogActionType.MessageDelete).Result[0];

                await args.Guild.GetChannel(channelId).SendMessageAsync(new DiscordEmbedBuilder()
                {
                    Title = $"Message deleted in #{args.Channel.Name}",
                    Description = Util.AddLint($"Author: {args.Message.Author.UsernameWithDiscriminator}\nAuthor ID: {args.Message.Author.Id}\n" +
                    $"Content: {args.Message.Content}\nSent Time: {args.Message.CreationTimestamp.DateTime}")

                }.WithFooter($"Deleted by {auditLog.UserResponsible.UsernameWithDiscriminator}", auditLog.UserResponsible.AvatarUrl)
                 .WithThumbnail(args.Message.Author.AvatarUrl));

            }

            else
            {
                return;
            }

        }
    }
}
