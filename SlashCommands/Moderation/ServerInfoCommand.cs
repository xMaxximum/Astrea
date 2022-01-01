using DisCatSharp;
using DisCatSharp.ApplicationCommands;
using DisCatSharp.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace BotName.SlashCommands.Moderation
{
    public class ServerInfoCommand : ApplicationCommandsModule
    {
        [SlashCommand("serverinfo", "Gets the server's info")]
        public static async Task ServerInfo(InteractionContext context)
        {
            var guild = context.Guild;
            var bans = await guild.GetBansAsync();
            var text = guild.Channels.Where(x => x.Value.Type == ChannelType.Text).ToList();
            var voice = guild.Channels.Where(x => x.Value.Type == ChannelType.Voice).ToList().Count > 0 ? $"\n Voice Channels: {guild.Channels.Where(x => x.Value.Type == ChannelType.Voice).ToList().Count}" : "";
            var category = guild.Channels.Where(x => x.Value.Type == ChannelType.Category).ToList().Count > 0 ? $" \n Categorys: {guild.Channels.Where(x => x.Value.Type == ChannelType.Category).ToList().Count}" : "";
            var stage = guild.Channels.Where(x => x.Value.Type == ChannelType.Stage).ToList().Count > 0 ? $"\n Stage Channels: {guild.Channels.Where(x => x.Value.Type == ChannelType.Stage).ToList().Count}" : "";

            var embed = new DiscordEmbedBuilder()
            {
                Title = $"Server info for {context.Guild.Name}",
                ImageUrl = guild.BannerUrl,
                Color = new DiscordColor(0x2F3136)
            }.WithThumbnail(guild.IconUrl);

            embed.AddField("General Information:", $"ID: {guild.Id} \n Owner: {guild.Owner.Mention} `{guild.Owner.UsernameWithDiscriminator}` \n Creation: <t:{guild.CreationTimestamp.ToUnixTimeSeconds()}:R> \n Channels: {guild.Channels.Where(x => x.Value.IsCategory == false).ToList().Count} \n Members: {guild.MemberCount} \n Roles: {guild.Roles.Count} \n Boosts: {guild.PremiumSubscriptionCount} (Level: {guild.PremiumTier})");
            embed.AddField("Member Information:", $"Users: {guild.Members.Where(x => x.Value.IsBot == false).ToList().Count} \n Bots: {guild.Members.Where(x => x.Value.IsBot == true).ToList().Count} \n Bans: {bans.Count}");
            embed.AddField("Channel Information:", $"Text Channels: {text.Count} {voice} {category} {stage}");
            embed.AddField("In-depth Information:", $"Emotes: {guild.Emojis.Count} \n Stickers: {guild.Stickers.Count} \n MFA: {guild.MfaLevel} \n NSFW Level: {guild.NsfwLevel} \n Verification Level: {guild.VerificationLevel} \n Features: {string.Join("", guild.RawFeatures.Select(x => $"`{x.ToLower()}` ").ToList())}");

            
            await context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(embed));
        }
    }
}
