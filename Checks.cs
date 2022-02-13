using DisCatSharp.ApplicationCommands;
using DisCatSharp.CommandsNext;
using DisCatSharp.CommandsNext.Attributes;
using System.Threading.Tasks;

namespace BotName
{
    public class IsBannedCheck : CheckBaseAttribute
    {
        public override async Task<bool> ExecuteCheckAsync(CommandContext context, bool help)
        {
            if (await Database.Database.Blacklists.BlacklistContains(context.Member.Id))
            {
                return false;
            }

            else
            {
                return true;
            }
        }
    }

    public class BannedCheck : SlashCheckBaseAttribute
    {
        public override async Task<bool> ExecuteChecksAsync(InteractionContext ctx)
        {
            if (await Database.Database.Blacklists.BlacklistContains(ctx.Member.Id))
            {
                return false;
            }

            else return true;
        }
    }
}
