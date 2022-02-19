using DisCatSharp;
using DisCatSharp.Entities;
using DisCatSharp.EventArgs;
using System.Linq;
using System.Threading.Tasks;
using static BotName.Database.Models.LoggingModel;

namespace BotName.Logging
{
    public class MessageBulkDeleteEvent
    {
        public static async Task MessageBulkDeleted(DiscordClient client, MessageBulkDeleteEventArgs args)
        {

            ulong channelId = await Database.Database.Logging.GetLogChannel(args.Guild.Id, LogType.MessageDeleteBulk);
            if (channelId != 0)
            {
                var auditLog = args.Guild.GetAuditLogsAsync(1, actionType: AuditLogActionType.MessageBulkDelete).Result[0];

                await args.Guild.GetChannel(channelId).SendMessageAsync(new DiscordEmbedBuilder()
                {
                    Title = $"Bulk Messages deleted in #{args.Channel.Name}",
                    Description = Util.AddLint($"Deletion actionee: {auditLog.UserResponsible.UsernameWithDiscriminator}\nDeletion actionee ID: {auditLog.UserResponsible.Id}" +
                    $"Messages deleted: {args.Messages.Count}\nTime of deletion: {auditLog.CreationTimestamp}{(auditLog.Reason.Any() ? $"\nReason: {auditLog.Reason}" : "")}")
                });

            }
            else return;
        }
    }
}
