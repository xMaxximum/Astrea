using DisCatSharp;
using DisCatSharp.Entities;
using DisCatSharp.EventArgs;
using System.Threading.Tasks;
using static BotName.Database.Models.LoggingModel;

namespace BotName.Logging
{
    public class ThreadDeleteEvent
    {
        public static async Task ThreadDeleted(DiscordClient client, ThreadDeleteEventArgs args)
        {
            ulong channelId = await Database.Database.Logging.GetLogChannel(args.Guild.Id, LogType.ThreadDelete);
            if (channelId != 0)
            {
                var auditLog = args.Guild.GetAuditLogsAsync(1, actionType: AuditLogActionType.ThreadDelete).Result[0];

                await args.Guild.GetChannel(channelId).SendMessageAsync(new DiscordEmbedBuilder()
                {
                    Title = $"Thread deleted: {args.Thread.Name} in: #{args.Parent.Name}",
                    Description = Util.AddLint($"Thread ID: {args.Thread.Id}\nParent channel ID: {args.Parent.Id}\n" +
                    $"Creation Date: {args.Thread.CreationTimestamp}")
                }.WithFooter($"Deleted by {auditLog.UserResponsible.UsernameWithDiscriminator}", auditLog.UserResponsible.AvatarUrl));
            }

            else return;
        }
    }
}
