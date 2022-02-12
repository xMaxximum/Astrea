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
}
