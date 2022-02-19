using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotName.Database.Models
{
    public class LoggingModel
    {
        public enum LogType
        {
            GuildUpdate,
            WebhooksUpdate,
            //Messages
            MessageCreate,
            MessageDelete,
            MessageUpdate,
            MessageDeleteBulk,
            //Channels
            ChannelCreate,
            ChannelDelete,
            ChannelUpdate,
            //Threads
            ThreadCreate,
            ThreadDelete,
            ThreadUpdate,
            //Guild Bans
            GuildBanAdd,
            GuildBanRemove,
            //Guild Emojis
            GuildEmojisUpdate,
            //Guild Stickers
            GuildStickersUpdate,
            //Guild Members
            GuildMemberAdd,
            GuildMemberUpdate,
            GuildMemberDelete,
            //Guild Roles
            GuildRoleCreate,
            GuildRoleUpdate,
            GuildRoleDelete,
            //Guild Invites
            GuildInviteCreate,
            GuildInviteDelete,
            //Members
            PresenceUpdate,
            UserUpdate,
            VoiceStateUpdate
        }
    }
}
