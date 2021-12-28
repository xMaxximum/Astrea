using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DisCatSharp;
using DisCatSharp.ApplicationCommands;
using DisCatSharp.Entities;

namespace BotName.SlashCommands.Utility {
  public class UserInfoCommand: ApplicationCommandsModule { [SlashCommand("userinfo", "views a user's info")]
    public async Task UserInfo(InteractionContext context, [Option("user", "the users info you're looking for")] DiscordUser user, [Option("hidden", "do you want an ephmeral output?")] bool hidden) {
      var lol = await context.Guild.GetMemberAsync(user.Id);
      var roles = lol.Roles.ToList().Select(i =>$ "<@&{i.Id}> `{i.Name}`");
      var bigboiembed = new DiscordEmbedBuilder {
        Description = $ "Who is **{lol.DisplayName}** [`{user.UsernameWithDiscriminator}`]\nID: {user.Id}",
        Color = lol.Color,
        ImageUrl = user.BannerUrl
      };
      bigboiembed.WithThumbnail(lol.AvatarUrl);
      bigboiembed.AddField("Roles:", $ "{String.Join("\n ", roles)}", false);
      bigboiembed.AddField("Age Info:", $ "Joined server at: `{lol.JoinedAt}`", false);
      await context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral(hidden).AddEmbed(bigboiembed.Build()));
    }
  }
}
