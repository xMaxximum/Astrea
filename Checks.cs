using DisCatSharp.ApplicationCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotName
{
    public class IsBannedCheck : SlashCheckBaseAttribute
    {
        public override Task<bool> ExecuteChecksAsync(InteractionContext context)
        {
            return Task.FromResult(true);
        }
    }
}
