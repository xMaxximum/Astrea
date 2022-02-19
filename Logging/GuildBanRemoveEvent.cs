using DisCatSharp;
using DisCatSharp.Entities;
using DisCatSharp.EventArgs;
using System.Threading.Tasks;
using static BotName.Database.Models.LoggingModel;

namespace BotName.Logging
{
    public class GuildBanRemoveEvent
    {
        public static async Task GuildBanRemoved(DiscordClient client, GuildBanRemoveEventArgs args)
        {
            ulong channelId = await Database.Database.Logging.GetLogChannel(args.Guild.Id, LogType.GuildBanRemove);
            if (channelId != 0)
            {
                var auditLog = args.Guild.GetAuditLogsAsync(1, actionType: AuditLogActionType.Ban).Result[0];

                await args.Guild.GetChannel(channelId).SendMessageAsync(new DiscordEmbedBuilder()
                {
                    Title = $"User unbanned: {args.Member.UsernameWithDiscriminator}",
                    Description = Util.AddLint($"User ID: {args.Member.Id}\nActionee ID: {auditLog.UserResponsible.Id}\n" +
                    $"Unbanned at: {auditLog.CreationTimestamp}{(auditLog.Reason != null ? $"\nReason: {auditLog.Reason}" : "")}")
                }.WithFooter($"Unbanned by {auditLog.UserResponsible.UsernameWithDiscriminator}", auditLog.UserResponsible.AvatarUrl));
            }

            else return;
        }
    }
}
