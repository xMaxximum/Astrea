using DisCatSharp;
using DisCatSharp.Entities;
using DisCatSharp.EventArgs;
using System;
using System.Linq;
using System.Threading.Tasks;
using static BotName.Database.Models.LoggingModel;

namespace BotName.Logging
{
    public class GuildEmojiUpdateEvent
    {
        public static string FirstCharToUpper(string input) => string.Concat(input[0].ToString().ToUpper(), input.AsSpan(1));

        public static async Task GuildEmojiUpdated(DiscordClient client, GuildEmojisUpdateEventArgs args)
        {
            ulong channelId = await Database.Database.Logging.GetLogChannel(args.Guild.Id, LogType.GuildEmojisUpdate);
            if (channelId != 0)
            {
                System.Collections.Generic.KeyValuePair<ulong, DiscordEmoji> emoji;
                AuditLogActionType action = AuditLogActionType.EmojiUpdate;

                string updateType = "updated";
                if (args.EmojisBefore.Count > args.EmojisAfter.Count)
                {
                    emoji = args.EmojisBefore.Except(args.EmojisAfter).First();
                    updateType = "deleted";
                    action = AuditLogActionType.EmojiDelete;
                }
                else if (args.EmojisBefore.Count < args.EmojisAfter.Count)
                {
                    emoji = args.EmojisAfter.Except(args.EmojisBefore).First();
                    updateType = "added";
                    action = AuditLogActionType.EmojiCreate;
                }
                else
                {
                    emoji = args.EmojisAfter.Except(args.EmojisBefore).First();
                }
                var auditLog = args.Guild.GetAuditLogsAsync(1, actionType: action).Result[0];

                await args.Guild.GetChannel(channelId).SendMessageAsync(new DiscordEmbedBuilder()
                {
                    Title = $"Emote {updateType}: {emoji.Value.Name}",
                    Description = Util.AddLint($"Emote ID: {emoji.Key}"),
                    ImageUrl = (updateType != "deleted" ? null : emoji.Value.Url)
                    /*
                    Title = $"User banned: {args.Member.UsernameWithDiscriminator}",
                    Description = Util.AddLint($"User ID: {args.Member.Id}\nActionee ID: {auditLog.UserResponsible.Id}\n" +
                    $"Banned at: {auditLog.CreationTimestamp}{(auditLog.Reason != null ? $"\nReason: {auditLog.Reason}" : "")}")
                    */
                }.WithFooter($"{FirstCharToUpper(updateType)} by {auditLog.UserResponsible.UsernameWithDiscriminator}", auditLog.UserResponsible.AvatarUrl));
            }

            else return;
        }
    }
}
