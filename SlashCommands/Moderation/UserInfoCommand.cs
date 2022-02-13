using DisCatSharp;
using DisCatSharp.ApplicationCommands;
using DisCatSharp.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace BotName.SlashCommands.Utility
{
    public class UserInfoCommand : ApplicationCommandsModule
    {
        [SlashCommand("userinfo", "Views a user's info"), BannedCheck]
        public static async Task UserInfo(InteractionContext context, [Option("user", "The users info you're looking for")] DiscordUser user = null, [Option("hidden", "do you want an ephmeral output?")] bool hidden = false)
        {
            if (user == null)
            {
                var member = await context.Client.GetUserAsync(context.Member.Id);
                var roles = context.Member.Roles.ToList().Select(i => $"<@&{i.Id}> `{i.Name}`");
                var bigboiembed = new DiscordEmbedBuilder
                {
                    Description = $"User info for **{context.Member.DisplayName}**",
                    Color = context.Member.Color,
                    ImageUrl = member.BannerUrl
                };
                bigboiembed.WithThumbnail(context.Member.AvatarUrl);
                bigboiembed.AddField("General Information:", $"Tag: {context.Member.UsernameWithDiscriminator} \n ID: {context.Member.Id} \n Color: `{context.Member.Color}` \n Status: {context.Member.Presence.Status} \n Bot: {context.Member.IsBot.ToString().ToLower()}");
                bigboiembed.AddField("Dates:", $"Joined server: <t:{context.Member.JoinedAt.ToUnixTimeSeconds()}:R> \n Creation: <t:{member.CreationTimestamp.ToUnixTimeSeconds()}:R>");
                bigboiembed.AddField("Roles:", $"{string.Join(", ", roles)}");
                await context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral(hidden).AddEmbed(bigboiembed.Build()));
            }
            
            else if (await user.IsInGuild(context.Guild))
            {
                var lol = await context.Client.GetUserAsync(user.Id, true);
                var member = await context.Guild.GetMemberAsync(lol.Id);
                var roles = member.Roles.ToList().Select(i => $"<@&{i.Id}> `{i.Name}`");
                var status = member?.Presence?.Status.GetName() ?? "Offline";


                var bigboiembed = new DiscordEmbedBuilder
                {
                    Description = $"User info for **{member.DisplayName}**",
                    Color = member.Color,
                    ImageUrl = member.BannerUrl
                };
                bigboiembed.WithThumbnail(member.AvatarUrl);
                bigboiembed.AddField("General Information:", $"Tag: {member.UsernameWithDiscriminator} \n ID: {member.Id} \n Color: `{member.Color}` \n Status: {status} \n Bot: {member.IsBot.ToString().ToLower()}");
                bigboiembed.AddField("Dates:", $"Joined server: <t:{member.JoinedAt.ToUnixTimeSeconds()}:R> \n Creation: <t:{member.CreationTimestamp.ToUnixTimeSeconds()}:R>");
                bigboiembed.AddField("Roles:", $"{string.Join(", ", roles)}");
                await context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral(hidden).AddEmbed(bigboiembed.Build()));
            }

            else
            {
                var lol = await context.Client.GetUserAsync(user.Id, true);
                var bigboiembed = new DiscordEmbedBuilder
                {
                    Description = $"User info for **{lol.Username}**",
                    Color = (Optional<DiscordColor>)lol.BannerColor,
                    ImageUrl = lol.BannerUrl
                }.WithFooter("User is not in the server!");
                bigboiembed.WithThumbnail(lol.AvatarUrl);
                bigboiembed.AddField("General Information:", $"Tag: {lol.UsernameWithDiscriminator} \n ID: {lol.Id} \n Bot: {lol.IsBot.ToString().ToLower()}");
                bigboiembed.AddField("Dates:", $"Creation: <t:{lol.CreationTimestamp.ToUnixTimeSeconds()}:R>");
                await context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral(hidden).AddEmbed(bigboiembed.Build()));
            }
        }
    }
}
