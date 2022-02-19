using DisCatSharp;
using DisCatSharp.Entities;
using DisCatSharp.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BotName.Database.Models.LoggingModel;

namespace BotName.Logging
{
    public class GuildRoleDelete
    {
        public static async Task RoleDeleted(DiscordClient client, GuildRoleDeleteEventArgs args)
        {
            ulong channelId = await Database.Database.Logging.GetLogChannel(args.Guild.Id, LogType.GuildRoleDelete);
            if (channelId != 0)
            {
                var auditLog = args.Guild.GetAuditLogsAsync(1, actionType: AuditLogActionType.RoleDelete).Result[0];
                var permissions = args.Role.Permissions.ToPermissionString().Split(',').Reverse().Select(x => $"`{x}`");

                await args.Guild.GetChannel(channelId).SendMessageAsync(new DiscordEmbedBuilder()
                {
                    Title = $"Role deleted: {args.Role.Name}",
                    Description = Util.AddLint($"Role ID: {args.Role.Id}\nCreation Date: {args.Role.CreationTimestamp}" +
                    $"{(auditLog.Reason.Any() ? $"\nReason: {auditLog.Reason}" : "")}")
                }.WithFooter($"Deleted by {auditLog.UserResponsible.UsernameWithDiscriminator}", auditLog.UserResponsible.AvatarUrl));
            }

            else return;
        }
    }
}
