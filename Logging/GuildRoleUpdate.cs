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
    public class GuildRoleUpdate
    {
        public static async Task RoleUpdated(DiscordClient client, GuildRoleUpdateEventArgs args)
        {

            ulong channelId = await Database.Database.Logging.GetLogChannel(args.Guild.Id, LogType.GuildRoleUpdate);
            if (channelId != 0)
            {
                var differences = args.RoleBefore.CompareRoleDifference(args.RoleAfter);

                string title = $"Role updated: {args.RoleAfter.Name} ({args.RoleAfter.Mention})";
                string color = args.RoleAfter.Color.ToString();
                string isHoisted = args.RoleAfter.IsHoisted.ToString();
                string isManaged = args.RoleAfter.IsManaged.ToString();
                string position = args.RoleAfter.Position.ToString();
                string permissions = args.RoleAfter.Permissions.ToPermissionString();

                foreach (var item in differences)
                {
                    switch (item.PropertyName)
                    {
                        case "Name":
                            title = $"Role updated: {args.RoleBefore.Name} -> {args.RoleAfter.Name} ({args.RoleAfter.Mention})";
                            break;
                        case "Color":
                            color = $"{args.RoleBefore.Color} -> {args.RoleAfter.Color}";
                            break;
                        case "IsHoisted":
                            isHoisted = $"{args.RoleBefore.IsHoisted} -> {args.RoleAfter.IsHoisted}";
                            break;
                        case "IsManaged":
                            isManaged = $"{args.RoleBefore.IsManaged} -> {args.RoleAfter.IsManaged}";
                            break;
                        case "Position":
                            position = $"{args.RoleBefore.Position} -> {args.RoleAfter.Position}";
                            break;
                        case "Permissions":
                            permissions = $"";
                            break;
                    }
                }
                var auditLog = args.Guild.GetAuditLogsAsync(1, actionType: AuditLogActionType.RoleUpdate).Result[0];

                await args.Guild.GetChannel(channelId).SendMessageAsync(new DiscordEmbedBuilder()
                {
                    ImageUrl = args.RoleAfter.IconUrl,
                    Title = title
                    /*
                    Description = Util.AddLint($"Role ID: {args.Role.Id}\nCreation Date: {args.Role.CreationTimestamp}" +
                    $"\nColor: {args.Role.Color}\nHoisted?: {args.Role.IsHoisted}\nManaged?: {args.Role.IsManaged}\nMentionable?: {args.Role.IsMentionable}\nPosition: {args.Role.Position}" +
                    $"{(args.Role.UnicodeEmoji != null ? $"Role emote: {args.Role.UnicodeEmoji.GetDiscordName()}" : "")}" +
                    $"Permissions: {permissions}" +
                    $"{(auditLog.Reason.Any() ? $"\nReason: {auditLog.Reason}" : "")}")
                    */
                }.WithFooter($"Updated by {auditLog.UserResponsible.UsernameWithDiscriminator}", auditLog.UserResponsible.AvatarUrl).WithThumbnail(args.RoleAfter.IconUrl));
            }

            else return;
        }
    }
}
