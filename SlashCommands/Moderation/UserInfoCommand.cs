using DisCatSharp;
using DisCatSharp.ApplicationCommands;
using DisCatSharp.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BotName.SlashCommands.Utility
{
    public class UserInfoCommand : ApplicationCommandsModule
    {
        [SlashCommand("userinfo", "Views a user's info")]
        public async Task UserInfo(InteractionContext context, [Option("user", "The users info you're looking for")] DiscordUser user = null, [Option("hidden", "do you want an ephmeral output?")] bool hidden = false)
        {
            if (user == null)
            {
                var member = await context.Client.GetUserAsync(context.Member.Id);
                var roles = context.Member.Roles.ToList().Select(i => $"<@&{i.Id}> `{i.Name}`");
                var bigboiembed = new DiscordEmbedBuilder
                {
                    Description = $"Who is **{context.Member.DisplayName}** [`{context.Member.UsernameWithDiscriminator}`]\nID: {context.Member.Id}",
                    Color = context.Member.Color,
                    ImageUrl = member.BannerUrl
                }.WithFooter($"ID: {context.Member.Id}");
                bigboiembed.WithThumbnail(context.Member.AvatarUrl);
                bigboiembed.AddField("Roles:", $"{string.Join("\n ", roles)}", false);
                bigboiembed.AddField("Age Info:", $"Joined server at: `{context.Member.JoinedAt}`", false);
                await context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral(hidden).AddEmbed(bigboiembed.Build()));
            }

            else if (await user.IsInGuild(context.Guild))
            {
                var lol = await context.Client.GetUserAsync(user.Id, true);
                var bigboiembed = new DiscordEmbedBuilder
                {
                    Description = $"Who is **{lol.Username}** [`{user.UsernameWithDiscriminator}`]\nID: {user.Id} \n User is not in the server!",
                    Color = (Optional<DiscordColor>)lol.BannerColor,
                    ImageUrl = lol.BannerUrl
                }.WithFooter($"ID: {lol.Id}");
                bigboiembed.WithThumbnail(lol.AvatarUrl);
                await context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral(hidden).AddEmbed(bigboiembed.Build()));
            }

            else
            {
                var lol = await context.Client.GetUserAsync(user.Id, true);
                var member = await context.Guild.GetMemberAsync(lol.Id);
                var roles = member.Roles.ToList().Select(i => $"<@&{i.Id}> `{i.Name}`");
                var bigboiembed = new DiscordEmbedBuilder
                {
                    Description = $"Who is **{member.DisplayName}** [`{user.UsernameWithDiscriminator}`]\nID: {user.Id}",
                    Color = member.Color,
                    ImageUrl = lol.BannerUrl
                }.WithFooter($"ID: {lol.Id}");
                bigboiembed.WithThumbnail(lol.AvatarUrl);
                bigboiembed.AddField("Roles:", $"{string.Join("\n ", roles)}", false);
                bigboiembed.AddField("Age Info:", $"Joined server at: `{member.JoinedAt}`", false);
                await context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral(hidden).AddEmbed(bigboiembed.Build()));
            }
            }
    }
}
