using DisCatSharp;
using DisCatSharp.Entities;
using DisCatSharp.EventArgs;
using System.Threading.Tasks;
using static BotName.Database.Models.LoggingModel;

namespace BotName.Logging
{
    public class GuildBanAddEvent
    {
        public static async Task GuildBanAdded(DiscordClient client, GuildBanAddEventArgs args)
        {
            ulong channelId = await Database.Database.Logging.GetLogChannel(args.Guild.Id, LogType.ThreadCreate);
            if (channelId != 0)
            {
                var auditLog = args.Guild.GetAuditLogsAsync(1, actionType: AuditLogActionType.Ban).Result[0];

                await args.Guild.GetChannel(channelId).SendMessageAsync(new DiscordEmbedBuilder()
                {
                    Title = $"User banned: {args.Member.UsernameWithDiscriminator}",
                    Description = Util.AddLint($"User ID: {args.Member.Id}\nActionee ID: {auditLog.UserResponsible.Id}\n" +
                    $"Banned at: {auditLog.CreationTimestamp}{(auditLog.Reason != null ? $"\nReason: {auditLog.Reason}" : "")}")
                }.WithFooter($"Banned by {auditLog.UserResponsible.UsernameWithDiscriminator}", auditLog.UserResponsible.AvatarUrl));
            }

            else return;
        }
    }
}
