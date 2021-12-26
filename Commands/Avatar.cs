using System.Threading.Tasks;
using DisCatSharp.CommandsNext;
using DisCatSharp.CommandsNext.Attributes;
using DisCatSharp.Entities;
public class AvatarModule : BaseCommandModule
{
    [Command("avatar")]
    public async Task AvatarCommand(CommandContext ctx, DiscordMember member = null)
    {
        if (member == null) 
        {
            var embed = new DiscordEmbedBuilder
            {
                Description = $"Avatar **[URL]({ctx.Member.AvatarUrl})** for `{ctx.Member.UsernameWithDiscriminator}`",
                Color = ctx.Member.Color,
                ImageUrl = ctx.Member.AvatarUrl
            };

            _ = ctx.RespondAsync(embed);
        }

        else
        {

            var embed = new DiscordEmbedBuilder
            {
                Description = $"Avatar **[URL]({member.AvatarUrl})** for `{member.UsernameWithDiscriminator}`",
                Color = member.Color,
                ImageUrl = member.AvatarUrl
            };

            _ = ctx.RespondAsync(embed);
        }
    }
}
