using DisCatSharp;
using DisCatSharp.Entities;
using DisCatSharp.EventArgs;
using System.Threading.Tasks;
using static BotName.Database.Models.LoggingModel;

namespace BotName.Logging
{
    public class ThreadUpdateEvent
    {
        public static async Task ThreadUpdated(DiscordClient client, ThreadUpdateEventArgs args)
        {
            ulong channelId = await Database.Database.Logging.GetLogChannel(args.Guild.Id, LogType.ThreadUpdate);
            if (channelId != 0)
            {
                var auditlog = args.Guild.GetAuditLogsAsync(1, actionType: AuditLogActionType.ThreadUpdate).Result[0];

                string archived = args.ThreadAfter.ThreadMetadata.Archived ? $"Archived at: {args.ThreadAfter.ThreadMetadata.ArchiveTimestamp}\n" : "";

                await args.Guild.GetChannel(channelId).SendMessageAsync(new DiscordEmbedBuilder()
                {
                    Title = $"Thread updated: {(args.ThreadBefore.Name != args.ThreadAfter.Name ? $"{args.ThreadBefore.Name} -> {args.ThreadAfter.Name}" : args.ThreadAfter.Name)} in: #{args.Parent.Name}",
                    Description = Util.AddLint($"{archived}Thread ID: {args.ThreadAfter.Id}\nParent channel ID: {args.Parent.Id}\n" +
                    $"Creation Date: {args.ThreadAfter.CreationTimestamp}\n")
                }.WithFooter($"Updated by {auditlog.UserResponsible.UsernameWithDiscriminator}", auditlog.UserResponsible.AvatarUrl));
            }

            else return;
        }
    }
}
