using DisCatSharp;
using DisCatSharp.Entities;
using DisCatSharp.EventArgs;
using System.Threading.Tasks;
using static BotName.Database.Models.LoggingModel;

namespace BotName.Logging
{
    public class GuildInviteDelete
    {
        public static async Task InviteDeleted(DiscordClient client, InviteDeleteEventArgs args)
        {

            ulong channelId = await Database.Database.Logging.GetLogChannel(args.Guild.Id, LogType.GuildInviteDelete);
            if (channelId != 0)
            {
                string footerName;
                string footerIconUrl;
                var auditLog = await Util.TryGetAuditLogEntry(args.Guild, AuditLogActionType.InviteDelete);
                if (args.Invite.IsRevoked)
                {
                    footerName = args.Invite.Inviter.UsernameWithDiscriminator;
                    footerIconUrl = args.Invite.Inviter.AvatarUrl;
                }
                else
                {
                    footerName = auditLog.UserResponsible.UsernameWithDiscriminator;
                    footerIconUrl = auditLog.UserResponsible.AvatarUrl;
                }

                await args.Guild.GetChannel(channelId).SendMessageAsync(new DiscordEmbedBuilder()
                {
                    Title = $"Invite deleted: {args.Invite.Url} for #{args.Channel.Name}",
                    Description = Util.AddLint($"Inviter ID: {args.Invite.Inviter.Id}\nCreation Date: {args.Invite.CreatedAt}" +
                    $"\nUses: {args.Invite.Uses}\nTemporary?: {args.Invite.IsTemporary}")
                }.WithFooter($"Deleted by {footerName}", footerIconUrl));
            }

            else return;
        }
    }
}
