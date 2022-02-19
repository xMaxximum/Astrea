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
    public class GuildRoleCreate
    {
        public static async Task RoleCreated(DiscordClient client, GuildRoleCreateEventArgs args)
        {

            ulong channelId = await Database.Database.Logging.GetLogChannel(args.Guild.Id, LogType.GuildRoleCreate);
            if (channelId != 0)
            {
                var permissions = args.Role.Permissions.ToPermissionString().Split(',').Reverse().Select(x => $"`{x}`");
                var auditLog = args.Guild.GetAuditLogsAsync(1, actionType: AuditLogActionType.RoleCreate).Result[0];

                await args.Guild.GetChannel(channelId).SendMessageAsync(new DiscordEmbedBuilder()
                {
                    Title = $"New role created: {args.Role.Name} ({args.Role.Mention})",
                    Description = Util.AddLint($"Role ID: {args.Role.Id}\nCreation Date: {args.Role.CreationTimestamp}" +
                    $"\nColor: {args.Role.Color}\nHoisted?: {args.Role.IsHoisted}\nManaged?: {args.Role.IsManaged}\nMentionable?: {args.Role.IsMentionable}\nPosition: {args.Role.Position}" +
                    $"{(args.Role.UnicodeEmoji != null ? $"Role emote: {args.Role.UnicodeEmoji.GetDiscordName()}" : "")}" +
                    $"Permissions: {permissions}" +
                    $"{(auditLog.Reason.Any() ? $"\nReason: {auditLog.Reason}" : "")}")
                }.WithFooter($"Created by {auditLog.UserResponsible.UsernameWithDiscriminator}", auditLog.UserResponsible.AvatarUrl).WithThumbnail(args.Role.IconUrl));
            }

            else return;
        }
    }
}
