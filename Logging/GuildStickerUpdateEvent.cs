using DisCatSharp;
using DisCatSharp.Entities;
using DisCatSharp.EventArgs;
using System;
using System.Linq;
using System.Threading.Tasks;
using static BotName.Database.Models.LoggingModel;

namespace BotName.Logging
{
    public class GuildStickerUpdateEvent
    {
        public static string FirstCharToUpper(string input) => string.Concat(input[0].ToString().ToUpper(), input.AsSpan(1));

        public static async Task GuildStickerUpdated(DiscordClient client, GuildStickersUpdateEventArgs args)
        {
            ulong channelId = await Database.Database.Logging.GetLogChannel(args.Guild.Id, LogType.GuildStickersUpdate);
            if (channelId != 0)
            {
                System.Collections.Generic.KeyValuePair<ulong, DiscordSticker> emoji;
                AuditLogActionType action = AuditLogActionType.StickerUpdate;

                string updateType = "updated";
                if (args.StickersBefore.Count > args.StickersAfter.Count)
                {
                    emoji = args.StickersBefore.Except(args.StickersAfter).First();
                    updateType = "deleted";
                    action = AuditLogActionType.StickerDelete;
                }
                else if (args.StickersBefore.Count < args.StickersAfter.Count)
                {
                    emoji = args.StickersAfter.Except(args.StickersBefore).First();
                    updateType = "added";
                    action = AuditLogActionType.StickerDelete;
                }
                else
                {
                    emoji = args.StickersAfter.Except(args.StickersBefore).First();
                }
                var auditLog = args.Guild.GetAuditLogsAsync(1, actionType: action).Result[0];

                await args.Guild.GetChannel(channelId).SendMessageAsync(new DiscordEmbedBuilder()
                {
                    Title = $"Sticker {updateType}: {emoji.Value.Name}",
                    Description = Util.AddLint($"Sticker ID: {emoji.Key}"),
                    ImageUrl = (updateType != "deleted" ? null : emoji.Value.Url)
                }.WithFooter($"{FirstCharToUpper(updateType)} by {auditLog.UserResponsible.UsernameWithDiscriminator}", auditLog.UserResponsible.AvatarUrl));
            }

            else return;
        }
    }
}
