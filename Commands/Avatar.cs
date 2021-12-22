using System.Threading.Tasks;
using DisCatSharp.CommandsNext;
using DisCatSharp.CommandsNext.Attributes;
using DisCatSharp.Entities;
public class AvatarModule : BaseCommandModule
{
    [Command("avatar")]
    public async Task AvatarCommand(CommandContext ctx, DiscordMember member)
    {
      var embed = new DiscordEmbedBuilder{
        Description = $"Avatar **[URL]({member.AvatarUrl})** for `{member.UsernameWithDiscriminator}`",
        Color = member.Color,
        ImageUrl = member.AvatarUrl
      };
      await ctx.RespondAsync(embed);
    }
}
