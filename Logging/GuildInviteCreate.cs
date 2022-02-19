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
    public class GuildInviteCreate
    {
        public static async Task InviteCreated(DiscordClient client, InviteCreateEventArgs args)
        {

            ulong channelId = await Database.Database.Logging.GetLogChannel(args.Guild.Id, LogType.GuildInviteCreate);
            if (channelId != 0)
            {
                await args.Guild.GetChannel(channelId).SendMessageAsync(new DiscordEmbedBuilder()
                {
                    Title = $"New invite created: {args.Invite.Url} for #{args.Channel.Name}",
                    Description = Util.AddLint($"Inviter ID: {args.Invite.Inviter.Id}\nCreation Date: {args.Invite.CreatedAt}" +
                    $"\nExpires at: {args.Invite.ExpiresAt}" +
                    $"\nMax Uses: {args.Invite.MaxUses}\nMax Age: {args.Invite.MaxAge}\nTemporary?: {args.Invite.IsTemporary}")
                }.WithFooter($"Created by {args.Invite.Inviter.UsernameWithDiscriminator}", args.Invite.Inviter.AvatarUrl));
            }

            else return;
        }
    }
}
